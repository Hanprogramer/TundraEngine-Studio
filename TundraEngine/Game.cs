using Arch.Core;
using Silk.NET.SDL;
using TundraEngine.Components;

namespace TundraEngine
{
    public class Game
    {
        public string Title { get; }
        public string Version { get; }
        public string BuildNumber { get; }

        public IGameWindow Window;
        public World World { get => Window.World; }
        public bool IsRunning = false;

        public delegate void OnUpdateHandler(double dt);

        public OnUpdateHandler OnUpdate;
        public Event OnRender;
        public Event OnInitialize;
        public Event OnClosed;

        public Game(string title, string version, string buildNumber, IGameWindow? window = null)
        {
            Title = title;
            Version = version;
            BuildNumber = buildNumber;
            if (window == null)
            {
                // If no Window, create one
                Window = new GameWindow(this);
            }
            else
            {
                Window = window;
            }

            Window.OnRender += Render;
        }

        public void Start()
        {
            IsRunning = true;

            Window.Initialize();
            while (IsRunning)
            {
                Update();
            }
        }

        public void Update()
        {
            Window.Update();
            OnUpdate.Invoke(1);
        }
        public void Render()
        {
        }
        public void Quit()
        {
            IsRunning = false;
        }
    }
}