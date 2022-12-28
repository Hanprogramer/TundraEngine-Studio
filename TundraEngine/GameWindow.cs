using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Silk.NET.Windowing;
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
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsInitialized { get; set; } = false;
        public Rendering.Renderer Renderer { get; set; }
        public GL Gl { get; set; }
        public IGLContext context;

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

            _window.ShouldSwapAutomatically = false;
            _window.Initialize();
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
                Renderer = new Rendering.Renderer(Gl);
                while (Game.IsRunning)
                {
                    if (_window != null)
                    {
                        _window.DoUpdate();
                        Gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
                    }
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
        }

        // Do nothing, we have a separate thread for render
        // this is used by another implementation of IGameWindow
        public void Render() { }

        public void Destroy()
        {
            _window.Close();
        }
    }
}
