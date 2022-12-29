using Arch.Core;
using Silk.NET.OpenGL;
using TundraEngine.Rendering;

namespace TundraEngine
{
    public interface IGameWindow
    {
        public delegate void OnRenderHandler();
        public event OnRenderHandler OnRender;
        public Renderer Renderer { get; set; }
        public GL Gl { get; set; }
        public bool IsInitialized { get; set; }
        public Game Game { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public World World { get; set; }

        public void Initialize();
        public void Update();
        public void Destroy();
        public void RenderWorld();
    }
}
