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
            BuildBuffers();
        }

        private void CreateGridData()
        {
            VertexList.Clear();
            NormalList.Clear();
            IndexList.Clear();
            EdgeList.Clear();

            const float size = 3.0f;
            const int divisions = 20;
            float step = (size * 2f) / divisions;

            const float FLOOR_Y = -1.8f;

            for (int i = 0; i <= divisions; i++)
            {
                float t = -size + i * step;
                VertexList.Add(new Vector3(-size, FLOOR_Y, t));
                VertexList.Add(new Vector3(+size, FLOOR_Y, t));
                VertexList.Add(new Vector3(t, FLOOR_Y, -size));
                VertexList.Add(new Vector3(t, FLOOR_Y, +size));
            }

            VertexList.Add(new Vector3(0, FLOOR_Y, 0)); VertexList.Add(new Vector3(1.8f, FLOOR_Y, 0)); // X красный
            VertexList.Add(new Vector3(0, FLOOR_Y, 0)); VertexList.Add(new Vector3(0, FLOOR_Y, 1.8f)); // Z зеленый  
            VertexList.Add(new Vector3(0, FLOOR_Y, 0)); VertexList.Add(new Vector3(0, FLOOR_Y + 1.8f, 0)); // Y синий (вверх)

            for (int i = 0; i < VertexList.Count; i++)
                NormalList.Add(Vector3.UnitY);

            IndexList.Clear();
            for (int i = 0; i < VertexList.Count; i += 2)
            {
                IndexList.Add((uint)i);
                IndexList.Add((uint)(i + 1));
            }
        }

        public void Render(Matrix4 view, Matrix4 projection, SimpleLighting lighting)
        {
            Matrix4 gridModel = Matrix4.Identity;

            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.UseProgram(shaderProgram);
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int colorLoc = GL.GetUniformLocation(shaderProgram, "drawColor");

            GL.UniformMatrix4(modelLoc, false, ref gridModel);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.BindVertexArray(Vao);

            int axisStartIndex = VertexList.Count - 6;
            GL.LineWidth(4.0f);
            GL.Uniform4(colorLoc, 1.0f, 0.2f, 0.2f, 1.0f); GL.DrawArrays(PrimitiveType.Lines, axisStartIndex, 2);
            GL.Uniform4(colorLoc, 0.2f, 1.0f, 0.2f, 1.0f); GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 2, 2);
            GL.Uniform4(colorLoc, 0.2f, 0.2f, 1.0f, 1.0f); GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 4, 2);

            GL.LineWidth(1.0f);
            GL.Uniform4(colorLoc, 0.6f, 0.6f, 0.6f, 0.15f);
            GL.DrawArrays(PrimitiveType.Lines, 0, axisStartIndex);

            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
