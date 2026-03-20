using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        private StreamWriter logWriter;

        private int vao, vboVertices, vboIndices, ebo;
        private float cameraDistance = 5f;
        private float angleX = 0f, angleY = 0f;
        private Point lastMousePos;

        private int edgesVao, pointsVao;

        private List<Vector3> vertexList = new List<Vector3>();
        private List<uint> indexList = new List<uint>();

        private int shaderProgram;

        private float modelAngleX = 0f;
        private float modelAngleY = 0f;

        private int selectedVertexIndex = -1;
        private int selectedFaceIndex = -1;

        private List<Face> faceList = new List<Face>();

        private int gridVao;
        private int gridVbo;
        private List<Vector3> gridLines = new List<Vector3>();

        private struct Face
        {
            public int v1, v2, v3;
        }

        private int vboEdges, vboVerticesPoints;
        private List<uint> edgeList = new List<uint>();

        private void Log(string message)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logWriter.WriteLine($"[{now}] {message}");
        }

        public MainForm()
        {
            InitializeComponent();

            logWriter = new StreamWriter("log.txt", append: true);
            logWriter.AutoFlush = true;

            SetupGLControl();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (logWriter != null)
            {
                logWriter.Close();
            }
        }

        private void SetupGLControl()
        {
            glControl.Load += (s, e) => GlControl_Load();
            glControl.Paint += (s, e) => GlControl_Paint();
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.Resize += GlControl_Resize;

            checkBoxShowEdges.CheckedChanged += (s, e) => glControl.Invalidate();
            checkBoxShowVertices.CheckedChanged += (s, e) => glControl.Invalidate();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            glControl.Invalidate();
        }

        private void GlControl_Load()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            SetupShaders();

            Log($"GlControl_Load: CullFace enabled = {GL.IsEnabled(EnableCap.CullFace)}");
            Log($"GlControl_Load: FaceCull.Mode = {GL.GetInteger(GetPName.CullFaceMode)}");
            Log($"GlControl_Load: FrontFace = {GL.GetInteger(GetPName.FrontFace)}");

            vao = GL.GenVertexArray();
            vboVertices = GL.GenBuffer();
            vboIndices = GL.GenBuffer();
        }

        private void SetupShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                uniform vec4 drawColor;
                out vec4 fColor;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                    fColor = drawColor;
                }";

            string fragmentShaderSource = @"
                #version 330 core
                in vec4 fColor;
                out vec4 FragColor;
                void main()
                {
                    FragColor = fColor;
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            CheckProgramLink(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                MessageBox.Show($"Shader compile error: {info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                MessageBox.Show($"Program link error: {info}");
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "OBJ|*.obj" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LoadModel(dlg.FileName);

                    Log($"BtnLoad_Click: vertexList.Count = {vertexList.Count}");
                    Log($"BtnLoad_Click: faceList.Count = {faceList.Count}");
                    Log($"BtnLoad_Click: indexList.Count = {indexList.Count}");

                    SetupBuffers();
                    glControl.Invalidate();
                }
            }
        }

        private void SaveModel(string path)
        {
            try
            {
                using (var writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.WriteLine("# OBJ Export from MainForm");
                    writer.WriteLine();

                    foreach (var vertex in vertexList)
                    {
                        writer.WriteLine("v " + vertex.X.ToString("F6") + " " +
                                                  vertex.Y.ToString("F6") + " " +
                                                  vertex.Z.ToString("F6"));
                    }
                    writer.WriteLine();

                    foreach (var face in faceList)
                    {
                        if (face.v1 >= 0 && face.v2 >= 0 && face.v3 >= 0 &&
                            face.v1 < vertexList.Count && face.v2 < vertexList.Count && face.v3 < vertexList.Count)
                        {
                            writer.WriteLine($"f {face.v1 + 1} {face.v2 + 1} {face.v3 + 1}");
                        }
                    }
                }

                logWriter.WriteLine("OBJ сохранен: " + path);
                logWriter.WriteLine("   Вершин: " + vertexList.Count + ", Граней: " + faceList.Count);
                MessageBox.Show("Модель сохранена как OBJ!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                logWriter.WriteLine("Ошибка сохранения: " + ex.Message);
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadModel(string path)
        {
            vertexList.Clear();
            indexList.Clear();
            edgeList.Clear();
            faceList.Clear();

            var context = new AssimpContext();
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);

            if (scene.Meshes.Count > 0)
            {
                var mesh = scene.Meshes[0];

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    var v = mesh.Vertices[i];
                    vertexList.Add(new Vector3(v.X, v.Y, v.Z));
                }

                float maxCoord = 0;
                foreach (var v in vertexList)
                {
                    float len = v.Length;
                    if (len > maxCoord) maxCoord = len;
                }
                float scale = 1.0f / (maxCoord * 1.5f);
                for (int i = 0; i < vertexList.Count; i++)
                {
                    vertexList[i] = vertexList[i] * scale;
                }

                logWriter.WriteLine($"Масштаб модели: {scale}, maxCoord: {maxCoord}");
                Log($"LoadModel: vertexList.Count после загрузки вершин = {vertexList.Count}");

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    logWriter.WriteLine($"Face {i}: IndexCount = {face.IndexCount}");
                }

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    if (face.IndexCount >= 3)
                    {
                        Face newFace = new Face
                        {
                            v1 = (int)face.Indices[0],
                            v2 = (int)face.Indices[1],
                            v3 = face.IndexCount > 2 ? (int)face.Indices[2] : (int)face.Indices[1]
                        };
                        faceList.Add(newFace);
                    }
                }

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    if (face.IndexCount == 3)
                    {
                        indexList.Add((uint)face.Indices[0]);
                        indexList.Add((uint)face.Indices[1]);
                        indexList.Add((uint)face.Indices[2]);
                    }
                }

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    if (face.IndexCount == 3)
                    {
                        uint v1 = (uint)face.Indices[0];
                        uint v2 = (uint)face.Indices[1];
                        uint v3 = (uint)face.Indices[2];
                        edgeList.Add(v1); edgeList.Add(v2);
                        edgeList.Add(v2); edgeList.Add(v3);
                        edgeList.Add(v3); edgeList.Add(v1);
                    }
                }
            }

            Log($"LoadModel: vertexList.Count = {vertexList.Count}");
            Log($"LoadModel: faceList.Count = {faceList.Count}");
            Log($"LoadModel: indexList.Count = {indexList.Count}");
            Log($"LoadModel: 3 * faceList.Count = {3 * faceList.Count}");
            Log($"LoadModel: edgeList.Count = {edgeList.Count}");
        }

        private void SetupBuffers()
        {
            if (edgesVao != 0) GL.DeleteVertexArray(edgesVao);
            if (pointsVao != 0) GL.DeleteVertexArray(pointsVao);
            if (vboEdges != 0) GL.DeleteBuffer(vboEdges);
            if (vboVerticesPoints != 0) GL.DeleteBuffer(vboVerticesPoints);

            ebo = GL.GenBuffer();
            vboEdges = GL.GenBuffer();
            vboVerticesPoints = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexList.Count * 3 * sizeof(float), ConvertVerticesToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexList.Count * sizeof(uint), indexList.ToArray(), BufferUsageHint.StaticDraw);

            edgesVao = GL.GenVertexArray();
            GL.BindVertexArray(edgesVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboEdges);
            GL.BufferData(BufferTarget.ArrayBuffer, edgeList.Count * 3 * sizeof(float), ConvertEdgesToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);  // stride=0!
            GL.EnableVertexAttribArray(0);

            pointsVao = GL.GenVertexArray();
            GL.BindVertexArray(pointsVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVerticesPoints);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexList.Count * 3 * sizeof(float), ConvertVerticesToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);  // stride=0!
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            uint maxIndex = 0;
            if (indexList.Count > 0)
            {
                maxIndex = indexList.Max();
            }

            BuildGrid();
            SetupGridBuffers();
        }

        private float[] ConvertVerticesToArray(List<Vector3> vertices)
        {
            float[] arr = new float[vertices.Count * 3];
            for (int i = 0; i < vertices.Count; i++)
            {
                arr[i * 3] = vertices[i].X;
                arr[i * 3 + 1] = vertices[i].Y;
                arr[i * 3 + 2] = vertices[i].Z;
            }
            return arr;
        }

        private void SetupGridBuffers()
        {
            if (gridVao != 0) GL.DeleteVertexArray(gridVao);
            if (gridVbo != 0) GL.DeleteBuffer(gridVbo);

            gridVao = GL.GenVertexArray();
            gridVbo = GL.GenBuffer();

            GL.BindVertexArray(gridVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, gridVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, gridLines.Count * 3 * sizeof(float), ConvertVerticesToArray(gridLines), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            Log($"SetupGridBuffers: gridLines.Count = {gridLines.Count}");
        }

        private void BuildGrid()
        {
            gridLines.Clear();

            // размер сетки (в мировых координатах)
            const float size = 3f;
            const int divisions = 10;

            float step = size * 2f / divisions;

            for (int i = 0; i <= divisions; i++)
            {
                float t = -size + i * step;

                // линия XZ (серые)
                gridLines.Add(new Vector3(-size, 0, t));
                gridLines.Add(new Vector3(+size, 0, t));

                gridLines.Add(new Vector3(t, 0, -size));
                gridLines.Add(new Vector3(t, 0, +size));
            }

            // оси X, Y, Z (центрированы, вверху/внизу/вдоль оси Z)
            float axisLen = 1.5f;

            // X (красная) — лежит в плоскости X, немного выше гридa
            gridLines.Add(new Vector3(-axisLen, 0.05f, 0));
            gridLines.Add(new Vector3(+axisLen, 0.05f, 0));

            // Y (зелёная)
            gridLines.Add(new Vector3(0, -axisLen, 0.05f));
            gridLines.Add(new Vector3(0, +axisLen, 0.05f));

            // Z (синяя)
            gridLines.Add(new Vector3(0.05f, 0.05f, -axisLen));
            gridLines.Add(new Vector3(0.05f, 0.05f, +axisLen));
        }

        private float[] ConvertEdgesToArray()
        {
            float[] arr = new float[edgeList.Count * 3];
            for (int i = 0; i < edgeList.Count; i++)
            {
                int idx = (int)edgeList[i];
                arr[i * 3] = vertexList[idx].X;
                arr[i * 3 + 1] = vertexList[idx].Y;
                arr[i * 3 + 2] = vertexList[idx].Z;
            }
            return arr;
        }

        private float[] ConvertVerticesToArray()
        {
            float[] arr = new float[vertexList.Count * 3];
            for (int i = 0; i < vertexList.Count; i++)
            {
                arr[i * 3] = vertexList[i].X;
                arr[i * 3 + 1] = vertexList[i].Y;
                arr[i * 3 + 2] = vertexList[i].Z;
            }
            return arr;
        }

        private void GlControl_Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) * Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)glControl.Width / glControl.Height, 0.1f, 10000f);
            var model = Matrix4.CreateRotationX(modelAngleX) * Matrix4.CreateRotationY(modelAngleY);

            GL.UseProgram(shaderProgram);
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int colorLoc = GL.GetUniformLocation(shaderProgram, "drawColor");

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.Uniform4(colorLoc, 1.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indexList.Count, DrawElementsType.UnsignedInt, 0);

            ErrorCode error;
            while ((error = GL.GetError()) != ErrorCode.NoError)
            {
                Log($"OpenGL Error: {error}");
            }

            // рисуем ориентиры (оси + сетка)
            GL.Disable(EnableCap.DepthTest);
            GL.LineWidth(1.2f);
            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            // ось X — красная
            GL.Uniform4(colorLoc, 1.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(gridVao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);   // первые 2 точки — X

            // ось Y — зелёная
            GL.Uniform4(colorLoc, 0.0f, 1.0f, 0.0f, 1.0f);
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);   // следующие 2 точки — Y

            // ось Z — синяя
            GL.Uniform4(colorLoc, 0.0f, 0.0f, 1.0f, 1.0f);
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);   // следующие 2 точки — Z

            // сетка XZ — серая
            GL.Uniform4(colorLoc, 0.5f, 0.5f, 0.5f, 1.0f);
            GL.DrawArrays(PrimitiveType.Lines, 6, gridLines.Count - 6);   // остальное

            GL.Enable(EnableCap.DepthTest);

            if (checkBoxShowEdges.Checked && edgeList.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.LineWidth(4.0f);
                GL.UniformMatrix4(modelLoc, false, ref model);
                GL.UniformMatrix4(viewLoc, false, ref view);
                GL.UniformMatrix4(projLoc, false, ref projection);
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 0.0f, 1.0f);
                GL.BindVertexArray(edgesVao);
                GL.DrawArrays(PrimitiveType.Lines, 0, edgeList.Count);
                GL.Enable(EnableCap.DepthTest);
            }

            if (checkBoxShowVertices.Checked && vertexList.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.PointSize(12.0f);
                GL.UniformMatrix4(modelLoc, false, ref model);
                GL.UniformMatrix4(viewLoc, false, ref view);
                GL.UniformMatrix4(projLoc, false, ref projection);
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 1.0f, 1.0f);
                GL.BindVertexArray(pointsVao);
                GL.DrawArrays(PrimitiveType.Points, 0, vertexList.Count);
                GL.Enable(EnableCap.DepthTest);
            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);
            glControl.SwapBuffers();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBoxModeEdit.Checked && e.Button == MouseButtons.Left)
            {
                if (checkBoxShowEdges.Checked && selectedFaceIndex != -1)
                {
                    Vector3 newWorldPos = ScreenToWorld(e.X, e.Y);

                    newWorldPos = ProjectPointToFacePlane(newWorldPos, faceList[selectedFaceIndex]);

                    var face = faceList[selectedFaceIndex];

                    Vector3 currentCenter = (vertexList[face.v1] + vertexList[face.v2] + vertexList[face.v3]) / 3f;
                    Vector3 offset = newWorldPos - currentCenter;

                    vertexList[face.v1] += offset;
                    vertexList[face.v2] += offset;
                    vertexList[face.v3] += offset;

                    UpdateAllBuffers();
                }
                else if (checkBoxShowVertices.Checked && selectedVertexIndex != -1)
                {
                    Vector3 newWorldPos = ScreenToWorld(e.X, e.Y);
                    vertexList[selectedVertexIndex] = newWorldPos;
                    UpdateAllBuffers();
                }
            }
            else
            {
                float deltaX = e.X - lastMousePos.X;
                float deltaY = e.Y - lastMousePos.Y;
                if (e.Button == MouseButtons.Left)
                {
                    angleY += deltaX * 0.01f;
                    angleX -= deltaY * 0.01f;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    modelAngleY += deltaX * 0.01f;
                    modelAngleX -= deltaY * 0.01f;
                }
            }

            lastMousePos = e.Location;
            glControl.Invalidate();
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;

            if (checkBoxModeEdit.Checked)
            {
                selectedVertexIndex = -1;
                selectedFaceIndex = -1;

                if (checkBoxAdd.Checked)
                {
                    AddNewVertexAtMousePosition(e.Location);
                    SetupBuffers();
                    glControl.Invalidate();
                    return;
                }

                if (checkBoxShowEdges.Checked)
                {
                    selectedFaceIndex = FindClosestFace(e.Location);
                }
                else if (checkBoxShowVertices.Checked)
                {
                    selectedVertexIndex = FindClosestVertex(e.Location);
                }
            }
            glControl.Invalidate();
        }

        private void AddNewVertexAtMousePosition(Point mousePos)
        {
            int closestFaceIndex = FindClosestFace(mousePos);
            if (closestFaceIndex == -1)
            {
                logWriter.WriteLine("Нет ближайшей грани");
                return;
            }

            var baseFace = faceList[closestFaceIndex];
            Vector3 baseCenter = (vertexList[baseFace.v1] + vertexList[baseFace.v2] + vertexList[baseFace.v3]) / 3f;

            Vector3 clickWorldPos = ScreenToWorld(mousePos.X, mousePos.Y);
            Vector3 projectedPos = ProjectPointToFacePlane(clickWorldPos, baseFace);

            float radius = 0.1f;

            int v1Index = vertexList.Count;
            int v2Index = v1Index + 1;
            int v3Index = v1Index + 2;

            Vector3 edge1 = vertexList[baseFace.v2] - vertexList[baseFace.v1];
            Vector3 edge2 = vertexList[baseFace.v3] - vertexList[baseFace.v1];
            Vector3 tangent1 = Vector3.Normalize(edge1);
            Vector3 tangent2 = Vector3.Normalize(Vector3.Cross(edge1, Vector3.Cross(edge1, edge2)));

            float angleStep = MathHelper.TwoPi / 3f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = tangent1 * (float)Math.Cos(angle) * radius +
                                tangent2 * (float)Math.Sin(angle) * radius;
                Vector3 newVertex = projectedPos + offset;
                vertexList.Add(newVertex);
            }

            faceList.Add(new Face { v1 = v1Index, v2 = v2Index, v3 = v3Index });

            UpdateIndicesAndEdges();

            logWriter.WriteLine("Новая грань #" + (faceList.Count - 1) +
                               " (вершины " + v1Index + "-" + v2Index + "-" + v3Index + ")");
        }

        private Vector3 ProjectPointToFacePlane(Vector3 point, Face face)
        {
            Vector3 v1 = vertexList[face.v1];
            Vector3 v2 = vertexList[face.v2];
            Vector3 v3 = vertexList[face.v3];

            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;
            Vector3 normal = Vector3.Cross(edge1, edge2);
            normal = Vector3.Normalize(normal);

            float distance = Vector3.Dot(v1, normal);

            float t = Vector3.Dot(point - v1, normal);
            return point - (t * normal);
        }

        private void UpdateIndicesAndEdges()
        {
            indexList.Clear();
            edgeList.Clear();

            Log($"UpdateIndicesAndEdges: BEFORE loop faceList.Count = {faceList.Count}");

            foreach (var face in faceList)
            {
                if (face.v1 >= 0 && face.v2 >= 0 && face.v3 >= 0 &&
                    face.v1 < vertexList.Count && face.v2 < vertexList.Count && face.v3 < vertexList.Count)
                {
                    indexList.Add((uint)face.v1);
                    indexList.Add((uint)face.v2);
                    indexList.Add((uint)face.v3);

                    edgeList.Add((uint)face.v1); edgeList.Add((uint)face.v2);
                    edgeList.Add((uint)face.v2); edgeList.Add((uint)face.v3);
                    edgeList.Add((uint)face.v3); edgeList.Add((uint)face.v1);
                }
            }

            Log($"UpdateIndicesAndEdges: AFTER loop faceList.Count = {faceList.Count}");
            Log($"UpdateIndicesAndEdges: indexList.Count = {indexList.Count}");
            Log($"UpdateIndicesAndEdges: edgeList.Count = {edgeList.Count}");
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog
            {
                Filter = "OBJ files (*.obj)|*.obj",
                DefaultExt = "obj"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SaveModel(dlg.FileName);
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/Rediska4445/3dEditor-";
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть браузер: " + ex.Message);
            }
        }

        private int FindClosestVertex(Point mousePos)
        {
            const float threshold = 10f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < vertexList.Count; i++)
            {
                Vector3 screenPos = ProjectToScreen(vertexList[i]);
                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                logWriter.WriteLine($"Vertex {i}: screenX={screenPos.X}, screenY={screenPos.Y}, mouseX={mousePos.X}, mouseY={mousePos.Y}, dist={dist}");
                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }

            logWriter.WriteLine($"Closest vertex index: {closestIndex} (минимальное расстояние: {minDist})");
            return closestIndex;
        }

        private int FindClosestFace(Point mousePos)
        {
            const float threshold = 25f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < faceList.Count; i++)
            {
                var face = faceList[i];
                Vector3 center = (vertexList[face.v1] + vertexList[face.v2] + vertexList[face.v3]) / 3f;
                Vector3 screenPos = ProjectToScreen(center);

                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            logWriter.WriteLine($"Selected face: {closestIndex}");
            return closestIndex;
        }

        private void UpdateAllBuffers()
        {
            Log($"UpdateAllBuffers: vertexList.Count = {vertexList.Count}");
            Log($"UpdateAllBuffers: faceList.Count = {faceList.Count}");
            Log($"UpdateAllBuffers: indexList.Count = {indexList.Count}");
            Log($"UpdateAllBuffers: edgeList.Count = {edgeList.Count}");

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexList.Count * 3 * sizeof(float),
                          ConvertVerticesToArray(), BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexList.Count * sizeof(uint),
                          indexList.ToArray(), BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboEdges);
            GL.BufferData(BufferTarget.ArrayBuffer, edgeList.Count * 3 * sizeof(float),
                          ConvertEdgesToArray(), BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVerticesPoints);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexList.Count * 3 * sizeof(float),
                          ConvertVerticesToArray(), BufferUsageHint.DynamicDraw);
        }

        private Vector3 ProjectToScreen(Vector3 worldPos)
        {
            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) *
                       Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                       (float)glControl.Width / glControl.Height, 0.1f, 100f);
            var model = Matrix4.CreateRotationX(modelAngleX) * Matrix4.CreateRotationY(modelAngleY);

            Vector4 clipSpacePos = Vector4.Transform(new Vector4(worldPos, 1.0f), model * view * projection);
            Vector3 ndc = new Vector3(clipSpacePos.X / clipSpacePos.W, clipSpacePos.Y / clipSpacePos.W, clipSpacePos.Z / clipSpacePos.W);
            float screenX = (ndc.X + 1) * 0.5f * glControl.Width;
            float screenY = (1 - ndc.Y) * 0.5f * glControl.Height;
            return new Vector3(screenX, screenY, ndc.Z);
        }

        private Vector3 ScreenToWorld(int mouseX, int mouseY)
        {
            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) *
                       Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                       (float)glControl.Width / glControl.Height, 0.1f, 100f);
            var model = Matrix4.CreateRotationX(modelAngleX) * Matrix4.CreateRotationY(modelAngleY);

            float x = (2.0f * mouseX) / glControl.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / glControl.Height;

            Vector4 rayStartNdc = new Vector4(x, y, -1.0f, 1.0f);
            Vector4 rayEndNdc = new Vector4(x, y, 1.0f, 1.0f);

            Matrix4 invModelViewProj = Matrix4.Invert(model * view * projection);

            Vector4 rayStartWorld = Vector4.Transform(rayStartNdc, invModelViewProj);
            Vector4 rayEndWorld = Vector4.Transform(rayEndNdc, invModelViewProj);

            rayStartWorld /= rayStartWorld.W;
            rayEndWorld /= rayEndWorld.W;

            Vector3 dir = Vector3.Normalize(rayEndWorld.Xyz - rayStartWorld.Xyz);
            float dirY = dir.Y;

            if (Math.Abs(dirY) < 1e-6f)
            {
                Log("ScreenToWorld: dir.Y ≈ 0 - возвращаем точку вдоль луча");
                return rayStartWorld.Xyz + dir * 5f;  
            }

            float t = -rayStartWorld.Y / dirY;
            Vector3 worldPoint = rayStartWorld.Xyz + dir * t;
            return worldPoint;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            cameraDistance -= e.Delta * 0.01f;
            if (cameraDistance < 1f) cameraDistance = 1f;
            if (cameraDistance > 20f) cameraDistance = 20f;
            glControl.Invalidate();
        }
    }
}