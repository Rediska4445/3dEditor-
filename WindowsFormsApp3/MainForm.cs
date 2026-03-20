using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        private SimpleLighting lighting;
        private Model3D model;
        private Grid3D grid;

        private StreamWriter logWriter;

        private float cameraDistance = 5f;
        private float angleX = 0f, angleY = 0f;
        private Point lastMousePos;

        private void Log(string message)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logWriter.WriteLine($"[{now}] {message}");
        }

        public MainForm()
        {
            InitializeComponent();

            logWriter = new StreamWriter("log.txt", append: true);
            logWriter.AutoFlush = true;

            SetupGLControl();
        }

        private void SetupGLControl()
        {
            glControl.Load += (s, e) => GlControl_Load();
            glControl.Paint += (s, e) => GlControl_Paint();
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.Resize += GlControl_Resize;

            checkBoxShowEdges.CheckedChanged += (s, e) => glControl.Invalidate();
            checkBoxShowVertices.CheckedChanged += (s, e) => glControl.Invalidate();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            glControl.Invalidate();
        }

        private void GlControl_Load()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            lighting = new SimpleLighting();
            grid = new Grid3D();
            model = null;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "OBJ|*.obj" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var context = new AssimpContext();
                        var scene = context.ImportFile(dlg.FileName,
                            PostProcessSteps.Triangulate |
                            PostProcessSteps.GenerateSmoothNormals |
                            PostProcessSteps.CalculateTangentSpace);

                        if (scene.Meshes.Count > 0)
                        {
                            model = new Model3D();
                            model.LoadFromAssimpMesh(scene.Meshes[0]);

                            Log($"Загружена модель: {model.VertexList.Count} вершин, {model.FaceList.Count} граней");
                            glControl.Invalidate();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Ошибка загрузки: {ex.Message}");
                        MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                    }
                }
            }
        }

        private void GlControl_Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) *
                       Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)glControl.Width / glControl.Height, 0.1f, 10000f);

            // 1. СНАЧАЛА СЕТКА (фиксированная внизу!)
            if (checkBoxShowGrid.Checked && grid != null)
            {
                GL.DepthMask(false); // Сетка не пишет в depth buffer
                grid.Render(view, projection, lighting);
                GL.DepthMask(true);  // Возвращаем запись depth
            }

            // 2. ПОТОМ МОДЕЛЬ (перекрывает сетку)
            if (model != null)
                model.Render(view, projection, checkBoxShowEdges.Checked, checkBoxShowVertices.Checked, lighting);

            glControl.SwapBuffers();
        }

        private Vector3 GetFaceCenter(int faceIndex)
        {
            if (model == null || faceIndex < 0 || faceIndex >= model.FaceList.Count)
                return Vector3.Zero;

            var face = model.FaceList[faceIndex];
            return (model.VertexList[face.v1] + model.VertexList[face.v2] + model.VertexList[face.v3]) / 3f;
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBoxModeEdit.Checked && e.Button == MouseButtons.Left && model != null)
            {
                Vector3 newWorldPos = ScreenToWorld(e.X, e.Y);

                if (checkBoxShowEdges.Checked && model.SelectedFaceIndex != -1)
                {
                    Vector3 faceCenter = GetFaceCenter(model.SelectedFaceIndex);
                    Vector3 offset = newWorldPos - faceCenter;
                    model.MoveSelectedFace(offset);
                }
                else if (checkBoxShowVertices.Checked && model.SelectedVertexIndex != -1)
                {
                    model.MoveSelectedVertex(newWorldPos);
                }
            }
            else
            {
                float deltaX = e.X - lastMousePos.X;
                float deltaY = e.Y - lastMousePos.Y;

                if (e.Button == MouseButtons.Left)
                {
                    angleY += deltaX * 0.01f;
                    angleX -= deltaY * 0.01f;
                }
                else if (e.Button == MouseButtons.Right && model != null)
                {
                    model.ModelAngleY += deltaX * 0.01f;
                    model.ModelAngleX -= deltaY * 0.01f;
                }
            }

            lastMousePos = e.Location;
            glControl.Invalidate();
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;

            if (checkBoxModeEdit.Checked && model != null)
            {
                var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) *
                           Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
                var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                    (float)glControl.Width / glControl.Height, 0.1f, 10000f);

                model.SelectedVertexIndex = -1;
                model.SelectedFaceIndex = -1;

                if (checkBoxAdd.Checked)
                {
                    model.AddNewFaceAtMousePosition(e.Location, view, projection,
                        glControl.Width, glControl.Height, logWriter);
                    return;
                }

                if (checkBoxShowEdges.Checked)
                    model.SelectedFaceIndex = model.FindClosestFace(e.Location, view, projection, glControl.Width, glControl.Height);
                else if (checkBoxShowVertices.Checked)
                    model.SelectedVertexIndex = model.FindClosestVertex(e.Location, view, projection, glControl.Width, glControl.Height);
            }
            glControl.Invalidate();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog
            {
                Filter = "OBJ files (*.obj)|*.obj",
                DefaultExt = "obj"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    model.SaveToObj(dlg.FileName);
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/Rediska4445/3dEditor-";
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть браузер: " + ex.Message);
            }
        }

        private Vector3 ScreenToWorld(int mouseX, int mouseY)
        {
            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) *
                       Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                       (float)glControl.Width / glControl.Height, 0.1f, 100f);

            var modelMatrix = Matrix4.CreateRotationX(model.ModelAngleX) * Matrix4.CreateRotationY(model.ModelAngleY);

            float x = (2.0f * mouseX) / glControl.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / glControl.Height;

            Vector4 rayStartNdc = new Vector4(x, y, -1.0f, 1.0f);
            Vector4 rayEndNdc = new Vector4(x, y, 1.0f, 1.0f);

            Matrix4 invModelViewProj = Matrix4.Invert(modelMatrix * view * projection);

            Vector4 rayStartWorld = Vector4.Transform(rayStartNdc, invModelViewProj);
            Vector4 rayEndWorld = Vector4.Transform(rayEndNdc, invModelViewProj);

            rayStartWorld /= rayStartWorld.W;
            rayEndWorld /= rayEndWorld.W;

            Vector3 dir = Vector3.Normalize(rayEndWorld.Xyz - rayStartWorld.Xyz);
            float dirY = dir.Y;

            if (Math.Abs(dirY) < 1e-6f)
            {
                Log("ScreenToWorld: dir.Y ≈ 0");
                return rayStartWorld.Xyz + dir * 5f;
            }

            float t = -rayStartWorld.Y / dirY;
            return rayStartWorld.Xyz + dir * t;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            cameraDistance -= e.Delta * 0.01f;
            if (cameraDistance < 1f) cameraDistance = 1f;
            if (cameraDistance > 20f) cameraDistance = 20f;
            glControl.Invalidate();
        }
    }
}