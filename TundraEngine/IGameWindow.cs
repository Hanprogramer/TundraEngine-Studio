using Silk.NET.OpenGL;
using TundraEngine.Rendering;

namespace TundraEngine
{
    public interface IGameWindow
    {
        public Renderer Renderer { get; set; }
        public GL Gl { get; set; }
        public bool IsInitialized { get; set; }
        public Game Game { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Initialize();
        public void Update();
        public void Render();
        public void Destroy();
    }
}
