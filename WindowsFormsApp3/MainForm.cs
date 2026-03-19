using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        private int vao, vboVertices, vboIndices, ebo;
        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();
        private float cameraDistance = 5f;
        private float angleX = 0f, angleY = 0f;
        private Point lastMousePos;

        private float modelAngleX = 0f;
        private float modelAngleY = 0f;

        public MainForm()
        {
            InitializeComponent();
            SetupGLControl();
        }

        private void SetupGLControl()
        {
            glControl.Load += (s, e) => GlControl_Load();
            glControl.Paint += (s, e) => GlControl_Paint();
            glControl.MouseDown += (s, e) => { lastMousePos = e.Location; };
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.Resize += GlControl_Resize; // добавьте это
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
            SetupShaders();

            // Инициализация VAO/VBO
            vao = GL.GenVertexArray();
            vboVertices = GL.GenBuffer();
            vboIndices = GL.GenBuffer();
        }

        private int shaderProgram;

        private void SetupShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(0.8, 0.8, 0.8, 1.0);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            CheckProgramLink(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                MessageBox.Show($"Shader compile error: {info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                MessageBox.Show($"Program link error: {info}");
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "OBJ|*.obj" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LoadModel(dlg.FileName);
                    SetupBuffers();
                    glControl.Invalidate();
                }
            }
        }

        private void LoadModel(string path)
        {
            vertices.Clear();
            indices.Clear();

            var context = new AssimpContext();
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);

            if (scene.Meshes.Count > 0)
            {
                var mesh = scene.Meshes[0];

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    var v = mesh.Vertices[i];
                    vertices.Add(v.X);
                    vertices.Add(v.Y);
                    vertices.Add(v.Z);
                }

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    if (face.IndexCount == 3)
                    {
                        indices.Add((uint)face.Indices[0]);
                        indices.Add((uint)face.Indices[1]);
                        indices.Add((uint)face.Indices[2]);
                    }
                }
            }
        }

        private void SetupBuffers()
        {
            GL.BindVertexArray(vao);

            // Вершины
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Индексы
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        private void GlControl_Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);

            // Матрицы камеры
            var view = Matrix4.CreateTranslation(0, 0, -cameraDistance) * Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)glControl.Width / glControl.Height, 0.1f, 100f);

            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");

            // Матрица вращения модели
            var model = Matrix4.CreateRotationX(modelAngleX) * Matrix4.CreateRotationY(modelAngleY);

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            // Рисуем модель
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            glControl.SwapBuffers();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float deltaX = e.X - lastMousePos.X;
                float deltaY = e.Y - lastMousePos.Y;

                angleY += deltaX * 0.01f;
                angleX += deltaY * 0.01f;
            }
            else if (e.Button == MouseButtons.Right)
            {
                float deltaX = e.X - lastMousePos.X;
                float deltaY = e.Y - lastMousePos.Y;
                modelAngleY += deltaX * 0.01f;
                modelAngleX += deltaY * 0.01f;
            }

            lastMousePos = e.Location;
            glControl.Invalidate();
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