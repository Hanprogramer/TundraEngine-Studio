using Silk.NET.OpenGL;

namespace TundraEngine.Rendering
{
    public class Shader : IDisposable
    {
        private uint _handle;
        private GL _gl;
        public static Shader FromFile(GL gl, string vertexPath, string fragmentPath)
        {
            return new Shader(gl, File.ReadAllText(vertexPath), File.ReadAllText(fragmentPath));
        }
        public Shader(GL gl, string vertexSource, string fragmentSource, string shaderName = "DefaultShader")
        {
            _gl = gl;

            uint vertex = CreateShaderFromSource(ShaderType.VertexShader, vertexSource, shaderName);
            uint fragment = CreateShaderFromSource(ShaderType.FragmentShader, fragmentSource, shaderName);
            _init(vertex, fragment);
        }

        private void _init(uint vertex, uint fragment)
        {
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }
            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public void Use()
        {
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name, int value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_handle);
        }

        private uint CreateShaderFromSource(ShaderType type, string content, string shaderName = "DefaultShader")
        {
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, content);
            _gl.CompileShader(handle);
            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                var message = $"[Shader {shaderName}] Error compiling shader of type {type}, failed with error {infoLog}";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
                throw new Exception(message);
            }

            return handle;
        }
    }
}
