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

        public static SimpleLighting lighting;
        public static Camera3D camera;
        public Model3D model;
        public Grid3D grid;

        public static StreamWriter logWriter;

        private Point lastMousePos;

        public static float scaleFactor = 1.1f;
        public static float translateModelStep = 0.1f;

        private string _transformMode = "Камеры";

        public static void Log(string message)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logWriter.WriteLine($"[{now}] {message}");
        }

        public MainForm()
        {
            InitializeComponent();

            logWriter = new StreamWriter("log.txt", append: true);
            logWriter.AutoFlush = true;

            this.KeyPreview = true;

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

            var allFiles = new System.Collections.Generic.List<string>();
            allFiles.AddRange(System.IO.Directory.GetFiles(modelsDir, "*.obj"));
            allFiles.AddRange(System.IO.Directory.GetFiles(modelsDir, "*.stl"));

            if (allFiles.Count == 0)
            {
                Log("В папке пресетов нет OBJ/STL-файлов: " + modelsDir);
                return;
            }

            listBox1.Items.Clear();
            _presets.Clear();

            foreach (string file in allFiles)
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
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.Resize += GlControl_Resize;
            this.KeyDown += MainForm_KeyDown;

            checkBoxShowEdges.CheckedChanged += (s, e) => glControl.Invalidate();
            checkBoxShowVertices.CheckedChanged += (s, e) => glControl.Invalidate();
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (grid != null)
            {
                grid.Gizmo.HandleMouseUp();
                Log("Гизмо: MouseUp вызван");
            }

            _isDraggingSelected = false;
            glControl.Capture = false;
            glControl.Invalidate();
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

            grid.Gizmo.OnAxisGrabbed += delegate (Axis axis, Vector3 pos)
            {
                Log(string.Format(
                    "Гизмо ЗАХВАТ: ось {0}, позиция ({1:F3}, {2:F3}, {3:F3})",
                    axis, pos.X, pos.Y, pos.Z));
            };

            grid.Gizmo.OnAxisDragging += delegate (Axis axis, Vector3 pos)
            {
                Log(string.Format(
                    "Гизмо DRAG: ось {0}, позиция ({1:F3}, {2:F3}, {3:F3})",
                    axis, pos.X, pos.Y, pos.Z));
            };

            grid.Gizmo.OnAxisReleased += delegate (Axis axis, Vector3 pos)
            {
                Log(string.Format(
                    "Гизмо ФИНАЛ: ось {0}, позиция ({1:F3}, {2:F3}, {3:F3})",
                    axis, pos.X, pos.Y, pos.Z));
            };

            InitPresets();
        }

        private void LoadObject()
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "OBJ/STL|*.obj;*.stl|OBJ|*.obj|STL|*.stl"
            })
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
                            var combinedMesh = CombineMeshes(scene.Meshes);
                            model.LoadFromAssimpMesh(combinedMesh);

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
                             отключитьГлубинуДляВершинToolStripMenuItem.Checked,
                             отключитьГлубинуДляГранейToolStripMenuItem.Checked,
                             lighting);

            glControl.SwapBuffers();
        }

        private Vector3 GetFaceCenter(int faceIndex)
        {
            if (model == null || faceIndex < 0 || faceIndex >= model.Mesh.Faces.Count)
                return Vector3.Zero;

            return model.Mesh.GetFaceCenter(faceIndex);
        }

        private Point _dragStartPos;
        private bool _isDraggingSelected = false;

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            var view = camera.GetViewMatrix();
            var projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);

            if (checkBoxModeEdit.Checked && glControl.Capture && grid != null)
            {
                int[] viewport = new int[4];
                GL.GetInteger(GetPName.Viewport, viewport);

                int screenX = e.X;
                int screenY = viewport[3] - e.Y;

                bool dragged = grid.Gizmo.HandleMouseMove(screenX, screenY, viewport, view, projection);
                if (dragged)
                {
                    glControl.Invalidate();
                    return;
                }
            }

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
                    if (сцеплениеЭлементовToolStripMenuItem.Checked) 
                    {
                        model.MoveSelectedVertexWithExternalLinks(newWorldPos);
                    }
                    else
                    {
                        model.MoveSelectedVertex(newWorldPos);
                    }
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
                    model.ModelAngleX += deltaY * 0.01f;
                }
            }

            if (_isDraggingSelected && checkBoxModeEdit.Checked && model != null)
            {
                Point delta = new Point(e.X - _dragStartPos.X, e.Y - _dragStartPos.Y);

                if (model.SelectedFaceIndex >= 0)
                {
                    Vector3 worldDelta = GetDragDeltaForMode(delta.X, delta.Y);
                    model.MoveSelectedFace(worldDelta);
                    Log($"Drag Face: delta=({delta.X},{delta.Y}), mode={_transformMode}");
                }
                else if (model.SelectedVertexIndex >= 0)
                {
                    Vector3 worldPos = ScreenToWorld(e.X, e.Y);
                    model.MoveSelectedVertex(worldPos);
                }

                _dragStartPos = e.Location; 
                glControl.Invalidate();
                return;
            }

            lastMousePos = e.Location;
            glControl.Invalidate();
        }

        private Vector3 GetPositionRelativeMode(Vector3 worldPos)
        {
            switch (_transformMode)
            {
                case "Камеры":
                    Matrix4 viewInv = Matrix4.Invert(camera.GetViewMatrix());
                    return Vector3.TransformPosition(worldPos, viewInv);  // View space

                case "Локальной модели":
                    Matrix4 modelInv = Matrix4.Invert(model.GetModelMatrix());
                    return Vector3.TransformPosition(worldPos, modelInv);  // Local model

                case "Мира":
                default:
                    return worldPos;  // World space
            }
        }

        private Vector3 GetOffsetRelativeMode(Vector3 worldOffset)
        {
            switch (_transformMode)
            {
                case "Камеры":
                    Matrix4 viewInv = Matrix4.Invert(camera.GetViewMatrix());
                    return Vector3.TransformPosition(worldOffset, viewInv);

                case "Локальной модели":
                    Matrix4 modelInv = Matrix4.Invert(model.GetModelMatrix());
                    return Vector3.TransformPosition(worldOffset, modelInv);

                case "Мира":
                default:
                    return worldOffset;
            }
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            lastMousePos = e.Location;
            Log($"LMB at ({e.X}, {e.Y})");

            var view = camera.GetViewMatrix();
            var projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);

            if (!checkBoxModeEdit.Checked || model == null)
                return;

            if (checkBoxAdd.Checked)
            {
                model.SelectedVertexIndex = model.SelectedFaceIndex = -1;
                model.AddNewFaceAtMousePosition(e.Location, view, projection, glControl.Width, glControl.Height, logWriter);
                glControl.Invalidate();
                return;
            }

            if (checkBoxRemove.Checked)
            {
                model.SelectedVertexIndex = model.SelectedFaceIndex = -1;
                int deleted = model.DeleteFaceAtMousePosition(e.Location, view, projection, glControl.Width, glControl.Height);
                if (deleted != -1) Log($"Удалена грань #{deleted}");
                glControl.Invalidate();
                return;
            }

            if (!checkBoxShowEdges.Checked && !checkBoxShowVertices.Checked)
                return;

            int prevFace = model.SelectedFaceIndex;
            int prevVertex = model.SelectedVertexIndex;
            model.SelectedVertexIndex = model.SelectedFaceIndex = -1;

            bool foundSomething = false;

            if (checkBoxShowEdges.Checked)
            {
                int faceIdx = model.FindClosestFace(e.Location, view, projection, glControl.Width, glControl.Height);
                if (faceIdx >= 0)
                {
                    model.SelectedFaceIndex = faceIdx;
                    foundSomething = true;
                    var center = model.GetFaceCenter(faceIdx);
                    Log($"ГРАНЬ #{faceIdx}");

                    _updatingVertexUi = true;
                    trackBar1.Value = (int)(center.X / TRACKBAR_SCALE);
                    trackBar2.Value = (int)(center.Y / TRACKBAR_SCALE);
                    trackBar3.Value = (int)(center.Z / TRACKBAR_SCALE);
                    numericVertexX.Value = (decimal)center.X;
                    numericVertexY.Value = (decimal)center.Y;
                    numericVertexZ.Value = (decimal)center.Z;
                    _updatingVertexUi = false;
                }
            }

            if (!foundSomething && checkBoxShowVertices.Checked)
            {
                int vertexIdx = model.FindClosestVertex(e.Location, view, projection, glControl.Width, glControl.Height);
                if (vertexIdx >= 0 && vertexIdx < model.Mesh.Vertices.Count)
                {
                    model.SelectedVertexIndex = vertexIdx;
                    foundSomething = true;
                    var v = model.Mesh.Vertices[vertexIdx];
                    Log("ВЕРШИНА #" + vertexIdx);

                    _updatingVertexUi = true;

                    float clampedX = Math.Max(trackBar1.Minimum * TRACKBAR_SCALE, Math.Min(trackBar1.Maximum * TRACKBAR_SCALE, v.X));
                    float clampedY = Math.Max(trackBar2.Minimum * TRACKBAR_SCALE, Math.Min(trackBar2.Maximum * TRACKBAR_SCALE, v.Y));
                    float clampedZ = Math.Max(trackBar3.Minimum * TRACKBAR_SCALE, Math.Min(trackBar3.Maximum * TRACKBAR_SCALE, v.Z));

                    trackBar1.Value = (int)(clampedX / TRACKBAR_SCALE);
                    trackBar2.Value = (int)(clampedY / TRACKBAR_SCALE);
                    trackBar3.Value = (int)(clampedZ / TRACKBAR_SCALE);

                    decimal safeX = Math.Max(numericVertexX.Minimum, Math.Min(numericVertexX.Maximum, (decimal)v.X));
                    decimal safeY = Math.Max(numericVertexX.Minimum, Math.Min(numericVertexX.Maximum, (decimal)v.Y));
                    decimal safeZ = Math.Max(numericVertexX.Minimum, Math.Min(numericVertexX.Maximum, (decimal)v.Z));

                    numericVertexX.Value = safeX;
                    numericVertexY.Value = safeY;
                    numericVertexZ.Value = safeZ;

                    _updatingVertexUi = false;
                }
                else
                {
                    model.SelectedVertexIndex = -1;
                    Log("Vertex index invalid: " + vertexIdx);
                }
            }

            if (!foundSomething)
            {
                Log("Пустой клик - игнор");
                model.SelectedFaceIndex = prevFace;
                model.SelectedVertexIndex = prevVertex;
                return;
            }

            if (foundSomething && (model.SelectedFaceIndex >= 0 || model.SelectedVertexIndex >= 0))
            {
                _isDraggingSelected = true;
                _dragStartPos = e.Location;
                Log($"Drag start: грань={model.SelectedFaceIndex}, вершина={model.SelectedVertexIndex}");
            }

            glControl.Capture = true;
            glControl.Invalidate();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            SaveObject();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (model == null)
            {
                Log("KeyDown: model == null");
                return;
            }

            Vector3 offset = Vector3.Zero;

            switch (e.KeyCode)
            {
                case Keys.W: offset.Z -= translateModelStep; break;
                case Keys.S: offset.Z += translateModelStep; break;
                case Keys.A: offset.X -= translateModelStep; break;
                case Keys.D: offset.X += translateModelStep; break;
                case Keys.Q: offset.Y -= translateModelStep; break;
                case Keys.E: offset.Y += translateModelStep; break;
                case Keys.OemOpenBrackets:
                    model.MultiplyScale(1f / scaleFactor);
                    Log("Scale down: " + model.ModelScale);
                    glControl.Invalidate();
                    return;

                case Keys.OemCloseBrackets:
                    model.MultiplyScale(scaleFactor);
                    Log("Scale up: " + model.ModelScale);
                    glControl.Invalidate();
                    return;
                default:
                    Log("KeyDown: ignored key " + e.KeyCode);
                    return;
            }

            Vector3 before = model.ModelPosition;
            model.Translate(offset);
            Vector3 after = model.ModelPosition;

            Log(string.Format(
                "KeyDown: {0}, offset=({1:0.###},{2:0.###},{3:0.###}), pos: ({4:0.###},{5:0.###},{6:0.###}) -> ({7:0.###},{8:0.###},{9:0.###})",
                e.KeyCode,
                offset.X, offset.Y, offset.Z,
                before.X, before.Y, before.Z,
                after.X, after.Y, after.Z
            ));

            glControl.Invalidate();
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

                    var combinedMesh = CombineMeshes(scene.Meshes);
                    model.LoadFromAssimpMesh(combinedMesh);

                    Log(string.Format(
                        "Загружен пресет '{0}': {1} вершин, {2} граней (мешей в сцене: {3})",
                        presetName,
                        model.Mesh.Vertices.Count,
                        model.Mesh.Faces.Count,
                        scene.Meshes.Count));

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

        public static Assimp.Mesh CombineMeshes(System.Collections.Generic.IList<Assimp.Mesh> meshes)
        {
            var result = new Assimp.Mesh("combined", Assimp.PrimitiveType.Triangle);

            int vertexOffset = 0;

            foreach (var m in meshes)
            {
                for (int i = 0; i < m.VertexCount; i++)
                {
                    result.Vertices.Add(m.Vertices[i]);

                    if (m.HasNormals)
                        result.Normals.Add(m.Normals[i]);
                }

                foreach (var f in m.Faces)
                {
                    var newFace = new Assimp.Face();
                    foreach (var idx in f.Indices)
                        newFace.Indices.Add(idx + vertexOffset);
                    result.Faces.Add(newFace);
                }

                vertexOffset += m.VertexCount;
            }

            return result;
        }

        private void checkBoxShowEdges_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxShowVertices_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool _updatingVertexUi = false;
        private bool _isUpdatingControls = false;
        const float TRACKBAR_SCALE = 0.05f;

        private void отключитьГлубинуДляВершинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            отключитьГлубинуДляВершинToolStripMenuItem.Checked = !отключитьГлубинуДляВершинToolStripMenuItem.Checked;
            glControl.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (_isUpdatingControls) return;
            _isUpdatingControls = true;

            float newX = trackBar1.Value * TRACKBAR_SCALE;
            numericVertexX.Value = (decimal)newX;

            if (model != null)
            {
                if (model.SelectedFaceIndex >= 0)
                {
                    float currentCenterX = model.GetFaceCenter(model.SelectedFaceIndex).X;
                    float deltaX = newX - currentCenterX;
                    model.MoveSelectedFace(new Vector3(deltaX, 0, 0));

                    trackBar1.Value = (int)(newX / TRACKBAR_SCALE);
                }
                else if (model.SelectedVertexIndex >= 0)
                {
                    float x = newX;
                    float y = (float)numericVertexY.Value;
                    float z = (float)numericVertexZ.Value;
                    model.MoveSelectedVertex(new Vector3(x, y, z));
                }
            }

            glControl.Invalidate();
            _isUpdatingControls = false;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (_isUpdatingControls) return;
            _isUpdatingControls = true;

            float newY = trackBar2.Value * TRACKBAR_SCALE;
            numericVertexY.Value = (decimal)newY;

            if (model != null)
            {
                if (model.SelectedFaceIndex >= 0)
                {
                    float currentCenterY = model.GetFaceCenter(model.SelectedFaceIndex).Y;
                    float deltaY = newY - currentCenterY;
                    model.MoveSelectedFace(new Vector3(0, deltaY, 0));

                    trackBar2.Value = (int)(newY / TRACKBAR_SCALE);
                }
                else if (model.SelectedVertexIndex >= 0)
                {
                    float x = (float)numericVertexX.Value;
                    float y = newY;
                    float z = (float)numericVertexZ.Value;
                    model.MoveSelectedVertex(new Vector3(x, y, z));
                }
            }

            glControl.Invalidate();
            _isUpdatingControls = false;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (_isUpdatingControls) return;
            _isUpdatingControls = false;

            float newZ = trackBar3.Value * TRACKBAR_SCALE;
            numericVertexZ.Value = (decimal)newZ;

            if (model != null)
            {
                if (model.SelectedFaceIndex >= 0)
                {
                    float currentCenterZ = model.GetFaceCenter(model.SelectedFaceIndex).Z;
                    float deltaZ = newZ - currentCenterZ;
                    model.MoveSelectedFace(new Vector3(0, 0, deltaZ));

                    trackBar3.Value = (int)(newZ / TRACKBAR_SCALE);
                }
                else if (model.SelectedVertexIndex >= 0)
                {
                    float x = (float)numericVertexX.Value;
                    float y = (float)numericVertexY.Value;
                    float z = newZ;
                    model.MoveSelectedVertex(new Vector3(x, y, z));
                }
            }

            glControl.Invalidate();
            _isUpdatingControls = false;
        }

        private void numericVertexX_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || _updatingVertexUi) return;
            if (model == null) return;
            if (model.SelectedVertexIndex < 0 || model.SelectedVertexIndex >= model.Mesh.Vertices.Count)
                return;

            _isUpdatingControls = true;
            trackBar1.Value = (int)numericVertexX.Value;
            _isUpdatingControls = false;

            float x = (float)numericVertexX.Value;
            float y = (float)numericVertexY.Value;
            float z = (float)numericVertexZ.Value;

            model.MoveSelectedVertex(new OpenTK.Vector3(x, y, z));
            glControl.Invalidate();
        }

        private void numericVertexY_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || _updatingVertexUi) return;
            if (model == null) return;
            if (model.SelectedVertexIndex < 0 || model.SelectedVertexIndex >= model.Mesh.Vertices.Count)
                return;

            _isUpdatingControls = true;
            trackBar2.Value = (int)numericVertexY.Value;
            _isUpdatingControls = false;

            float x = (float)numericVertexX.Value;
            float y = (float)numericVertexY.Value;
            float z = (float)numericVertexZ.Value;

            model.MoveSelectedVertex(new OpenTK.Vector3(x, y, z));
            glControl.Invalidate();
        }

        private void numericVertexZ_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || _updatingVertexUi) return;
            if (model == null) return;
            if (model.SelectedVertexIndex < 0 || model.SelectedVertexIndex >= model.Mesh.Vertices.Count)
                return;

            _isUpdatingControls = true;
            trackBar3.Value = (int)numericVertexZ.Value;
            _isUpdatingControls = false;

            float x = (float)numericVertexX.Value;
            float y = (float)numericVertexY.Value;
            float z = (float)numericVertexZ.Value;

            model.MoveSelectedVertex(new OpenTK.Vector3(x, y, z));
            glControl.Invalidate();
        }

        // Цвет
        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        // Добавить
        private void button3_Click(object sender, EventArgs e)
        {
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);

            if (model.SelectedFaceIndex >= 0)
            {
                Vector3 center = model.GetFaceCenter(model.SelectedFaceIndex);
                Vector3 normal = model.Mesh.GetFaceNormal(model.SelectedFaceIndex);
                Vector3 newPos = center + normal * 0.1f;

                Vector4 screenPos = Vector4.Transform(new Vector4(newPos, 1f), model.GetModelMatrix() * view * projection);
                screenPos /= screenPos.W;
                Point mousePos = new Point((int)(screenPos.X * glControl.Width * 0.5f + glControl.Width * 0.5f),
                                           (int)(-screenPos.Y * glControl.Height * 0.5f + glControl.Height * 0.5f));

                model.AddNewFaceAtMousePosition(mousePos, view, projection, glControl.Width, glControl.Height, null);
            }
            else
            {
                Point mousePos = glControl.PointToClient(Cursor.Position);
                model.AddNewFaceAtMousePosition(mousePos, view, projection, glControl.Width, glControl.Height, null);
            }

            glControl.Invalidate();
        }

        // Удалить
        private void button4_Click(object sender, EventArgs e)
        {
            if (model.SelectedFaceIndex >= 0)
            {
                // Удаление выбранной грани
                model.Mesh.Faces.RemoveAt(model.SelectedFaceIndex);
                model.Mesh.UpdateIndicesAndEdges();
                model.Buffers.UpdateAll(model.Mesh);
                model.SelectedFaceIndex = -1;
            }
            else
            {
                // Или по мыши
                Point mousePos = glControl.PointToClient(MousePosition);
                Matrix4 view = camera.GetViewMatrix();
                Matrix4 projection = camera.GetProjectionMatrix(glControl.Width, glControl.Height);
                int deleted = model.DeleteFaceAtMousePosition(mousePos, view, projection, glControl.Width, glControl.Height);
                if (deleted != -1)
                    MainForm.Log($"Удалена грань {deleted}");
            }
            glControl.Invalidate();
        }

        private void отключитьГлубинуДляГранейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            отключитьГлубинуДляГранейToolStripMenuItem.Checked = !отключитьГлубинуДляГранейToolStripMenuItem.Checked;
            glControl.Invalidate();
        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private Vector3 GetDragDeltaForMode(int screenDx, int screenDy)
        {
            float strength = 0.005f;

            if (_transformMode == "Камеры")
            {
                Vector3 right = camera.Right;
                Vector3 up = camera.Up;
                return right * screenDx * strength + up * (-screenDy * strength);
            }
            else if (_transformMode == "Локальной модели")
            {
                Matrix4 modelMatrix = model.GetModelMatrix();
                Matrix4 invModel = new Matrix4();
                Matrix4.Invert(ref modelMatrix, out invModel);
                Vector3 screenDelta = new Vector3(screenDx * strength, -screenDy * strength, 0);
                return Vector3.TransformPosition(screenDelta, invModel);
            }
            else if (_transformMode == "Дисплея")
            {
                return new Vector3(screenDx * strength, -screenDy * strength, 0);
            }
            else  
            {
                Vector3 right = camera.Right;
                Vector3 up = camera.Up;
                return right * screenDx * strength + up * (-screenDy * strength);
            }
        }

        private void управлятьОтносительноToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void управлятьОтносительноToolStripMenuItem_TextChanged(object sender, EventArgs e)
        {

        }

        private void управлятьОтносительноToolStripMenuItem_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void сцеплениеЭлементовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            сцеплениеЭлементовToolStripMenuItem.Checked = !сцеплениеЭлементовToolStripMenuItem.Checked;
            glControl.Invalidate();
        }
    }
}