using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using TundraEngine.Classes;
using TundraEngine.Rendering;
using Thread = System.Threading.Thread;
using Window = Silk.NET.Windowing.Window;

namespace TundraEngine
{
    /// <summary>
    /// A simple wrapper around Silk.NET window
    /// </summary>
    public class GameWindow : IGameWindow
    {

        public Game Game { get; set; }
        public int Width { get => _window.Size.X; set { } }
        public int Height { get => _window.Size.Y; set { } }
        public bool IsInitialized { get; set; } = false;
        public Rendering.Renderer Renderer { get; set; }
        public event IGameWindow.OnLoadAssetsHandler OnLoadAssets;
        public event IGameWindow.OnUpdateHandler OnUpdate;

        public GL Gl { get; set; }
        public Scene Scene { get; set; }

        public IGLContext context;
        public IInputContext input;

        public Thread? renderThread;
        bool renderOnSeparateThread = true;

        IWindow _window;
        public GameWindow(Game game, int width = 800, int height = 600)
        {
            Game = game;
            Width = width;
            Height = height;
            if (renderOnSeparateThread)
            {
                _window = Window.Create(WindowOptions.Default with
                {
                    Title = game.Title,
                    IsContextControlDisabled = true
                });
                _window.ShouldSwapAutomatically = false;
            }
            else
            {
                _window = Window.Create(WindowOptions.Default with
                {
                    Title = game.Title,
                    FramesPerSecond = 60,
                    UpdatesPerSecond = 120,
                    VSync = false
                });
                _window.Render += (dt) => _window_Render();
            }
            _window.Load += _window_Load;
            _window.Closing += _window_Closing;
            _window.Resize += _window_Resize;
            _window.Update += _window_Update;

            Scene = new Scene(this);
        }

        private void _window_Update(double obj)
        {
            Update((float)obj);
        }

        private void _window_Render()
        {
            if (_window != null)
                Gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);

            PollEvents();
            Renderer?.Clear();

            Renderer?.Begin();
            Scene.Render(Renderer);
            Renderer?.End();
        }

        private void Keyboard_KeyUp(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
        {
            Input.InputManager.Release((TundraEngine.Classes.Key)arg2);
        }

        private void Keyboard_KeyDown(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
        {
            Input.InputManager.Press((TundraEngine.Classes.Key)arg2);
        }

        private void _window_Resize(Silk.NET.Maths.Vector2D<int> size)
        {
            Width = size.X;
            Height = size.Y;
            Renderer.SetSize(size.X, size.Y);
        }

        private void _window_Closing()
        {
            Renderer.Dispose();
            Game.Quit();
            if (renderThread != null)
            {
                renderThread.Join();
            }
            _window.Reset();
        }

        private void _window_Load()
        {
            if (renderOnSeparateThread)
            {
                Console.WriteLine("Starting render thread");
                if (_window.GLContext != null)
                    context = _window.GLContext;

                context?.Clear();

                // Start the render thread
                renderThread = new System.Threading.Thread(() =>
                {
                    context?.MakeCurrent();
                    Gl = GL.GetApi(context);
                    Renderer = new Rendering.Renderer(this, Gl);
                    Renderer.SetSize(_window.Size.X, _window.Size.Y);
                    if (Scene == null) Scene = new Scene(this);
                    Scene.Initialize();

                    if (OnLoadAssets != null)
                    {
                        /// Wait until game is really running
                        while (!Game.IsRunning) { }
                        OnLoadAssets.Invoke(Renderer);
                    }
                    IsInitialized = true;

                    while (Game.IsRunning)
                    {
                        _window_Render();
                        _window?.SwapBuffers();
                    }
                });
                renderThread.Start();
            }
            else
            {
                Gl = GL.GetApi(_window);
                Renderer = new Rendering.Renderer(this, Gl);
                Renderer.SetSize(_window.Size.X, _window.Size.Y);
                if (Scene == null) Scene = new Scene(this);
                Scene.Initialize();
                OnLoadAssets?.Invoke(Renderer);
                IsInitialized = true;
            }
        }

        public void Initialize()
        {
            _window.Initialize();

            input = _window.CreateInput();
            foreach (var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
                keyboard.KeyUp += Keyboard_KeyUp;
            }
            _window.Run();
        }

        public void PollEvents()
        {
            if (renderOnSeparateThread)
                _window.DoEvents();
        }

        public void Destroy()
        {
            _window.Close();
        }

        public void Update(float dt)
        {
            if (IsInitialized)
                Scene.Update(dt);
        }

        public void SetIcon(string path)
        {
            var icon = TundraEngine.Rendering.Image.Load(path);
            _window.SetWindowIcon(ref icon);
        }
    }
}
