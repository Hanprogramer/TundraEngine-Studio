using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TundraEngine.Components;

namespace TundraEngine.Rendering
{
    public class Renderer
    {
        public GL Gl;


        private BufferObject<float> Vbo;
        private BufferObject<uint> Ebo;
        private VertexArrayObject<float, uint> Vao;
        private Shader Shader;
        private Texture Texture;

        public int RenderWidth = 1, RenderHeight = 1;
        public float PixelWidth = 1, PixelHeight = 1;

        int drawCalls = 0;
        bool isDrawing = false;

        private float[] Vertices =
        {
            //X    Y      Z     U   V
             1f,  1f, 0.0f,     1f, 0f,
             1f, -1f, 0.0f,     1f, 1f,
            -1f, -1f, 0.0f,     0f, 1f,
            -1f,  1f, 0.0f,     0f, 0f
        };

        private uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        private uint idx = 0;



        public Renderer(GL gl)
        {
            Gl = gl;
            Initialize();
        }

        public void Initialize()
        {

            //Instantiating our new abstractions
            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

            //Telling the VAO object how to lay out the attribute pointers
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            Shader = new Shader(Gl, DefaultShader.Vertex, DefaultShader.Fragment);

            Texture = new Texture(Gl, "Assets/silk.png");

        }

        public Renderer(IWindow window)
        {
            Gl = GL.GetApi(window);
            Initialize();
        }

        public unsafe void Render()
        {
            Gl.ClearColor(System.Drawing.Color.Green);
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

            Ebo.Bind();
            Vbo.Bind();
            Vao.Bind();
            Shader.Use();

            Texture.Bind(TextureUnit.Texture0);
            Shader.SetUniform("uTexture0", 0);

            Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        public void SetupMatrix()
        {

        }

        public void Begin()
        {
            if (isDrawing) throw new Exception("Renderer.End must be called before starting another one");
            isDrawing = true;
            drawCalls = 0;
            idx = 0;
            SetupMatrix();
        }

        public void Flush()
        {
        }

        public void End()
        {
            if (!isDrawing) throw new Exception("Renderer.Begin must be called before ending one");
            isDrawing = true;
            if (drawCalls > 0)
                Flush();
        }

        public unsafe void DrawTexture(Texture texture, Transform position)
        {

        }

        public void DrawSprite(Sprite sprite, Transform position)
        {

        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Sets the correct resolution for the renderer
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height)
        {
            RenderWidth = width;
            RenderHeight = height;
            PixelWidth = 1 / width;
            PixelHeight = 1 / height;
        }
    }
}
