using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace cg_lab1
{
    internal class Game : GameWindow
    {
        Shader shader;
        VertexArray vao;

        const int N = 7;

        // Changable vars
        float rotationAngle = 0.0f;
        float deltaX = 0.0f, deltaY = 0.0f;
        float scale = 1.0f;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            CreateShaderProgram();

            float[] vertices;
            uint[] indices;
            Utils.GeneratePolygon(N, 0.5f, out vertices, out indices);
             
            vao = new VertexArray();
            vao.PushVbo(vertices);
            vao.PushEbo(indices);

            var positionLayout = new VboLayout
            {
                Location = shader.GetAttribLocation("aPosition"),
                Size = 2
            };

            VboLayout[] layouts = { positionLayout };

            vao.DefineVboLayout(layouts);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var keyState = KeyboardState;
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            {
                rotationAngle -= 0.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            {
                rotationAngle += 0.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {
                scale *= 1.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {
                scale /= 1.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
            {
                deltaX += 0.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
            {
                deltaX -= 0.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
            {
                deltaY -= 0.0005f;
            }
            if (keyState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down))
            {
                deltaY += 0.0005f;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();

            var modelMatrix = Matrix4.Identity *
                              Matrix4.CreateRotationZ(rotationAngle) *
                              Matrix4.CreateScale(scale) *
                              Matrix4.CreateTranslation(deltaX, deltaY, 0);

            shader.SetMatrix4("uModel", ref modelMatrix);

            vao.DrawTriangles();

            SwapBuffers();
        }

        private void CreateShaderProgram()
        {
            // Вершинный шейдер
            const string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec2 aPosition;

                uniform mat4 uModel;

                void main()
                {
                    gl_Position = uModel * vec4(aPosition, 0.0, 1.0);
                }
            ";

            // Фрагментный шейдер
            const string fragmentShaderSource = @"
                #version 330 core

                out vec4 FragColor; 

                void main()
                {
                    FragColor = vec4(1.0);
                }
            ";

            shader = new Shader(vertexShaderSource, fragmentShaderSource);
        }
    }
}
