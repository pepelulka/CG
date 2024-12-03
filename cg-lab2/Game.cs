using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace cg_lab2
{
    internal class Game : GameWindow
    {
        Shader shader;
        VertexArray vao;

        Vector3 viewPos = new(-5.0f, 1f, -1f);
        Vector3 viewTarget = new(0.0f, 0.0f, 0.0f);

        // Задание:
        bool isPerspective = true;
        float indicator = 0.0f; // 0 - perspective, 1 - ortho
        float indicatorChangeVelocity = 2f; // Измеряется в с^-1

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        void InitVao(Shader targetShader)
        {
            // Мне лень их выравнивать...
            float[] vertices = {
            // Позиции          // Цвет
            // Передняя грань
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f, 
                0.5f, -0.5f,  0.5f,  0f, 1f, 0f, 
                0.5f,  0.5f,  0.5f,  1f, 0f, 0f, 
            -0.5f,  0.5f,  0.5f,  1f, 1f, 1f, 

            // Задняя грань
            -0.5f, -0.5f, -0.5f,  0f, 0f, 1f,
                0.5f, -0.5f, -0.5f,  0f, 1f, 0f,
                0.5f,  0.5f, -0.5f,  1f, 0f, 0f,
            -0.5f,  0.5f, -0.5f,  1f, 1f, 1f,
    
            // Левая грань
            -0.5f, -0.5f, -0.5f,  0f, 0f, 1f,
            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f,
            -0.5f,  0.5f,  0.5f,  1f, 0f, 0f,
            -0.5f, -0.5f,  0.5f,  1f, 1f, 1f,

            // Правая грань
                0.5f, -0.5f, -0.5f,  0f, 0f, 1f,
                0.5f,  0.5f, -0.5f,  0f, 1f, 0f,
                0.5f,  0.5f,  0.5f,  1f, 0f, 0f,
                0.5f, -0.5f,  0.5f,  1f, 1f, 1f,

            // Нижняя грань
            -0.5f, -0.5f, -0.5f,  0f, 0f, 1f,
                0.5f, -0.5f, -0.5f,  0f, 1f, 0f,
                0.5f, -0.5f,  0.5f,  1f, 0f, 0f,
            -0.5f, -0.5f,  0.5f,  1f, 1f, 1f,

            // Верхняя грань
            -0.5f,  0.5f, -0.5f,  0f, 0f, 1f,
                0.5f,  0.5f, -0.5f,  0f, 1f, 0f,
                0.5f,  0.5f,  0.5f,  1f, 0f, 0f,
            -0.5f,  0.5f,  0.5f,  1f, 1f, 1f,
             };

                // Индексы для куба
            uint[] indices = {
                0, 1, 2, 2, 3, 0,   // Передняя грань
                4, 5, 6, 6, 7, 4,   // Задняя грань
                8, 9, 10, 10, 11, 8, // Левая грань
                12, 13, 14, 14, 15, 12, // Правая грань
                16, 17, 18, 18, 19, 16, // Нижняя грань
                20, 21, 22, 22, 23, 20  // Верхняя грань
            };

            vao = new VertexArray(vertices, indices);

            var positionLayout = new VboLayout
            {
                Location = targetShader.GetAttribLocation("aPosition"),
                Size = 3
            };

            var colorLayout = new VboLayout
            {
                Location = targetShader.GetAttribLocation("aColor"),
                Size = 3
            };

            VboLayout[] layouts = { positionLayout, colorLayout };

            vao.DefineVboLayout(layouts);
        }

        static Matrix4 Interpolate(Matrix4 m1, Matrix4 m2, float t)
        {
            Matrix4 result = new();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = m1[i, j] * (1 - t) + m2[i, j] * t;
                }
            }
            return result;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.4f, 0.5f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            CreateShaderProgram();
            InitVao(shader);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            float dt = (float)e.Time;
            if (isPerspective)
            {
                indicator -= dt * indicatorChangeVelocity;
            } else
            {
                indicator += dt * indicatorChangeVelocity;
            }
            indicator = Math.Clamp(indicator, 0.0f, 1.0f);

            var keyState = KeyboardState;
            if (keyState.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.C))
            {
                isPerspective = !isPerspective;
            }
        }

        void SetupMatricesAndUniforms(Shader targetShader)
        {
            Matrix4 view = Matrix4.LookAt(
                viewPos,
                viewTarget,
                new Vector3(0.0f, 1.0f, 0.0f)
            );
            Matrix4 projectionPerspective = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)Size[1] / Size[0], 0.1f, 10.0f);
            Matrix4 projectionOrtho = Matrix4.CreateOrthographicOffCenter(-5f, 5f, -5f, 5f, 0.1f, 10.0f);

            Matrix4 projection = Interpolate(projectionPerspective, projectionOrtho, indicator);

            targetShader.Use();

            targetShader.SetMatrix4("uProjection", ref projection);
            targetShader.SetMatrix4("uView", ref view);
        }

        void RenderScene(Shader targetShader)
        {
            targetShader.Use();
            Matrix4 model = Matrix4.Identity;
            targetShader.SetMatrix4("uModel", ref model);
            vao.DrawTriangles();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shader.Use();

            SetupMatricesAndUniforms(shader);
            RenderScene(shader);

            SwapBuffers();
        }

        private void CreateShaderProgram()
        {
            // Вершинный шейдер
            const string vertexShaderSource = @"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 color;

void main()
{
    color = aColor;
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}
            ";

            // Фрагментный шейдер
            const string fragmentShaderSource = @"
#version 330 core

in vec3 color;           

out vec4 FragColor; 

void main()
{
    FragColor = vec4(color, 1.0);
}
            ";

            shader = new Shader(vertexShaderSource, fragmentShaderSource);
        }
    }
}
