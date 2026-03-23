using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp3.Forms;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        private readonly System.Collections.Generic.Dictionary<string, string> _presets =
    new System.Collections.Generic.Dictionary<string, string>();

        private SimpleLighting lighting;
        private Model3D model;
        private Grid3D grid;
        private Camera3D camera;

        private StreamWriter logWriter;

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

        private void InitPresets()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string modelsDir = System.IO.Path.Combine(baseDir, "Presets");

            if (!System.IO.Directory.Exists(modelsDir))
            {
                Log("Папка с пресетами не найдена: " + modelsDir);
                return;
            }

            string[] files = System.IO.Directory.GetFiles(modelsDir, "*.obj");
            if (files.Length == 0)
            {
                Log("В папке пресетов нет OBJ-файлов: " + modelsDir);
                return;
            }

            listBox1.Items.Clear();
            _presets.Clear();

            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                _presets[name] = file;
                listBox1.Items.Add(name);
            }

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
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
            camera = new Camera3D();
            grid = new Grid3D();
            model = null;

            InitPresets();
        }

        private void LoadObject()
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

                            Log(
                                $"Загружена модель: {model.Mesh.Vertices.Count} вершин, {model.Mesh.Faces.Count} граней"
                            );
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

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadObject();
        }

        private void GlControl_Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            var view = camera.GetViewMatrix();
            var projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);

            if (checkBoxShowGrid.Checked && grid != null)
            {
                GL.DepthMask(false);
                grid.Render(view, projection);
                GL.DepthMask(true);
            }

            if (model != null)
                model.Render(view, projection,
                             checkBoxShowEdges.Checked,
                             checkBoxShowVertices.Checked,
                             lighting);

            glControl.SwapBuffers();
        }

        private Vector3 GetFaceCenter(int faceIndex)
        {
            if (model == null || faceIndex < 0 || faceIndex >= model.Mesh.Faces.Count)
                return Vector3.Zero;

            return model.Mesh.GetFaceCenter(faceIndex);
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
                    camera.Rotate(deltaX, deltaY);
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
            Log($"MouseDown at ({e.X}, {e.Y}), Add={checkBoxAdd.Checked}, Delete={checkBoxRemove.Checked}");

            if (checkBoxModeEdit.Checked && model != null)
            {
                var view = camera.GetViewMatrix();
                var projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);

                model.SelectedVertexIndex = -1;
                model.SelectedFaceIndex = -1;

                if (checkBoxAdd.Checked)
                {
                    model.AddNewFaceAtMousePosition(e.Location, view, projection,
                        glControl.Width, glControl.Height, logWriter);
                    return;
                }

                if (checkBoxRemove.Checked)
                {
                    int deletedFace = model.DeleteFaceAtMousePosition(e.Location, view, projection, glControl.Width, glControl.Height);
                    if (deletedFace != -1)
                        Log($"Удалена грань #{deletedFace}");
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
            SaveObject();
        }

        private void SaveObject()
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
            var view = camera.GetViewMatrix();
            var projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);
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
                return rayStartWorld.Xyz + dir * 5f;

            float t = -rayStartWorld.Y / dirY;
            return rayStartWorld.Xyz + dir * t;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.Zoom(e.Delta);
            glControl.Invalidate();
        }

        private void toolStripMenuItemImport_Click(object sender, EventArgs e)
        {
            LoadObject();
        }

        private void toolStripMenuItemExport_Click(object sender, EventArgs e)
        {
            SaveObject();
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripMenuItem1.Checked = !toolStripMenuItem1.Checked;

            flowLayoutPanelLoadSaveButtons.Visible = toolStripMenuItem1.Checked;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            toolStripMenuItem2.Checked = !toolStripMenuItem2.Checked;

            groupBoxView.Visible = toolStripMenuItem2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (model == null)
                return;

            using (var dlg = new ColorDialog())
            {
                dlg.FullOpen = true;
                dlg.Color = model.ModelColorRgb;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    model.SetColorRgb(dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    glControl.Invalidate();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            string presetName = listBox1.SelectedItem.ToString();
            if (!_presets.ContainsKey(presetName))
                return;

            string path = _presets[presetName];

            try
            {
                var context = new Assimp.AssimpContext();
                var scene = context.ImportFile(path,
                    Assimp.PostProcessSteps.Triangulate |
                    Assimp.PostProcessSteps.GenerateSmoothNormals |
                    Assimp.PostProcessSteps.CalculateTangentSpace);

                if (scene.Meshes.Count > 0)
                {
                    model = new Model3D();
                    model.LoadFromAssimpMesh(scene.Meshes[0]);

                    Log(string.Format(
                        "Загружен пресет '{0}': {1} вершин, {2} граней",
                        presetName,
                        model.Mesh.Vertices.Count,
                        model.Mesh.Faces.Count));

                    glControl.Invalidate();
                }
                else
                {
                    Log("В файле пресета нет мешей: " + path);
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка загрузки пресета '" + presetName + "': " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }
    }
}