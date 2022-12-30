using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TundraEngine.Components;

namespace TundraEngine.Rendering
{
    public enum RendererFilter {
        Nearest,
        Linear
    }
    public class Renderer
    {
        public GL Gl;


        private BufferObject<float> Vbo;
        private BufferObject<uint> Ebo;
        private VertexArrayObject<float, uint> Vao;
        private Shader Shader;

        public IGameWindow Window;
        public Camera camera;
        public Texture lastTexture;

        public RendererFilter RendererFilter = RendererFilter.Nearest;
        public int RenderWidth = 1, RenderHeight = 1;
        public float PixelWidth = 1, PixelHeight = 1;

        int drawCalls = 0;
        bool isDrawing = false;

        private float[] Vertices = new float[4048];
        //{
        //    //X    Y      U   V
        //     1f,  1f,     1f, 0f,
        //     1f, -1f,     1f, 1f,
        //    -1f, -1f,     0f, 1f,
        //    -1f,  1f,     0f, 0f
        //};

        private uint[] Indices = new uint[4048];
        //{
        //    0, 1, 3,
        //    1, 2, 3
        //};
        private int maxDrawCalls = 253; // 4048 / 15
        private uint vertId = 0;
        private uint indId = 0;



        public Renderer(IGameWindow window, GL gl)
        {
            Gl = gl;
            Window = window;
            camera = new Camera(window);
            Initialize();
        }

        public void Initialize()
        {

            //Instantiating our new abstractions
            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer, 0);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer, 0);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

            //Telling the VAO object how to lay out the attribute pointers
            Vao.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 4, 0); // XY
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 4, 2); // UV

            Shader = new Shader(Gl, DefaultShader.Vertex, DefaultShader.Fragment);

        }

        public Renderer(IWindow window)
        {
            Gl = GL.GetApi(window);
            Initialize();
        }

        public unsafe void Clear()
        {

            Gl.ClearColor(System.Drawing.Color.Green);
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
        }

        public unsafe void Render()
        {

            Ebo.Bind();
            Vbo.Bind();
            Vao.Bind();
            Shader.Use();

            Shader.SetUniform("uTexture0", 0);

            Shader.SetUniform("uProjection", camera.ProjectionMatrix);
            Gl.DrawElements(PrimitiveType.Triangles, indId, DrawElementsType.UnsignedInt, null);
        }

        public void SetupMatrix()
        {
        }

        public void Begin()
        {
            if (isDrawing) throw new Exception("Renderer.End must be called before starting another one");
            isDrawing = true;
            drawCalls = 0;
            vertId = 0;
            indId = 0;
            SetupMatrix();
        }

        public void Flush()
        {
            Vbo.UpdateData(Vertices, vertId);
            Ebo.UpdateData(Indices, indId);
            Vao.UpdateData(Vbo, Ebo);
            
            Render();
            drawCalls = 0;
            vertId = 0;
            indId = 0;
        }

        public void End()
        {
            if (!isDrawing) throw new Exception("Renderer.Begin must be called before ending one");
            isDrawing = false;
            if (drawCalls > 0)
                Flush();
        }

        private void AddQuad(Transform transform)
        {
            // Multiplication by zero sometimes causes infinity
            if (double.IsInfinity(transform.X))
            {
                transform.X = 0;
            }
            if (double.IsInfinity(transform.Y))
            {
                transform.Y = 0;
            }

            // Top Right
            Vertices[vertId++] = transform.X + transform.Width;  // X
            Vertices[vertId++] = transform.Y + transform.Height;  // Y
            Vertices[vertId++] = 1;  // U
            Vertices[vertId++] = 0;  // V

            // Bottom Right
            Vertices[vertId++] = transform.X + transform.Width;  // X
            Vertices[vertId++] = transform.Y; // y
            Vertices[vertId++] = 1;  // U
            Vertices[vertId++] = 1;  // V

            // Bottom Left
            Vertices[vertId++] = transform.X; // X
            Vertices[vertId++] = transform.Y; // Y
            Vertices[vertId++] = 0;  // U
            Vertices[vertId++] = 1;  // V

            // Top Left
            Vertices[vertId++] = transform.X; // X
            Vertices[vertId++] = transform.Y + transform.Height;  // Y
            Vertices[vertId++] = 0;  // U
            Vertices[vertId++] = 0;  // V

            // Indices
            uint dc = (uint)(drawCalls * 4);
            Indices[indId++] = 0 + dc;
            Indices[indId++] = 1 + dc;
            Indices[indId++] = 3 + dc;

            Indices[indId++] = 1 + dc;
            Indices[indId++] = 2 + dc;
            Indices[indId++] = 3 + dc;

        }

        public unsafe void DrawTexture(Texture texture, Transform position)
        {
            if (drawCalls >= maxDrawCalls)
            {
                Flush();
            }

            AddQuad(position);

            drawCalls++;
            if (lastTexture != texture)
            {
                texture.Bind();
                Flush();
            }
        }

        public void DrawSprite(Sprite sprite, Transform position)
        {
            if (drawCalls >= maxDrawCalls)
            {
                Flush();
            }
            AddQuad(position);

            drawCalls++;
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
            PixelWidth = 1f / width;
            PixelHeight = 1f / height;
        }
    }
}
