using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace WindowsFormsApp3
{
    public class Grid3D : Model3D
    {
        public Gizmo3D Gizmo { get; private set; }

        public Grid3D()
        {
            CreateGridData();
            Buffers.Build(Mesh);

            const float size = 3.0f;
            const float FLOOR_Y = -1.8f;
            Vector3 gridOrigin = new Vector3(-size, FLOOR_Y, -size);

            Gizmo = new Gizmo3D(50.0f);
            Gizmo.SetPosition(gridOrigin);
        }

        private void CreateGridData()
        {
            Mesh.Clear();

            const float size = 3.0f;
            const int divisions = 20;
            float step = (size * 2f) / divisions;

            const float FLOOR_Y = -1.8f;

            for (int i = 0; i <= divisions; i++)
            {
                float t = -size + i * step;

                Mesh.Vertices.Add(new Vector3(-size, FLOOR_Y, t));
                Mesh.Vertices.Add(new Vector3(+size, FLOOR_Y, t));

                Mesh.Vertices.Add(new Vector3(t, FLOOR_Y, -size));
                Mesh.Vertices.Add(new Vector3(t, FLOOR_Y, +size));
            }

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
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            Matrix4 gridModel = Matrix4.Identity;

            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            EdgeVertexShader.Use(gridModel, view, projection,
                new OpenTK.Graphics.Color4(0.6f, 0.6f, 0.6f, 0.15f));

            GL.BindVertexArray(Buffers.PointsVao);
            GL.BindVertexArray(Buffers.EdgesVao);

            GL.LineWidth(1.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, Mesh.Vertices.Count);

            GL.BindVertexArray(0);
            EdgeVertexShader.Stop();

            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);

            Gizmo.Render(view, projection, Gizmo._position);
        }
    }
}
