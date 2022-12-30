using Silk.NET.OpenGL;
using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TundraEngine
{
    public interface IGameWindow
    {
        public delegate void OnRenderHandler();
        public event OnRenderHandler OnRender;

        public delegate void OnLoadAssetsHandler();
        public event OnLoadAssetsHandler OnLoadAssets;

        public Renderer Renderer { get; set; }
        public GL Gl { get; set; }
        public bool IsInitialized { get; set; }
        public Game Game { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Scene Scene { get; set; }

        public void Initialize();
        public void PollEvents();
        public void Destroy();

    }
}
