using Avalonia;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;
using TundraEngine.Classes;
using TundraEngine.Rendering;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Controls
{
    public class TundraView : OpenGlControlInputBase, IGameWindow
    {
        Renderer? Renderer;

        private GL? Gl;

        Renderer IGameWindow.Renderer { get; set; }
        GL IGameWindow.Gl { get; set; }
        bool IGameWindow.IsInitialized { get; set; } = false;
        public Game Game { get; set; }
        int IGameWindow.Width { get => (int)GetActualBounds().Width; set { } }
        int IGameWindow.Height { get => (int)GetActualBounds().Height; set { } }
        public Scene Scene { get; set; }

        public event IGameWindow.OnLoadAssetsHandler OnLoadAssets;
        public event IGameWindow.OnUpdateHandler OnUpdate;

        public bool GameStarted = false;

        public delegate void OnGameStartedHandler();
        public event OnGameStartedHandler? OnGameStarted;

        protected override void OnOpenGlInit(GlInterface gl, int fb)
        {
            base.OnOpenGlInit(gl, fb);

            Gl = GL.GetApi(gl.GetProcAddress);
            Renderer = new Renderer(this, Gl);

            if (!GameStarted)
            {
                Scene = new Scene(this);
                Start();
                OnGameStarted?.Invoke();
            }
        }

        public void Start()
        {
            if (Game == null)
            {
                Console.WriteLine("Game is null");
                return;
            }
            GameStarted = true;
            Game.OnStart();
            OnLoadAssets?.Invoke(Renderer);
            Game.Start();
        }

        public void Stop()
        {
            Game.Quit();
            Game = null;
        }


        protected override void OnOpenGlDeinit(GlInterface gl, int fb)
        {
            base.OnOpenGlDeinit(gl, fb);
        }



        public Rect GetActualBounds()
        {
            // Avalonia is not scaling properly on Windows, so have to manually does it
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var scaling = Win32Native.GetScreenScaling(this);
                return Bounds * scaling;
            }
            else
                return Bounds;
        }

        protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
        {
            if (!(this as IGameWindow).IsInitialized)
            {
                (this as IGameWindow).IsInitialized = true;
                // Flips the Camera on Y Axis, idk why though
                var cam = Scene.FindObjectOrNull<Camera>();
                if (cam != null)
                    cam.FlipY = true;
                ClipToBounds = true;
            }
            try
            {
                var bound = GetActualBounds();
                Renderer?.Clear();
                Gl?.Viewport(0, 0, (uint)bound.Width, (uint)bound.Height);

                Renderer?.Begin();
                Scene.Render(Renderer);
                Renderer?.End();

                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Update(float dt)
        {
            if ((this as IGameWindow).IsInitialized)
                Scene.Update(dt);
        }

        public void Initialize()
        {
            Console.WriteLine("Initializing Scene");
            Scene.Initialize();
        }

        public void PollEvents()
        {
            //throw new System.NotImplementedException();
        }

        public void Destroy()
        {
            //throw new System.NotImplementedException();
        }

        public void SetIcon(string path)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var tundraKey = TundraAvalon.TranslateAvaloniaKeys(e.Key);
            if (tundraKey != null)
                Input.InputManager.Press((Classes.Key)tundraKey!);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            var tundraKey = TundraAvalon.TranslateAvaloniaKeys(e.Key);
            if (tundraKey != null)
                Input.InputManager.Release((Classes.Key)tundraKey!);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            Focus();
        }
    }
}
