using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

// TODO:
namespace cg_lab1
{
    public record struct VboLayout
    {
        public int Location;
        public int Size;
    }

    // Класс представляет VAO и связанные с ним VBO и EBO
    internal class VertexArray : IDisposable
    {
        public VertexArray()
        {
            Handle = GL.GenVertexArray();
            VboHandle = GL.GenBuffer();
            EboHandle = GL.GenBuffer();
        }

        public int Handle { get; set; }
        public int VboHandle { get; set; }
        public int EboHandle { get; set; }
        private int eboSize, vboSize;

        public void Dispose()
        {
            GL.DeleteVertexArray(Handle);
        }

        // Context is idempotent
        public VertexArrayContext Context()
        {
            return new VertexArrayContext(Handle);
        }

        public void PushVbo(float[] buffer)
        {
            using (Context())
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, VboHandle);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    buffer.Length * sizeof(float),
                    buffer,
                    BufferUsageHint.StaticDraw
                );
                vboSize = buffer.Length;
            }
        }

        public void PushEbo(uint[] buffer)
        {
            using (Context())
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EboHandle);
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    buffer.Length * sizeof(uint),
                    buffer,
                    BufferUsageHint.StaticDraw
                );
                eboSize = buffer.Length;
            }
        }

        public void DefineVboLayout(VboLayout[] layouts)
        {
            using (Context())
            {
                // Sum over all layouts
                int overallSize = layouts.Aggregate(0, (acc, x) => acc + x.Size);
                int accumulated = 0;
                foreach (VboLayout layout in layouts)
                {
                    GL.EnableVertexAttribArray(layout.Location);
                    GL.VertexAttribPointer(
                        layout.Location,
                        layout.Size,
                        VertexAttribPointerType.Float,
                        false,
                        overallSize * sizeof(float),
                        accumulated * sizeof(float)
                    );
                    accumulated += layout.Size;
                }
            }
        }

        public void DrawTriangles()
        {
            using (Context())
            {
                GL.DrawElements(PrimitiveType.Triangles, eboSize, DrawElementsType.UnsignedInt, 0);
            }
        }
    }

    internal class VertexArrayContext : IDisposable
    {
        public VertexArrayContext(int handle)
        {
            Handle = handle;
            GL.BindVertexArray(Handle);
        }

        int Handle { get; set; }

        public void Dispose()
        {
            GL.BindVertexArray(0);
        }
    }
}
