using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace WindowsFormsApp3
{
    public class Model3D : IDisposable
    {
        public MeshData Mesh { get; }
        public GlMeshBuffers Buffers { get; }
        public SimpleColorShader EdgeVertexShader { get; }
        public ModelPicker Picker { get; }

        public float ModelAngleX { get; set; } = 0f;
        public float ModelAngleY { get; set; } = 0f;
        public int SelectedVertexIndex { get; set; } = -1;
        public int SelectedFaceIndex { get; set; } = -1;

        public Vector3 ModelPosition { get; set; } = Vector3.Zero;

        public Vector3 ModelScale { get; private set; } = Vector3.One;

        public System.Drawing.Color ModelColorRgb { get; private set; } = System.Drawing.Color.FromArgb(255, 204, 128);

        public Model3D()
        {
            Mesh = new MeshData();
            Buffers = new GlMeshBuffers();
            EdgeVertexShader = new SimpleColorShader();
            Picker = new ModelPicker(Mesh, () => GetModelMatrix());
        }

        public void SetScale(Vector3 scale)
        {
            ModelScale = scale;
        }

        public void SetUniformScale(float scale)
        {
            ModelScale = new Vector3(scale, scale, scale);
        }

        public void MultiplyScale(float factor)
        {
            ModelScale *= factor;
        }

        public void Translate(Vector3 offset)
        {
            ModelPosition += offset;
        }

        public void SetPosition(Vector3 pos)
        {
            ModelPosition = pos;
        }

        public void SetColorRgb(byte r, byte g, byte b)
        {
            ModelColorRgb = System.Drawing.Color.FromArgb(r, g, b);
        }

        private Vector3 GetModelColorVector()
        {
            return new Vector3(
                ModelColorRgb.R / 255f,
                ModelColorRgb.G / 255f,
                ModelColorRgb.B / 255f
            );
        }

        private Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(ModelScale) *
                   Matrix4.CreateRotationX(ModelAngleX) *
                   Matrix4.CreateRotationY(ModelAngleY) *
                   Matrix4.CreateTranslation(ModelPosition);
        }

        public void LoadFromAssimpMesh(Assimp.Mesh mesh)
        {
            Mesh.LoadFromAssimpMesh(mesh);
            Buffers.Build(Mesh);
        }

        public void UpdateAllBuffers()
        {
            Buffers.UpdateAll(Mesh);
        }

        public int DeleteFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection,
                                             int width, int height)
        {
            int closestFace = Picker.DeleteFaceAtMousePosition(mousePos, view, projection, width, height);
            if (closestFace != -1)
                Buffers.UpdateAll(Mesh);
            return closestFace;
        }

        public int FindClosestVertex(Point mousePos, Matrix4 view, Matrix4 projection,
                                     int controlWidth, int controlHeight)
        {
            return Picker.FindClosestVertex(mousePos, view, projection, controlWidth, controlHeight);
        }

        public int FindClosestFace(Point mousePos, Matrix4 view, Matrix4 projection,
                                   int controlWidth, int controlHeight)
        {
            return Picker.FindClosestFace(mousePos, view, projection, controlWidth, controlHeight);
        }

        public void AddNewFaceAtMousePosition(Point mousePos, Matrix4 view, Matrix4 projection,
                                              int controlWidth, int controlHeight,
                                              System.IO.StreamWriter logWriter)
        {
            int oldFaceCount = Mesh.Faces.Count;
            Picker.AddNewFaceAtMousePosition(mousePos, view, projection, controlWidth, controlHeight, logWriter);
            if (Mesh.Faces.Count != oldFaceCount)
                Buffers.UpdateAll(Mesh);
        }

        public void AddNewVertexAtPosition(Vector3 projectedPos, int baseFaceIndex, float radius = 0.1f)
        {
            int oldVertexCount = Mesh.Vertices.Count;
            Picker.AddNewVertexAtPosition(projectedPos, baseFaceIndex, radius);
            if (Mesh.Vertices.Count != oldVertexCount)
                Buffers.UpdateAll(Mesh);
        }

        public void MoveSelectedFace(Vector3 offset)
        {
            if (SelectedFaceIndex == -1) return;

            var face = Mesh.Faces[SelectedFaceIndex];
            if (face.v1 < Mesh.Vertices.Count) Mesh.Vertices[face.v1] += offset;
            if (face.v2 < Mesh.Vertices.Count) Mesh.Vertices[face.v2] += offset;
            if (face.v3 < Mesh.Vertices.Count) Mesh.Vertices[face.v3] += offset;

            Mesh.UpdateIndicesAndEdges();
            Buffers.UpdateAll(Mesh);
        }

        public void MoveSelectedVertex(Vector3 newPosition)
        {
            if (SelectedVertexIndex >= 0 && SelectedVertexIndex < Mesh.Vertices.Count)
            {
                Mesh.Vertices[SelectedVertexIndex] = newPosition;
                Mesh.UpdateIndicesAndEdges();
                Buffers.UpdateAll(Mesh);
            }
        }

        public Vector3 GetFaceCenter(int faceIndex)
        {
            return Mesh.GetFaceCenter(faceIndex);
        }

        public void SaveToObj(string path)
        {
            Mesh.SaveToObj(path);
        }

        public void Render(Matrix4 view, Matrix4 projection,
                           bool showEdges, bool showVertices, SimpleLighting lighting)
        {
            Render(view, projection, showEdges, showVertices, false, lighting);
        }

        public void Render(Matrix4 view, Matrix4 projection,
                           bool showEdges, bool showVertices, bool verticesOccludedByDepth, SimpleLighting lighting)
        {
            var modelMatrix = GetModelMatrix();
            var colorVec = GetModelColorVector();
            lighting.Use(modelMatrix, view, projection, colorVec);

            GL.BindVertexArray(Buffers.Vao);
            GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Count, DrawElementsType.UnsignedInt, 0);
            lighting.Unbind();
            GL.BindVertexArray(0);

            EdgeVertexShader.Use(modelMatrix, view, projection, new OpenTK.Graphics.Color4(1f, 1f, 0f, 1f));
            if (showEdges && Mesh.Edges.Count > 0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.LineWidth(4.0f);
                GL.BindVertexArray(Buffers.EdgesVao);
                GL.DrawArrays(PrimitiveType.Lines, 0, Mesh.Edges.Count);
                GL.Enable(EnableCap.DepthTest);
            }

            if (showVertices && Mesh.Vertices.Count > 0)
            {
                EdgeVertexShader.Use(modelMatrix, view, projection, new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f));
                GL.PointSize(12.0f);

                if (verticesOccludedByDepth)
                {
                    GL.Enable(EnableCap.DepthTest);
                }
                else
                {
                    GL.Disable(EnableCap.DepthTest);
                }

                GL.BindVertexArray(Buffers.PointsVao);
                GL.DrawArrays(PrimitiveType.Points, 0, Mesh.Vertices.Count);
                GL.Enable(EnableCap.DepthTest);

                GL.BindVertexArray(0);
            }

            EdgeVertexShader.Stop();
        }

        public void Dispose()
        {
            Buffers.Cleanup();
            EdgeVertexShader.Dispose();
        }
    }
}
