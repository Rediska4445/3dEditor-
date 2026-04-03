using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace WindowsFormsApp3
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public struct MyRay
    {
        public Vector3 Position;
        public Vector3 Direction;

        public MyRay(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }
    }

    public class Gizmo3D : Model3D
    {
        private readonly float length;
        private int axisStartIndex;

        private bool isDragging = false;
        private Axis? activeAxis = null;
        private Vector3 grabStartWorldPos;
        private Vector3 dragStartMousePos;
        private Vector3 dragStartObjectPos;

        public event Action<Axis, Vector3> OnAxisGrabbed;
        public event Action<Axis, Vector3> OnAxisDragging;
        public event Action<Axis, Vector3> OnAxisReleased;

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

            Mesh.Vertices.Add(origin);
            Mesh.Vertices.Add(origin + new Vector3(length, 0, 0));

            Mesh.Vertices.Add(origin);
            Mesh.Vertices.Add(origin + new Vector3(0, length, 0));

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

            OpenTK.Graphics.Color4 baseX = new OpenTK.Graphics.Color4(1.0f, 0.2f, 0.2f, 1.0f);
            OpenTK.Graphics.Color4 baseY = new OpenTK.Graphics.Color4(0.2f, 1.0f, 0.2f, 1.0f);
            OpenTK.Graphics.Color4 baseZ = new OpenTK.Graphics.Color4(0.2f, 0.2f, 1.0f, 1.0f);
            OpenTK.Graphics.Color4 activeColor = new OpenTK.Graphics.Color4(1.0f, 0.8f, 0.0f, 1.0f);

            OpenTK.Graphics.Color4 currentColorX = (activeAxis == Axis.X) ? activeColor : baseX;
            OpenTK.Graphics.Color4 currentColorY = (activeAxis == Axis.Y) ? activeColor : baseY;
            OpenTK.Graphics.Color4 currentColorZ = (activeAxis == Axis.Z) ? activeColor : baseZ;

            EdgeVertexShader.Use(positionMatrix, view, projection, currentColorX);
            GL.BindVertexArray(Buffers.EdgesVao);
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex, 2);

            EdgeVertexShader.Use(positionMatrix, view, projection, currentColorY);
            GL.DrawArrays(PrimitiveType.Lines, axisStartIndex + 2, 2);

            EdgeVertexShader.Use(positionMatrix, view, projection, currentColorZ);
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

        public bool HandleMouseMove(int screenX, int screenY, int[] viewport, Matrix4 view, Matrix4 projection)
        {
            if (!isDragging || activeAxis == null) return false;

            MyRay ray = CreateRayFromScreen(screenX, screenY, viewport, view, projection);
            Vector3 originWorld = new Vector3(_position.M41, _position.M42, _position.M43);

            Vector3 axisDir;
            if (activeAxis == Axis.X) axisDir = Vector3.UnitX;
            else if (activeAxis == Axis.Y) axisDir = Vector3.UnitY;
            else axisDir = Vector3.UnitZ;

            if (IntersectRayPlane(ray, grabStartWorldPos, axisDir, out Vector3 newPosition))
            {
                Vector3 dir = newPosition - originWorld;
                float projectLen = Vector3.Dot(dir, axisDir);
                projectLen = MathHelper.Clamp(projectLen, 0, length);

                Vector3 constrainedPos = originWorld + axisDir * projectLen;

                if (OnAxisDragging != null)
                    OnAxisDragging(activeAxis.Value, constrainedPos);
                return true;
            }

            return false;
        }

        public void HandleMouseUp()
        {
            if (!isDragging || activeAxis == null) return;

            Vector3 originWorld = new Vector3(_position.M41, _position.M42, _position.M43);
            Vector3 axisDir;
            if (activeAxis == Axis.X) axisDir = Vector3.UnitX;
            else if (activeAxis == Axis.Y) axisDir = Vector3.UnitY;
            else axisDir = Vector3.UnitZ;

            float finalLen = MathHelper.Clamp(Vector3.Dot(grabStartWorldPos - originWorld, axisDir), 0, length);
            Vector3 finalPos = originWorld + axisDir * finalLen;

            if (OnAxisReleased != null)
                OnAxisReleased(activeAxis.Value, finalPos);

            isDragging = false;
            activeAxis = null;
        }

        private static MyRay CreateRayFromScreen(int screenX, int screenY, int[] viewport, Matrix4 view, Matrix4 projection)
        {
            float nx = (2.0f * (screenX - viewport[0])) / viewport[2] - 1f;
            float ny = (2.0f * (screenY - viewport[1])) / viewport[3] - 1f;

            Matrix4 invViewProj = Matrix4.Invert(projection * view);

            Vector4 near4 = new Vector4(nx, ny, -1f, 1f);
            Vector4 far4 = new Vector4(nx, ny, 1f, 1f);

            Vector4 nearTrans = Vector4.Transform(near4, invViewProj);
            Vector4 farTrans = Vector4.Transform(far4, invViewProj);

            Vector3 nearWorld = ToVec3(nearTrans);
            Vector3 farWorld = ToVec3(farTrans);

            Vector3 dir = Vector3.Normalize(farWorld - nearWorld);
            return new MyRay(nearWorld, dir);
        }

        private static Vector3 ToVec3(Vector4 v)
        {
            float w = v.W;
            if (Math.Abs(w) < 1e-6f) w = 1f;
            return new Vector3(v.X / w, v.Y / w, v.Z / w);
        }

        public const float thickness = 1.5f;

        private static bool IntersectLineSegment(MyRay ray, Vector3 segStart, Vector3 segEnd, out float t, out Vector3 point)
        {
            Vector3 d = segEnd - segStart;
            float segLen2 = d.LengthSquared;

            if (segLen2 < 1e-8f)
            {
                t = float.MaxValue;
                point = new Vector3();
                return false;
            }

            Vector3 r = ray.Direction;
            Vector3 w = segStart - ray.Position;

            float a = Vector3.Dot(r, r);
            float b = Vector3.Dot(r, d);
            float c = Vector3.Dot(d, d);
            float e = Vector3.Dot(r, w);
            float f = Vector3.Dot(d, w);

            float det = a * c - b * b;
            const float eps = 1e-4f;

            if (Math.Abs(det) < eps)
            {
                Vector3 closest = segStart;
                float dist21 = (closest - ray.Position).LengthSquared;
                if (dist21 <= thickness * thickness)
                {
                    t = 0f;
                    point = closest;
                    return true;
                }
                t = float.MaxValue;
                point = new Vector3();
                return false;
            }

            float s = (b * e - a * f) / det;
            if (s < 0f) s = 0f;
            if (s > 1f) s = 1f;

            Vector3 closestOnSegment = segStart + d * s;
            float dist2 = (closestOnSegment - ray.Position).LengthSquared;

            if (dist2 > thickness * thickness)
            {
                t = float.MaxValue;
                point = new Vector3();
                return false;
            }

            point = closestOnSegment;
            t = Vector3.Dot(point - ray.Position, ray.Direction);
            return t >= 0f;
        }

        private static bool IntersectRayPlane(MyRay ray, Vector3 planePoint, Vector3 planeNormal, out Vector3 point)
        {
            float denom = Vector3.Dot(ray.Direction, planeNormal);
            if (Math.Abs(denom) < 1e-6f)
            {
                point = new Vector3();
                return false;
            }

            float t = Vector3.Dot(planePoint - ray.Position, planeNormal) / denom;  // ray.Position
            if (t < 0)
            {
                point = new Vector3();
                return false;
            }

            point = ray.Position + ray.Direction * t;  // ray.Position
            return true;
        }

        public static Vector3 ToOpenTK(Assimp.Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Assimp.Vector3D ToAssimp(Vector3 v)
        {
            return new Assimp.Vector3D(v.X, v.Y, v.Z);
        }
    }
}