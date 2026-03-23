using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace WindowsFormsApp3
{
    public class Grid3D : Model3D
    {
        public Grid3D()
        {
            CreateGridData();
            Buffers.Build(Mesh);
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

            Mesh.Vertices.Add(new Vector3(0, FLOOR_Y, 0)); Mesh.Vertices.Add(new Vector3(1.8f, FLOOR_Y, 0));          // X
            Mesh.Vertices.Add(new Vector3(0, FLOOR_Y, 0)); Mesh.Vertices.Add(new Vector3(0, FLOOR_Y, 1.8f));          // Z
            Mesh.Vertices.Add(new Vector3(0, FLOOR_Y, 0)); Mesh.Vertices.Add(new Vector3(0, FLOOR_Y + 1.8f, 0));      // Y

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

            int axisStartIndex = Mesh.Vertices.Count - 6;

            GL.LineWidth(4.0f);

            EdgeVertexShader.Use(gridModel, view, projection,
                new OpenTK.Graphics.Color4(1.0f, 0.2f, 0.2f, 1.0f)); // X
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex, 2);

            EdgeVertexShader.Use(gridModel, view, projection,
                new OpenTK.Graphics.Color4(0.2f, 1.0f, 0.2f, 1.0f)); // Z
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 2, 2);

            EdgeVertexShader.Use(gridModel, view, projection,
                new OpenTK.Graphics.Color4(0.2f, 0.2f, 1.0f, 1.0f)); // Y
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 4, 2);

            GL.LineWidth(1.0f);
            EdgeVertexShader.Use(gridModel, view, projection,
                new OpenTK.Graphics.Color4(0.6f, 0.6f, 0.6f, 0.15f));
            GL.DrawArrays(PrimitiveType.Lines, 0, axisStartIndex);

            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            EdgeVertexShader.Stop();

            GL.DepthMask(true);
        }
    }
}