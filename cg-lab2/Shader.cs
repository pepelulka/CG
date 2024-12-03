using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace cg_lab2
{

    // Класс представляет собой скомпилированные вершинный и фрагментарный шейдеры.
    internal class Shader : IDisposable
    {
        public int Handle { get; set; }
        private bool disposed = false;

        // Constructors & destructors
        public Shader(string vertexText, string fragmentText)
        {
            // Компиляция шейдеров
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexText);
            GL.CompileShader(vertexShader);
            CheckShaderCompileStatus(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentText);
            GL.CompileShader(fragmentShader);
            CheckShaderCompileStatus(fragmentShader);

            // Линковка
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLinkStatus(Handle);

            // Удаление исходников шейдеров
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader); // Удаляем вершинный шейдер
            GL.DeleteShader(fragmentShader); // Удаляем фрагментный шейдеры
        }

        static private void CheckShaderCompileStatus(int shader) // проверка статуса компиляции шейдера
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new System.Exception($"Shader compilation error: {infoLog}");
            }
        }

        static private void CheckProgramLinkStatus(int program) // проверка статуса линковки программы
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new System.Exception($"Shader linking error: {infoLog}");
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                GL.DeleteProgram(Handle);
                disposed = true;
            }
        }

        ~Shader()
        {
            if (disposed == false)
            {
                Console.Error.WriteLine("Shader class: GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        // Useful functions
        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void SetMatrix4(string name, ref Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetFloat(string name, ref float value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
        }

    }
}
