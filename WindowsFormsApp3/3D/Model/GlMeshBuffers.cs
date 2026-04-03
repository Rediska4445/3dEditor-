using OpenTK.Graphics.OpenGL;
using System;

namespace WindowsFormsApp3
{
    public class GlMeshBuffers : IDisposable
    {
        public int Vao { get; private set; }
        public int VboVertices { get; private set; }
        public int Ebo { get; private set; }
        public int EdgesVao { get; private set; }
        public int PointsVao { get; private set; }
        public int VboEdges { get; private set; }
        public int VboVerticesPoints { get; private set; }

        public void Build(MeshData mesh)
        {
            Cleanup();
            SetupMainBuffers(mesh);
            SetupEdgesBuffer(mesh);
            SetupPointsBuffer(mesh);
        }

        public void UpdateAll(MeshData mesh)
        {
            UpdateMainBuffers(mesh);
            UpdateEdgesBuffer(mesh);
            UpdatePointsBuffer(mesh);
        }

        private void SetupMainBuffers(MeshData mesh)
        {
            Vao = GL.GenVertexArray();
            VboVertices = GL.GenBuffer();
            Ebo = GL.GenBuffer();

            GL.BindVertexArray(Vao);

            var vertexArray = mesh.ToInterleavedVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float),
                          vertexArray, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Count * sizeof(uint),
                          mesh.Indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        private void SetupEdgesBuffer(MeshData mesh)
        {
            EdgesVao = GL.GenVertexArray();
            VboEdges = GL.GenBuffer();

            GL.BindVertexArray(EdgesVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboEdges);
            var arr = mesh.ToEdgesArray();
            GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * sizeof(float),
                          arr, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void SetupPointsBuffer(MeshData mesh)
        {
            PointsVao = GL.GenVertexArray();
            VboVerticesPoints = GL.GenBuffer();

            GL.BindVertexArray(PointsVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVerticesPoints);
            var arr = mesh.ToVerticesOnlyArray();
            GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * sizeof(float),
                          arr, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void UpdateMainBuffers(MeshData mesh)
        {
            if (Vao == 0) return;

            GL.BindVertexArray(Vao);

            var vertexArray = mesh.ToInterleavedVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float),
                          vertexArray, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Count * sizeof(uint),
                          mesh.Indices.ToArray(), BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
        }

        private void UpdateEdgesBuffer(MeshData mesh)
        {
            if (EdgesVao == 0) return;

            GL.BindVertexArray(EdgesVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboEdges);
            var arr = mesh.ToEdgesArray();
            GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * sizeof(float),
                          arr, BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(0);
        }

        private void UpdatePointsBuffer(MeshData mesh)
        {
            if (PointsVao == 0) return;

            GL.BindVertexArray(PointsVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboVerticesPoints);
            var arr = mesh.ToVerticesOnlyArray();
            GL.BufferData(BufferTarget.ArrayBuffer, arr.Length * sizeof(float),
                          arr, BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(0);
        }

        public void Cleanup()
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

        public void Dispose() => Cleanup();
    }

}
