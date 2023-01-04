using Avalonia;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TundraEngine.Studio
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

        protected override void OnOpenGlInit(GlInterface gl, int fb)
        {
            base.OnOpenGlInit(gl, fb);
            Gl = GL.GetApi(gl.GetProcAddress);
            Scene = new Scene(this);
            Renderer = new Renderer(this, Gl);
            Game = new TestGame1Game(this);

            Game.OnStart();
            OnLoadAssets?.Invoke(Renderer);
            Game.Start();
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
                Scene.FindObject<Camera>().FlipY = true;
            }

            this.ClipToBounds = true;

            var bound = GetActualBounds();
            Renderer?.Clear();
            Gl?.Viewport(0, 0, (uint)(bound.Width), (uint)(bound.Height));

            Renderer?.Begin();
            Scene.Render(Renderer);
            Renderer?.End();

            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }

        public void Update(float dt)
        {
            if ((this as IGameWindow).IsInitialized)
                Scene.Update(dt);
        }

        public void Initialize()
        {
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
                Input.InputManager.Press((TundraEngine.Classes.Key)tundraKey!);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            var tundraKey = TundraAvalon.TranslateAvaloniaKeys(e.Key);
            if (tundraKey != null)
                Input.InputManager.Release((TundraEngine.Classes.Key)tundraKey!);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            this.Focus();
        }
    }
}
