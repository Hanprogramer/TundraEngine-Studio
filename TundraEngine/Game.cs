using Silk.NET.SDL;
using TundraEngine.Classes;
using TundraEngine.Components;

namespace TundraEngine
{
    public class Game
    {
        public string Title { get; }
        public string Version { get; }
        public string BuildNumber { get; }

        public IGameWindow Window;
        public Scene World { get => Window.Scene; }
        public bool IsRunning = false;

        public delegate void OnUpdateHandler(double dt);

        public OnUpdateHandler OnUpdate;
        public Event OnRender;
        public Event OnInitialize;
        public Event OnClosed;

        public float Ticks = 0;
        public AssetManager AssetManager;

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
            AssetManager = new AssetManager(Window);
            Window.OnLoadAssets += () =>
            {
                AssetManager.LoadTextures();
            };
            GameManager.Game = this;
            GameManager.AssetManager = AssetManager;
            GameManager.GameWindow = Window;
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

            //TODO: implement real DT
            Window.PollEvents();
            if(OnUpdate != null)
                OnUpdate.Invoke(1);
            Ticks++;
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