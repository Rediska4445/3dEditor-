using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace WindowsFormsApp3
{
    public class SimpleLighting
    {
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 AmbientColor { get; set; }

        public Vector3 EmissiveColor { get; set; }
        public float EmissiveStrength { get; set; }
        public bool EnableEmissive { get; set; }

        private int shaderProgram;
        private int vertexShader;
        private int fragmentShader;

        private int modelLoc;
        private int viewLoc;
        private int projLoc;
        private int lightDirLoc;
        private int lightColorLoc;
        private int ambientColorLoc;
        private int baseColorLoc;
        private int emissiveColorLoc;
        private int emissiveStrengthLoc;
        private int enableEmissiveLoc;

        public SimpleLighting()
        {
            LightDirection = new Vector3(0.0f, 1.0f, 0.0f);
            LightColor = new Vector3(1.0f, 1.0f, 1.0f);
            AmbientColor = new Vector3(0.2f, 0.2f, 0.2f);

            EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
            EmissiveStrength = 1.0f;
            EnableEmissive = false;

            CreateShaders();
        }

        private void CreateShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec3 aNormal;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                out vec3 vNormal;
                out vec3 vFragPos;

                void main()
                {
                    vec4 worldPos = model * vec4(aPosition, 1.0);
                    vFragPos = worldPos.xyz;
                    vNormal = mat3(transpose(inverse(model))) * aNormal;

                    gl_Position = projection * view * worldPos;
                }";

            string fragmentShaderSource = @"
                #version 330 core
                in vec3 vNormal;
                in vec3 vFragPos;

                uniform vec3 lightDir;
                uniform vec3 lightColor;
                uniform vec3 ambientColor;
                uniform vec3 baseColor;
                
                uniform vec3 emissiveColor;
                uniform float emissiveStrength;
                uniform bool enableEmissive;

                out vec4 FragColor;

                void main()
                {
                    vec3 normal = normalize(vNormal);
                    vec3 light = normalize(-lightDir);

                    float diff = max(dot(normal, light), 0.0);
                    vec3 diffuse = lightColor * baseColor * diff;

                    vec3 result = ambientColor + diffuse;
                    
                    if (enableEmissive)
                    {
                        result += emissiveColor * emissiveStrength;
                    }

                    FragColor = vec4(result, 1.0);
                }";

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
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

            modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            lightDirLoc = GL.GetUniformLocation(shaderProgram, "lightDir");
            lightColorLoc = GL.GetUniformLocation(shaderProgram, "lightColor");
            ambientColorLoc = GL.GetUniformLocation(shaderProgram, "ambientColor");
            baseColorLoc = GL.GetUniformLocation(shaderProgram, "baseColor");

            emissiveColorLoc = GL.GetUniformLocation(shaderProgram, "emissiveColor");
            emissiveStrengthLoc = GL.GetUniformLocation(shaderProgram, "emissiveStrength");
            enableEmissiveLoc = GL.GetUniformLocation(shaderProgram, "enableEmissive");
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new InvalidOperationException($"Shader compile error: {info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new InvalidOperationException($"Program link error: {info}");
            }
        }

        public void Use(Matrix4 model, Matrix4 view, Matrix4 projection, Vector3 baseColor)
        {
            GL.UseProgram(shaderProgram);

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.Uniform3(lightDirLoc, LightDirection.X, LightDirection.Y, LightDirection.Z);
            GL.Uniform3(lightColorLoc, LightColor.X, LightColor.Y, LightColor.Z);
            GL.Uniform3(ambientColorLoc, AmbientColor.X, AmbientColor.Y, AmbientColor.Z);
            GL.Uniform3(baseColorLoc, baseColor.X, baseColor.Y, baseColor.Z);

            GL.Uniform3(emissiveColorLoc, EmissiveColor.X, EmissiveColor.Y, EmissiveColor.Z);
            GL.Uniform1(emissiveStrengthLoc, EmissiveStrength);
            GL.Uniform1(enableEmissiveLoc, EnableEmissive ? 1 : 0);
        }

        public void BindVertexAttributes()
        {
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
