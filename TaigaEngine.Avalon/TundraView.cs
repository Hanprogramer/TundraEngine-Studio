using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using TundraEngine.Rendering;

namespace TundraEngine.Studio
{
    public class TundraView : OpenGlControlBase
    {
        Renderer? Renderer;

        private GL? Gl;

        protected override void OnOpenGlInit(GlInterface gl, int fb)
        {
            base.OnOpenGlInit(gl, fb);
            Gl = GL.GetApi(gl.GetProcAddress);
            Renderer = new Renderer(Gl);
        }


        protected override void OnOpenGlDeinit(GlInterface gl, int fb)
        {
            base.OnOpenGlDeinit(gl, fb);
        }

        protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
        {
            // Avalonia is not scaling properly on Windows, so have to manually does it
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var scaling = Win32Native.GetScreenScaling(this);
                Gl?.Viewport(0, 0, (uint)(Bounds.Width * scaling), (uint)(Bounds.Height * scaling));
            }
            else
                Gl?.Viewport(0, 0, (uint)(Bounds.Width), (uint)(Bounds.Height));

            this.ClipToBounds = true;


            Renderer?.Render();

            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }
    }
}
