using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace WindowsFormsApp3
{
    public class Gizmo3D : Model3D
    {
        private readonly float length;
        private int axisStartIndex;

        public Gizmo3D(float length = 1.8f)
        {
            this.length = length;
            CreateGizmoData();
            Buffers.Build(Mesh);
        }

        private void CreateGizmoData()
        {
            Mesh.Clear();

            Vector3 origin = Vector3.Zero;

            // X - красный
            Mesh.Vertices.Add(origin);
            Mesh.Vertices.Add(origin + new Vector3(length, 0, 0));

            // Y - зелёный
            Mesh.Vertices.Add(origin);
            Mesh.Vertices.Add(origin + new Vector3(0, length, 0));

            // Z - синий
            Mesh.Vertices.Add(origin);
            Mesh.Vertices.Add(origin + new Vector3(0, 0, length));

            for (int i = 0; i < Mesh.Vertices.Count; i++)
                Mesh.Normals.Add(Vector3.UnitY);

            Mesh.Indices.Clear();
            Mesh.Edges.Clear();

            for (int i = 0; i < Mesh.Vertices.Count; i += 2)
            {
                Mesh.Indices.Add((uint)i);
                Mesh.Indices.Add((uint)(i + 1));
                Mesh.Edges.Add((uint)i);
                Mesh.Edges.Add((uint)(i + 1));
            }

            axisStartIndex = 0;
        }

        public void Render(Matrix4 view, Matrix4 projection, Matrix4 positionMatrix)
        {
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.LineWidth(4.0f);

            // X - красный
            EdgeVertexShader.Use(positionMatrix, view, projection,
                new OpenTK.Graphics.Color4(1.0f, 0.2f, 0.2f, 1.0f));
            GL.BindVertexArray(Buffers.EdgesVao);
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex, 2);

            // Y - зелёный
            EdgeVertexShader.Use(positionMatrix, view, projection,
                new OpenTK.Graphics.Color4(0.2f, 1.0f, 0.2f, 1.0f));
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 2, 2);

            // Z - синий
            EdgeVertexShader.Use(positionMatrix, view, projection,
                new OpenTK.Graphics.Color4(0.2f, 0.2f, 1.0f, 1.0f));
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 4, 2);

            GL.LineWidth(1.0f);

            GL.BindVertexArray(0);
            EdgeVertexShader.Stop();

            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
        }

        public new void SetPosition(Vector3 pos)
        {
            _position = Matrix4.CreateTranslation(pos);
        }

        public Matrix4 _position = Matrix4.Identity;
    }
}
