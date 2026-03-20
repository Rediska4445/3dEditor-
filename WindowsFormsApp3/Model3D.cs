using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace WindowsFormsApp3
{
    public class Model3D
    {
        public List<Vector3> VertexList { get; private set; } = new List<Vector3>();
        public List<Vector3> NormalList { get; private set; } = new List<Vector3>();
        public List<Face> FaceList { get; private set; } = new List<Face>();
        public List<uint> IndexList { get; private set; } = new List<uint>();
        public List<uint> EdgeList { get; private set; } = new List<uint>();

        public int Vao { get; private set; } = 0;
        public int VboVertices { get; private set; } = 0;
        public int Ebo { get; private set; } = 0;
        public int EdgesVao { get; private set; } = 0;
        public int PointsVao { get; private set; } = 0;
        public int VboEdges { get; private set; } = 0;
        public int VboVerticesPoints { get; private set; } = 0;

        public int shaderProgram;
        public float ModelAngleX { get; set; } = 0f;
        public float ModelAngleY { get; set; } = 0f;
        public int SelectedVertexIndex { get; set; } = -1;
        public int SelectedFaceIndex { get; set; } = -1;

        public struct Face
        {
            public int v1, v2, v3;
        }

        public Model3D()
        {
            SetupShaders();
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
                throw new Exception($"Shader compile error: {info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"Program link error: {info}");
            }
        }

        public void LoadFromAssimpMesh(Assimp.Mesh mesh)
        {
            VertexList.Clear();
            NormalList.Clear();
            FaceList.Clear();
            IndexList.Clear();
            EdgeList.Clear();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                VertexList.Add(new Vector3(v.X, v.Y, v.Z));
            }

            if (mesh.HasNormals)
            {
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    var n = mesh.Normals[i];
                    NormalList.Add(new Vector3(n.X, n.Y, n.Z));
                }
            }
            else
            {
                for (int i = 0; i < VertexList.Count; i++)
                    NormalList.Add(Vector3.UnitY);
            }

            float maxCoord = VertexList.Max(v => v.Length);
            float scale = 1.0f / (maxCoord * 1.5f);
            for (int i = 0; i < VertexList.Count; i++)
            {
                VertexList[i] *= scale;
                if (NormalList.Count > i) NormalList[i] = Vector3.Normalize(NormalList[i]);
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
                        v3 = (int)face.Indices[2]
                    };
                    FaceList.Add(newFace);
                }
            }

            UpdateIndicesAndEdges();
            BuildBuffers();
        }

        public void BuildBuffers()
        {
            CleanupBuffers();
            SetupMainBuffers();
            SetupEdgesBuffer();
            SetupPointsBuffer();
        }

        private void SetupMainBuffers()
        {
            Vao = GL.GenVertexArray();
            VboVertices = GL.GenBuffer();
            Ebo = GL.GenBuffer();

            GL.BindVertexArray(Vao);

            // Вершины + нормали
            float[] vertexArray = new float[VertexList.Count * 6];
            for (int i = 0; i < VertexList.Count; i++)
            {
                vertexArray[i * 6 + 0] = VertexList[i].X;
                vertexArray[i * 6 + 1] = VertexList[i].Y;
                vertexArray[i * 6 + 2] = VertexList[i].Z;
                vertexArray[i * 6 + 3] = NormalList[i].X;
                vertexArray[i * 6 + 4] = NormalList[i].Y;
                vertexArray[i * 6 + 5] = NormalList[i].Z;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexList.Count * sizeof(uint), IndexList.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public int DeleteFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection, int width, int height)
        {
            int closestFace = FindClosestFace(mousePos, view, projection, width, height);
            if (closestFace == -1) return -1;

            FaceList.RemoveAt(closestFace);

            RemoveOrphanedVertices();

            UpdateIndicesAndEdges();
            UpdateAllBuffers();

            return closestFace;
        }

        private void RemoveOrphanedVertices()
        {
            var usedVertices = new HashSet<int>();
            foreach (var face in FaceList)
            {
                if (face.v1 >= 0 && face.v1 < VertexList.Count) usedVertices.Add(face.v1);
                if (face.v2 >= 0 && face.v2 < VertexList.Count) usedVertices.Add(face.v2);
                if (face.v3 >= 0 && face.v3 < VertexList.Count) usedVertices.Add(face.v3);
            }

            var newVertexList = new List<Vector3>();
            var newNormalList = new List<Vector3>();
            var vertexMap = new Dictionary<int, int>();

            for (int i = 0; i < VertexList.Count; i++)
            {
                if (usedVertices.Contains(i))
                {
                    int newIndex = newVertexList.Count;
                    vertexMap[i] = newIndex;
                    newVertexList.Add(VertexList[i]);
                    newNormalList.Add(NormalList.Count > i ? NormalList[i] : Vector3.UnitY);
                }
            }

            VertexList = newVertexList;
            NormalList = newNormalList;

            for (int i = 0; i < FaceList.Count; i++)
            {
                var face = FaceList[i];
                face.v1 = vertexMap.ContainsKey(face.v1) ? vertexMap[face.v1] : 0;
                face.v2 = vertexMap.ContainsKey(face.v2) ? vertexMap[face.v2] : 0;
                face.v3 = vertexMap.ContainsKey(face.v3) ? vertexMap[face.v3] : 0;
                FaceList[i] = face; 
            }
        }

        private void SetupEdgesBuffer()
        {
            EdgesVao = GL.GenVertexArray();
            VboEdges = GL.GenBuffer();

            GL.BindVertexArray(EdgesVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboEdges);
            GL.BufferData(BufferTarget.ArrayBuffer, EdgeList.Count * 3 * sizeof(float), ConvertEdgesToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void SetupPointsBuffer()
        {
            PointsVao = GL.GenVertexArray();
            VboVerticesPoints = GL.GenBuffer();

            GL.BindVertexArray(PointsVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVerticesPoints);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexList.Count * 3 * sizeof(float), ConvertVerticesToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        // ✅ ПОЛНЫЙ РЕНДЕР в Model3D
        public void Render(Matrix4 view, Matrix4 projection, bool showEdges, bool showVertices, SimpleLighting lighting)
        {
            var modelMatrix = Matrix4.CreateRotationX(ModelAngleX) * Matrix4.CreateRotationY(ModelAngleY);

            // Основная модель с освещением
            lighting.Use(modelMatrix, view, projection, new Vector3(1.0f, 0.8f, 0.5f));
            GL.BindVertexArray(Vao);
            GL.DrawElements(PrimitiveType.Triangles, IndexList.Count, DrawElementsType.UnsignedInt, 0);
            lighting.Unbind();

            // Wireframe overlay
            GL.UseProgram(shaderProgram);
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int colorLoc = GL.GetUniformLocation(shaderProgram, "drawColor");

            GL.UniformMatrix4(modelLoc, false, ref modelMatrix);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            if (showEdges && EdgeList.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.LineWidth(4.0f);
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 0.0f, 1.0f);
                GL.BindVertexArray(EdgesVao);
                GL.DrawArrays(PrimitiveType.Lines, 0, EdgeList.Count);
                GL.Enable(EnableCap.DepthTest);
            }

            if (showVertices && VertexList.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.PointSize(12.0f);
                GL.Uniform4(colorLoc, 1.0f, 1.0f, 1.0f, 1.0f);
                GL.BindVertexArray(PointsVao);
                GL.DrawArrays(PrimitiveType.Points, 0, VertexList.Count);
                GL.Enable(EnableCap.DepthTest);
            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        public void UpdateAllBuffers()
        {
            UpdateMainBuffers();
            UpdateEdgesBuffer();
            UpdatePointsBuffer();
        }

        private void UpdateMainBuffers()
        {
            if (Vao == 0) return;
            GL.BindVertexArray(Vao);

            float[] vertexArray = new float[VertexList.Count * 6];
            for (int i = 0; i < VertexList.Count; i++)
            {
                vertexArray[i * 6 + 0] = VertexList[i].X;
                vertexArray[i * 6 + 1] = VertexList[i].Y;
                vertexArray[i * 6 + 2] = VertexList[i].Z;
                vertexArray[i * 6 + 3] = NormalList.Count > i ? NormalList[i].X : 0;
                vertexArray[i * 6 + 4] = NormalList.Count > i ? NormalList[i].Y : 1;
                vertexArray[i * 6 + 5] = NormalList.Count > i ? NormalList[i].Z : 0;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexList.Count * sizeof(uint), IndexList.ToArray(), BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(0);
        }

        private void UpdateEdgesBuffer()
        {
            if (EdgesVao == 0) return;
            GL.BindVertexArray(EdgesVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboEdges);
            GL.BufferData(BufferTarget.ArrayBuffer, EdgeList.Count * 3 * sizeof(float), ConvertEdgesToArray(), BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(0);
        }

        private void UpdatePointsBuffer()
        {
            if (PointsVao == 0) return;
            GL.BindVertexArray(PointsVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVerticesPoints);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexList.Count * 3 * sizeof(float), ConvertVerticesToArray(), BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(0);
        }

        public void UpdateIndicesAndEdges()
        {
            IndexList.Clear();
            EdgeList.Clear();

            foreach (var face in FaceList)
            {
                if (face.v1 >= 0 && face.v2 >= 0 && face.v3 >= 0 &&
                    face.v1 < VertexList.Count && face.v2 < VertexList.Count && face.v3 < VertexList.Count)
                {
                    IndexList.Add((uint)face.v1);
                    IndexList.Add((uint)face.v2);
                    IndexList.Add((uint)face.v3);

                    EdgeList.Add((uint)face.v1); EdgeList.Add((uint)face.v2);
                    EdgeList.Add((uint)face.v2); EdgeList.Add((uint)face.v3);
                    EdgeList.Add((uint)face.v3); EdgeList.Add((uint)face.v1);
                }
            }
        }

        public void AddNewVertexAtPosition(Vector3 projectedPos, int baseFaceIndex, float radius = 0.1f)
        {
            if (baseFaceIndex < 0 || baseFaceIndex >= FaceList.Count) return;

            var baseFace = FaceList[baseFaceIndex];
            Vector3 edge1 = VertexList[baseFace.v2] - VertexList[baseFace.v1];
            Vector3 edge2 = VertexList[baseFace.v3] - VertexList[baseFace.v1];
            Vector3 tangent1 = Vector3.Normalize(edge1);
            Vector3 tangent2 = Vector3.Normalize(Vector3.Cross(edge1, Vector3.Cross(edge1, edge2)));

            int v1Index = VertexList.Count;
            int v2Index = v1Index + 1;
            int v3Index = v1Index + 2;

            float angleStep = OpenTK.MathHelper.TwoPi / 3f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = tangent1 * (float)Math.Cos(angle) * radius +
                                tangent2 * (float)Math.Sin(angle) * radius;
                Vector3 newVertex = projectedPos + offset;
                VertexList.Add(newVertex);
                NormalList.Add(Vector3.UnitY);
            }

            FaceList.Add(new Face { v1 = v1Index, v2 = v2Index, v3 = v3Index });
            UpdateIndicesAndEdges();
            UpdateAllBuffers();
        }

        public int FindClosestVertex(Vector3[] screenPositions, Point mousePos)
        {
            const float threshold = 10f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < VertexList.Count && i < screenPositions.Length; i++)
            {
                float dx = screenPositions[i].X - mousePos.X;
                float dy = screenPositions[i].Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int FindClosestFace(Vector3[] screenCenters, Point mousePos)
        {
            const float threshold = 25f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < FaceList.Count && i < screenCenters.Length; i++)
            {
                Vector3 center = (VertexList[FaceList[i].v1] + VertexList[FaceList[i].v2] + VertexList[FaceList[i].v3]) / 3f;
                float dx = screenCenters[i].X - mousePos.X;
                float dy = screenCenters[i].Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public void MoveSelectedFace(Vector3 offset)
        {
            if (SelectedFaceIndex == -1) return;
            var face = FaceList[SelectedFaceIndex];
            if (face.v1 < VertexList.Count) VertexList[face.v1] += offset;
            if (face.v2 < VertexList.Count) VertexList[face.v2] += offset;
            if (face.v3 < VertexList.Count) VertexList[face.v3] += offset;
            UpdateAllBuffers();
        }

        public void MoveSelectedVertex(Vector3 newPosition)
        {
            if (SelectedVertexIndex >= 0 && SelectedVertexIndex < VertexList.Count)
            {
                VertexList[SelectedVertexIndex] = newPosition;
                UpdateAllBuffers();
            }
        }

        private float[] ConvertVerticesToArray()
        {
            float[] arr = new float[VertexList.Count * 3];
            for (int i = 0; i < VertexList.Count; i++)
            {
                arr[i * 3] = VertexList[i].X;
                arr[i * 3 + 1] = VertexList[i].Y;
                arr[i * 3 + 2] = VertexList[i].Z;
            }
            return arr;
        }

        private float[] ConvertEdgesToArray()
        {
            float[] arr = new float[EdgeList.Count * 3];
            for (int i = 0; i < EdgeList.Count; i++)
            {
                int idx = (int)EdgeList[i];
                if (idx < VertexList.Count)
                {
                    arr[i * 3] = VertexList[idx].X;
                    arr[i * 3 + 1] = VertexList[idx].Y;
                    arr[i * 3 + 2] = VertexList[idx].Z;
                }
            }
            return arr;
        }

        public void CleanupBuffers()
        {
            if (Vao != 0) GL.DeleteVertexArray(Vao);
            if (VboVertices != 0) GL.DeleteBuffer(VboVertices);
            if (Ebo != 0) GL.DeleteBuffer(Ebo);
            if (EdgesVao != 0) GL.DeleteVertexArray(EdgesVao);
            if (PointsVao != 0) GL.DeleteVertexArray(PointsVao);
            if (VboEdges != 0) GL.DeleteBuffer(VboEdges);
            if (VboVerticesPoints != 0) GL.DeleteBuffer(VboVerticesPoints);

            Vao = VboVertices = Ebo = EdgesVao = PointsVao = VboEdges = VboVerticesPoints = 0;
        }

        public void AddNewFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection,
                                             int controlWidth, int controlHeight, StreamWriter logWriter)
        {
            int closestFaceIndex = FindClosestFace(mousePos, view, projection, controlWidth, controlHeight);
            if (closestFaceIndex == -1)
            {
                logWriter?.WriteLine("Нет ближайшей грани");
                return;
            }

            var baseFace = FaceList[closestFaceIndex];
            Vector3 baseCenter = GetFaceCenter(closestFaceIndex);

            Vector3 clickWorldPos = ScreenToWorld(mousePos.X, mousePos.Y, view, projection, controlWidth, controlHeight);
            Vector3 projectedPos = ProjectPointToFacePlane(clickWorldPos, baseFace);

            float radius = 0.1f;
            int v1Index = VertexList.Count;
            int v2Index = v1Index + 1;
            int v3Index = v1Index + 2;

            Vector3 edge1 = VertexList[baseFace.v2] - VertexList[baseFace.v1];
            Vector3 edge2 = VertexList[baseFace.v3] - VertexList[baseFace.v1];
            Vector3 tangent1 = Vector3.Normalize(edge1);
            Vector3 tangent2 = Vector3.Normalize(Vector3.Cross(edge1, Vector3.Cross(edge1, edge2)));

            float angleStep = OpenTK.MathHelper.TwoPi / 3f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = tangent1 * (float)Math.Cos(angle) * radius +
                                tangent2 * (float)Math.Sin(angle) * radius;
                Vector3 newVertex = projectedPos + offset;
                VertexList.Add(newVertex);
                NormalList.Add(Vector3.UnitY);
            }

            FaceList.Add(new Face { v1 = v1Index, v2 = v2Index, v3 = v3Index });
            UpdateIndicesAndEdges();
            UpdateAllBuffers();

            logWriter?.WriteLine($"Новая грань #{FaceList.Count - 1} (вершины {v1Index}-{v2Index}-{v3Index})");
        }

        private Vector3 ProjectPointToFacePlane(Vector3 point, Face face)
        {
            Vector3 v1 = VertexList[face.v1];
            Vector3 v2 = VertexList[face.v2];
            Vector3 v3 = VertexList[face.v3];

            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;
            Vector3 normal = Vector3.Cross(edge1, edge2);
            normal = Vector3.Normalize(normal);

            float distance = Vector3.Dot(v1, normal);
            float t = Vector3.Dot(point - v1, normal);
            return point - (t * normal);
        }

        private Vector3 ScreenToWorld(int mouseX, int mouseY, Matrix4 view, Matrix4 projection, int width, int height)
        {
            float x = (2.0f * mouseX) / width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / height;

            Vector4 rayStartNdc = new Vector4(x, y, -1.0f, 1.0f);
            Vector4 rayEndNdc = new Vector4(x, y, 1.0f, 1.0f);

            Matrix4 modelMatrix = Matrix4.CreateRotationX(ModelAngleX) * Matrix4.CreateRotationY(ModelAngleY);
            Matrix4 invModelViewProj = Matrix4.Invert(modelMatrix * view * projection);

            Vector4 rayStartWorld = Vector4.Transform(rayStartNdc, invModelViewProj);
            Vector4 rayEndWorld = Vector4.Transform(rayEndNdc, invModelViewProj);

            rayStartWorld /= rayStartWorld.W;
            rayEndWorld /= rayEndWorld.W;

            Vector3 dir = Vector3.Normalize(rayEndWorld.Xyz - rayStartWorld.Xyz);
            float dirY = dir.Y;

            if (Math.Abs(dirY) < 1e-6f)
                return rayStartWorld.Xyz + dir * 5f;

            float t = -rayStartWorld.Y / dirY;
            return rayStartWorld.Xyz + dir * t;
        }

        public int FindClosestVertex(Point mousePos, Matrix4 view, Matrix4 projection, int controlWidth, int controlHeight)
        {
            const float threshold = 10f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < VertexList.Count; i++)
            {
                Vector3 screenPos = ProjectToScreen(VertexList[i], view, projection, controlWidth, controlHeight);
                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int FindClosestFace(Point mousePos, Matrix4 view, Matrix4 projection, int controlWidth, int controlHeight)
        {
            const float threshold = 25f;
            int closestIndex = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < FaceList.Count; i++)
            {
                var face = FaceList[i];
                Vector3 center = (VertexList[face.v1] + VertexList[face.v2] + VertexList[face.v3]) / 3f;
                Vector3 screenPos = ProjectToScreen(center, view, projection, controlWidth, controlHeight);
                float dx = screenPos.X - mousePos.X;
                float dy = screenPos.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist < threshold)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        private Vector3 ProjectToScreen(Vector3 worldPos, Matrix4 view, Matrix4 projection, int width, int height)
        {
            Matrix4 modelMatrix = Matrix4.CreateRotationX(ModelAngleX) * Matrix4.CreateRotationY(ModelAngleY);
            Vector4 clipSpacePos = Vector4.Transform(new Vector4(worldPos, 1.0f), modelMatrix * view * projection);

            Vector3 ndc = new Vector3(
                clipSpacePos.X / clipSpacePos.W,
                clipSpacePos.Y / clipSpacePos.W,
                clipSpacePos.Z / clipSpacePos.W
            );

            float screenX = (ndc.X * 0.5f + 0.5f) * width;
            float screenY = (1.0f - ndc.Y * 0.5f - 0.5f) * height;
            return new Vector3(screenX, screenY, ndc.Z);
        }

        public Vector3 GetFaceCenter(int faceIndex)
        {
            if (faceIndex < 0 || faceIndex >= FaceList.Count) return Vector3.Zero;
            var face = FaceList[faceIndex];
            return (VertexList[face.v1] + VertexList[face.v2] + VertexList[face.v3]) / 3f;
        }

        public void SaveToObj(string path)
        {
            try
            {
                using (var writer = new StreamWriter(path, false))
                {
                    writer.WriteLine("# OBJ Export from Model3D");
                    foreach (var vertex in VertexList)
                        writer.WriteLine($"v " + vertex.X + " " + vertex.Y + " " + vertex.Z);
                    writer.WriteLine();
                    foreach (var face in FaceList)
                        if (face.v1 >= 0 && face.v2 >= 0 && face.v3 >= 0 &&
                            face.v1 < VertexList.Count && face.v2 < VertexList.Count && face.v3 < VertexList.Count)
                            writer.WriteLine($"f {face.v1 + 1} {face.v2 + 1} {face.v3 + 1}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения OBJ: {ex.Message}");
            }
        }
    }
}
