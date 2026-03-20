using OpenTK;

namespace WindowsFormsApp3
{
    public class Camera3D
    {
        public float Distance { get; set; } = 5f;
        public float AngleX { get; set; } = 0f;
        public float AngleY { get; set; } = 0f;

        private const float MIN_PITCH = -1.47f;
        private const float MAX_PITCH = 1.47f;

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.CreateTranslation(0, 0, -Distance) *
                   Matrix4.CreateRotationX(AngleX) *
                   Matrix4.CreateRotationY(AngleY);
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
            AngleY += deltaX * 0.01f;
            AngleX += deltaY * 0.01f;

            if (AngleX > MAX_PITCH) AngleX = MAX_PITCH;
            if (AngleX < MIN_PITCH) AngleX = MIN_PITCH;
        }

        public void Zoom(float delta)
        {
            Distance -= delta * 0.01f;
            if (Distance < 1f) Distance = 1f;
            if (Distance > 20f) Distance = 20f;
        }
    }
}
