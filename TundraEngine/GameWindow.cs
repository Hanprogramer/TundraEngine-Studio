using Arch.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using System.Security.Cryptography.X509Certificates;
using TundraEngine.Components;
using TundraEngine.Rendering;
using Window = Silk.NET.Windowing.Window;

namespace TundraEngine
{
    /// <summary>
    /// A simple wrapper around Silk.NET window
    /// </summary>
    public class GameWindow : IGameWindow
    {

        public Game Game { get; set; }
        public int Width { get => _window.Size.X; set {  } }
        public int Height { get => _window.Size.Y; set {  } }
        public bool IsInitialized { get; set; } = false;
        public Rendering.Renderer Renderer { get; set; }
        public event IGameWindow.OnRenderHandler OnRender;
        public GL Gl { get; set; }
        public World World { get; set; }

        public IGLContext context;
        int ticks = 0;

        IWindow _window;
        public GameWindow(Game game, int width = 800, int height = 600)
        {
            Game = game;
            Width = width;
            Height = height;
            _window = Window.Create(WindowOptions.Default with
            {
                Title = game.Title,
                IsContextControlDisabled = true
            });
            _window.Load += _window_Load;
            _window.Closing += _window_Closing;
            _window.Resize += _window_Resize;

            _window.ShouldSwapAutomatically = false;

            _window.Initialize();
            World = World.Create();
        }

        private void _window_Resize(Silk.NET.Maths.Vector2D<int> size)
        {
            Width = size.X;
            Height = size.Y;
            Renderer.SetSize(size.X, size.Y);
        }

        private void _window_Closing()
        {
            Console.WriteLine("Window Closing");
            Renderer.Dispose();
            Game.IsRunning = false;
        }
        private void _window_Load()
        {
            Console.WriteLine("Starting render thread");
            if (_window.GLContext != null)
                context = _window.GLContext;

            context?.Clear();

            // Start the render thread
            var thread = new System.Threading.Thread(() =>
            {
                context?.MakeCurrent();
                Gl = GL.GetApi(context);
                Renderer = new Rendering.Renderer(this, Gl);
                Renderer.SetSize(_window.Size.X, _window.Size.Y);
                while (Game.IsRunning)
                {
                    if (_window != null)
                    {
                        _window.DoUpdate();
                        Gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
                    }
                    Renderer?.Clear();
                    RenderWorld();
                    // Render the entire batch
                    Renderer?.Render();
                    context?.SwapBuffers();
                }
            });
            thread.Start();
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void Update()
        {
            _window.DoEvents();

            var query = new QueryDescription { All = new Arch.Core.Utils.ComponentType[] { typeof(Transform) } };
            World.Query(in query, (ref Transform transform) =>
            {
                ticks++;
                transform.X = (MathF.Sin((ticks/10 + transform.Y * 100000) / 1000000f) * 100f);
            });
        }

        public void Destroy()
        {
            _window.Close();
        }

        

        public void RenderWorld()
        {
            // Rendering everything 
            Renderer.Begin();
            var renderQuery = new QueryDescription { All = new Arch.Core.Utils.ComponentType[] { typeof(Transform), typeof(Sprite) } };
            World.Query(in renderQuery, (ref Transform transform, ref Sprite sprite) =>
            {
                Renderer.DrawSprite(sprite, transform);
            });
            Renderer.End();
        }
    }
}
