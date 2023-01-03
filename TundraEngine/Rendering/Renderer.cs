using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TundraEngine.Classes;
using TundraEngine.Components;

namespace TundraEngine.Rendering
{
    public enum RendererFilter {
        Nearest,
        Linear
    }
    public class Renderer
    {
        // OpenGL stuffs
        public GL Gl;

        // Buffers
        private BufferObject<float> Vbo;
        private BufferObject<uint> Ebo;
        private VertexArrayObject<float, uint> Vao;

        // Default Shader
        private Shader Shader;

        // Windowing
        public IGameWindow Window;

        // Render stuffs
        public Camera Camera;
        public Scene Scene;
        public Texture lastTexture;

        public RendererFilter RendererFilter = RendererFilter.Nearest;
        public int RenderWidth = 1, RenderHeight = 1;
        public float PixelWidth = 1, PixelHeight = 1;

        // Drawing state
        int quadsCount = 0;
        bool isDrawing = false;

        private float[] Vertices = new float[32752];
        //{
        //    //X    Y      U   V
        //     1f,  1f,     1f, 0f,
        //     1f, -1f,     1f, 1f,
        //    -1f, -1f,     0f, 1f,
        //    -1f,  1f,     0f, 0f
        //};

        private uint[] Indices = new uint[12282];
        //{
        //    0, 1, 3,
        //    1, 2, 3
        //};

        private int maxQuadsCount = 2047; // 32752 / 16
        private uint vertId = 0;
        private uint indId = 0;
        private int drawCalls = 0;


        public Renderer(IGameWindow window, GL gl)
        {
            Gl = gl;
            Window = window;
            Camera = (Camera)window.Scene.AddObject(new Camera(window.Scene));
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

        public unsafe void Clear()
        {

            Gl.ClearColor(0.94f, 0.79f, 0.64f, 1.0f);
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

        }

        public unsafe void Render()
        {
            if (!isDrawing) throw new Exception("Renderer.Begin must be called before starting to draw");
            Ebo.Bind();
            Vbo.Bind();
            Vao.Bind();
            Shader.Use();

            Shader.SetUniform("uTexture0", 0);
            //lastTexture.Bind();

            Shader.SetUniform("uProjection", Camera.ProjectionMatrix);
            Gl.DrawElements(PrimitiveType.Triangles, indId, DrawElementsType.UnsignedInt, null);

            drawCalls++;
        }


        public void Begin()
        {
            if (isDrawing) throw new Exception("Renderer.End must be called before starting another one");
            isDrawing = true;
            quadsCount = 0;
            vertId = 0;
            indId = 0;
            drawCalls = 0;
            lastTexture = null;
        }

        public void Flush()
        {
            Vbo.UpdateData(Vertices, vertId);
            Ebo.UpdateData(Indices, indId);
            Vao.UpdateData(Vbo, Ebo);

            Render();
            quadsCount = 0;
            vertId = 0;
            indId = 0;
        }

        public void End()
        {
            if (!isDrawing) throw new Exception("Renderer.Begin must be called before ending one");
            if (quadsCount > 0)
                Flush();
            lastTexture = null;
            isDrawing = false;
        }

        private void AddQuad(Transform transform)
        {
            float x1 = transform.X - transform.Width / 2;
            float y1 = transform.Y - transform.Height / 2;
            float x2 = transform.X + transform.Width / 2;
            float y2 = transform.Y + transform.Height / 2;

            // Top Right
            Vertices[vertId++] = x2;  // X
            Vertices[vertId++] = y2;  // Y
            Vertices[vertId++] = 1;  // U
            Vertices[vertId++] = 0;  // V

            // Bottom Right
            Vertices[vertId++] = x2;  // X
            Vertices[vertId++] = y1; // y
            Vertices[vertId++] = 1;  // U
            Vertices[vertId++] = 1;  // V

            // Bottom Left
            Vertices[vertId++] = x1; // X
            Vertices[vertId++] = y1; // Y
            Vertices[vertId++] = 0;  // U
            Vertices[vertId++] = 1;  // V

            // Top Left
            Vertices[vertId++] = x1; // X
            Vertices[vertId++] = y2;  // Y
            Vertices[vertId++] = 0;  // U
            Vertices[vertId++] = 0;  // V

            // Indices
            uint dc = (uint)(quadsCount * 4);
            Indices[indId++] = 0 + dc;
            Indices[indId++] = 1 + dc;
            Indices[indId++] = 3 + dc;

            Indices[indId++] = 1 + dc;
            Indices[indId++] = 2 + dc;
            Indices[indId++] = 3 + dc;
            quadsCount++;

        }

        public unsafe void DrawTexture(Texture texture, Transform transform)
        {

            if (lastTexture != texture)
            {
                Flush();
                lastTexture = texture;
                lastTexture.Bind();
            }
            else 
            if (quadsCount >= maxQuadsCount)
                Flush();
            
            AddQuad(transform);
        }

        public void DrawSprite(SpriteRenderer sprite, Transform transform)
        {
            throw new NotImplementedException();
            if (quadsCount >= maxQuadsCount)
            {
                Flush();
            }
            AddQuad(transform);
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
