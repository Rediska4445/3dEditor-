using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace WindowsFormsApp3
{
    public enum GizmoAxis
    {
        None,
        X,
        Y,
        Z
    }

    public class VertexGizmo
    {
        public bool IsActive { get; private set; }
        public GizmoAxis ActiveAxis { get; private set; } = GizmoAxis.None;

        public Vector3 Position;

        public float AxisLength { get; set; } = 0.5f;
        public float DragScale { get; set; } = 0.01f;

        private bool _isDragging = false;
        private Point _dragStartMouse;
        private Vector3 _dragStartPosition;

        public void Activate(Vector3 position)
        {
            IsActive = true;
            ActiveAxis = GizmoAxis.None;
            Position = position;
        }

        public void Deactivate()
        {
            IsActive = false;
            ActiveAxis = GizmoAxis.None;
            _isDragging = false;
        }

        public void StartDrag(GizmoAxis axis, Point mousePos)
        {
            if (!IsActive || axis == GizmoAxis.None) return;
            ActiveAxis = axis;
            _isDragging = true;
            _dragStartMouse = mousePos;
            _dragStartPosition = Position;
        }

        public void EndDrag()
        {
            _isDragging = false;
            ActiveAxis = GizmoAxis.None;
        }

        public Vector3 UpdateDrag(Point currentMouse)
        {
            if (!_isDragging || ActiveAxis == GizmoAxis.None) return Position;

            float dx = currentMouse.X - _dragStartMouse.X;
            float dy = currentMouse.Y - _dragStartMouse.Y;

            float delta = (dx - dy) * DragScale;

            Vector3 newPos = _dragStartPosition;
            switch (ActiveAxis)
            {
                case GizmoAxis.X:
                    newPos.X += delta;
                    break;
                case GizmoAxis.Y:
                    newPos.Y += delta;
                    break;
                case GizmoAxis.Z:
                    newPos.Z += delta;
                    break;
            }

            Position = newPos;
            return Position;
        }
        public GizmoAxis HitTestAxis(
            Point mousePos,
            Matrix4 view,
            Matrix4 projection,
            int width,
            int height)
        {
            if (!IsActive) return GizmoAxis.None;

            Matrix4 model = Matrix4.Identity;

            Func<Vector3, Vector3> project = world =>
            {
                Vector4 clip = Vector4.Transform(new Vector4(world, 1.0f), model * view * projection);
                Vector3 ndc = new Vector3(
                    clip.X / clip.W,
                    clip.Y / clip.W,
                    clip.Z / clip.W
                );
                float sx = (ndc.X * 0.5f + 0.5f) * width;
                float sy = (1.0f - ndc.Y * 0.5f - 0.5f) * height;
                return new Vector3(sx, sy, ndc.Z);
            };

            GizmoAxis result = GizmoAxis.None;
            float bestDist = 10f;

            {
                Vector3 p0 = project(Position);
                Vector3 p1 = project(Position + Vector3.UnitX * AxisLength);
                float dist = DistancePointToSegment(mousePos, p0, p1);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    result = GizmoAxis.X;
                }
            }

            {
                Vector3 p0 = project(Position);
                Vector3 p1 = project(Position + Vector3.UnitY * AxisLength);
                float dist = DistancePointToSegment(mousePos, p0, p1);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    result = GizmoAxis.Y;
                }
            }

            {
                Vector3 p0 = project(Position);
                Vector3 p1 = project(Position + Vector3.UnitZ * AxisLength);
                float dist = DistancePointToSegment(mousePos, p0, p1);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    result = GizmoAxis.Z;
                }
            }

            return result;
        }

        private float DistancePointToSegment(Point p, Vector3 a, Vector3 b)
        {
            float px = p.X;
            float py = p.Y;

            var ap = new OpenTK.Vector2(px - a.X, py - a.Y);
            var ab = new OpenTK.Vector2(b.X - a.X, b.Y - a.Y);

            float abLen2 = ab.LengthSquared;
            if (abLen2 < 1e-6f)
                return ap.Length;

            float t = OpenTK.Vector2.Dot(ap, ab) / abLen2;
            t = Math.Max(0f, Math.Min(1f, t));
            var proj = new OpenTK.Vector2(a.X, a.Y) + ab * t;
            return (new OpenTK.Vector2(px, py) - proj).Length;
        }

        public void Render(SimpleColorShader shader, Matrix4 view, Matrix4 projection)
        {
            if (!IsActive) return;

            Matrix4 model = Matrix4.Identity;

            GL.Disable(EnableCap.DepthTest);

            DrawAxis(shader, model, view, projection,
                     Position, Vector3.UnitX, AxisLength,
                     new OpenTK.Graphics.Color4(1f, 0f, 0f, 1f));

            DrawAxis(shader, model, view, projection,
                     Position, Vector3.UnitY, AxisLength,
                     new OpenTK.Graphics.Color4(0f, 1f, 0f, 1f));

            DrawAxis(shader, model, view, projection,
                     Position, Vector3.UnitZ, AxisLength,
                     new OpenTK.Graphics.Color4(0f, 0f, 1f, 1f));

            GL.Enable(EnableCap.DepthTest);
        }

        private void DrawAxis(SimpleColorShader shader,
                              Matrix4 model, Matrix4 view, Matrix4 projection,
                              Vector3 origin, Vector3 dir, float len,
                              OpenTK.Graphics.Color4 color)
        {
            Vector3 p0 = origin;
            Vector3 p1 = origin + dir * len;

            float[] line = {
                p0.X, p0.Y, p0.Z,
                p1.X, p1.Y, p1.Z
            };

            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          sizeof(float) * line.Length,
                          line,
                          BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                                   3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            shader.Use(model, view, projection, color);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);

            GL.BindVertexArray(0);
            shader.Stop();

            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
        }
    }
}
