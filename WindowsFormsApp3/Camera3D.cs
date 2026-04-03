using OpenTK;
using System;

namespace WindowsFormsApp3
{
    public class Camera3D
    {
        public float Distance { get; set; } = 5f;
        public float Azimuth { get; set; } = 0f;
        public float Elevation { get; set; } = 0f;

        private const float MIN_ELEV = -0.4f;
        private const float MAX_ELEV = 0.8f;

        public Matrix4 GetViewMatrix(Vector3? target = null)
        {
            Vector3 lookTarget = target ?? new Vector3(0, -1.8f, 0);

            float x = Distance * (float)Math.Cos(Elevation) * (float)Math.Sin(Azimuth);
            float y = Distance * (float)Math.Sin(Elevation);
            float z = Distance * (float)Math.Cos(Elevation) * (float)Math.Cos(Azimuth);

            Vector3 eye = lookTarget + new Vector3(x, y, z);
            return Matrix4.LookAt(eye, lookTarget, Vector3.UnitY);
        }

        public Vector3 GetPosition()
        {
            Vector3 lookTarget = new Vector3(0, -1.8f, 0);
            float x = Distance * (float)Math.Cos(Elevation) * (float)Math.Sin(Azimuth);
            float y = Distance * (float)Math.Sin(Elevation);
            float z = Distance * (float)Math.Cos(Elevation) * (float)Math.Cos(Azimuth);
            return lookTarget + new Vector3(x, y, z);
        }

        public Matrix4 GetProjectionMatrix(int width, int height)
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                OpenTK.MathHelper.PiOver4,
                (float)width / height,
                0.1f, 10000f);
        }

        public void Rotate(float deltaX, float deltaY)
        {
            Azimuth += deltaX * 0.01f;
            Elevation -= deltaY * 0.01f;

            if (Elevation > MAX_ELEV) Elevation = MAX_ELEV;
            if (Elevation < MIN_ELEV) Elevation = MIN_ELEV;
        }

        public void Zoom(float delta)
        {
            Distance -= delta * 0.01f;
            if (Distance < 1f) Distance = 1f;
            if (Distance > 20f) Distance = 20f;
        }

        public Vector3 Right => new Vector3(
     GetViewMatrix().M11, GetViewMatrix().M12, GetViewMatrix().M13).Normalized();

        public Vector3 Up => new Vector3(
            GetViewMatrix().M21, GetViewMatrix().M22, GetViewMatrix().M23).Normalized();

        public Vector3 Forward => -new Vector3(
            GetViewMatrix().M31, GetViewMatrix().M32, GetViewMatrix().M33).Normalized();
    }
}
