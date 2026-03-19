using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        private Matrix4 viewMatrix;
        private Matrix4 projectionMatrix;

        private int selectedVertexIndex = -1;

        private int vboEdges, vboVerticesPoints;
        private List<uint> edgeList = new List<uint>();

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
                    SetupBuffers();
                    glControl.Invalidate();
                }
            }
        }

        private void LoadModel(string path)
        {
            vertexList.Clear();
            indexList.Clear();
            edgeList.Clear();

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

            logWriter.WriteLine($"Загружено: вершин={vertexList.Count}, граней={indexList.Count / 3}, рёбер={edgeList.Count}");
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
            logWriter.WriteLine("SetupBuffers завершён успешно");
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
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)glControl.Width / glControl.Height, 0.1f, 100f);
            var model = Matrix4.CreateRotationX(modelAngleX) * Matrix4.CreateRotationY(modelAngleY);

            GL.UseProgram(shaderProgram);
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int colorLoc = GL.GetUniformLocation(shaderProgram, "drawColor");  // ← НОВЫЙ!

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.Uniform4(colorLoc, 1.0f, 0.0f, 0.0f, 1.0f);  // КРАСНЫЙ
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexList.Count, DrawElementsType.UnsignedInt, 0);

            if (checkBoxShowEdges.Checked && edgeList.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.LineWidth(4.0f);
                GL.UniformMatrix4(modelLoc, false, ref model);
                GL.UniformMatrix4(viewLoc, false, ref view);
                GL.UniformMatrix4(projLoc, false, ref projection);
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 0.0f, 1.0f);  // ЖЁЛТЫЙ!
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
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 1.0f, 1.0f);  // БЕЛЫЙ!
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
            if (checkBoxModeEdit.Checked && selectedVertexIndex != -1 && e.Button == MouseButtons.Left)
            {
                Vector3 newWorldPos = ScreenToWorld(e.X, e.Y);
                vertexList[selectedVertexIndex] = newWorldPos;

                GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexList.Count * 3 * sizeof(float), ConvertVerticesToArray(), BufferUsageHint.DynamicDraw);

                glControl.Invalidate();
            }
            else
            {
                float deltaX = e.X - lastMousePos.X;
                float deltaY = e.Y - lastMousePos.Y;

                if (e.Button == MouseButtons.Left)
                {
                    angleY += deltaX * 0.01f;
                    angleX += deltaY * 0.01f;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    modelAngleY += deltaX * 0.01f;
                    modelAngleX += deltaY * 0.01f;
                }

                lastMousePos = e.Location;
                glControl.Invalidate();
            }

            lastMousePos = e.Location;
            glControl.Invalidate();
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;

            if (checkBoxModeEdit.Checked)
            {
                selectedVertexIndex = FindClosestVertex(e.Location);
            }

            glControl.Invalidate();
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

        private Vector3 ProjectToScreen(Vector3 worldPos)
        {
            Vector4 clipSpacePos = Vector4.Transform(new Vector4(worldPos, 1.0f), viewMatrix * projectionMatrix);
            Vector3 ndc = new Vector3(clipSpacePos.X / clipSpacePos.W, clipSpacePos.Y / clipSpacePos.W, clipSpacePos.Z / clipSpacePos.W);
            float screenX = (ndc.X + 1) * 0.5f * glControl.Width;
            float screenY = (1 - ndc.Y) * 0.5f * glControl.Height;
            return new Vector3(screenX, screenY, ndc.Z);
        }

        private Vector3 ScreenToWorld(int mouseX, int mouseY)
        {
            float x = (2.0f * mouseX) / glControl.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / glControl.Height;

            Vector4 rayStartNdc = new Vector4(x, y, -1.0f, 1.0f);
            Vector4 rayEndNdc = new Vector4(x, y, 1.0f, 1.0f);

            Matrix4 invViewProj = Matrix4.Invert(viewMatrix * projectionMatrix);

            Vector4 rayStartWorld = Vector4.Transform(rayStartNdc, invViewProj);
            Vector4 rayEndWorld = Vector4.Transform(rayEndNdc, invViewProj);

            rayStartWorld /= rayStartWorld.W;
            rayEndWorld /= rayEndWorld.W;

            Vector3 dir = Vector3.Normalize(new Vector3(rayEndWorld.X - rayStartWorld.X,
                                                           rayEndWorld.Y - rayStartWorld.Y,
                                                           rayEndWorld.Z - rayStartWorld.Z));

            float t = -rayStartWorld.Y / dir.Y;
            Vector3 worldPoint = new Vector3(rayStartWorld.X + dir.X * t,
                                             0,
                                             rayStartWorld.Z + dir.Z * t);
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