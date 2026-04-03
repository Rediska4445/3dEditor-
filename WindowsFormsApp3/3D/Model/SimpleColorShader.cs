using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace WindowsFormsApp3
{
    public class SimpleColorShader : IDisposable
    {
        public int Program { get; private set; }

        public SimpleColorShader()
        {
            Setup();
        }

        private void Setup()
        {
            string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform vec4 drawColor;
            out vec4 fColor;
            void main()
            {
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
                fColor = drawColor;
            }";

            string fragmentShaderSource = @"
            #version 330 core
            in vec4 fColor;
            out vec4 FragColor;
            void main()
            {
                FragColor = fColor;
            }";

            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertexShaderSource);
            GL.CompileShader(vs);
            CheckShaderCompile(vs);

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragmentShaderSource);
            GL.CompileShader(fs);
            CheckShaderCompile(fs);

            Program = GL.CreateProgram();
            GL.AttachShader(Program, vs);
            GL.AttachShader(Program, fs);
            GL.LinkProgram(Program);
            CheckProgramLink(Program);

            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
        }

        private static void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
                throw new Exception("Shader compile error: " + GL.GetShaderInfoLog(shader));
        }

        private static void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
                throw new Exception("Program link error: " + GL.GetProgramInfoLog(program));
        }

        public void Use(Matrix4 model, Matrix4 view, Matrix4 projection, Color4 color)
        {
            GL.UseProgram(Program);
            int modelLoc = GL.GetUniformLocation(Program, "model");
            int viewLoc = GL.GetUniformLocation(Program, "view");
            int projLoc = GL.GetUniformLocation(Program, "projection");
            int colorLoc = GL.GetUniformLocation(Program, "drawColor");

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.Uniform4(colorLoc, color);
        }

        public void Stop()
        {
            GL.UseProgram(0);
        }

        public void Dispose()
        {
            if (Program != 0)
            {
                GL.DeleteProgram(Program);
                Program = 0;
            }
        }
    }

}
