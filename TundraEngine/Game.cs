using System.Diagnostics;
using TundraEngine.Classes;
using TundraEngine.Rendering;

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

        public delegate void GameEvent();
        public GameEvent OnRender;
        public GameEvent OnInitialize;
        public GameEvent OnClosed;

        public float Ticks = 0;
        public AssetManager AssetManager;

        private Stopwatch UpdateStopwatch;

        public bool DoUpdateOnSeparateThread = true;

        string? icon; //TODO: better icon loading

        public Game(string title, string version, string buildNumber, IGameWindow? window = null)
        {
            Title = title;
            Version = version;
            BuildNumber = buildNumber;
            if (window == null)
                // If no Window, create one
                Window = new GameWindow(this);
            else
                Window = window;

            AssetManager = new AssetManager(Window);
            Window.OnLoadAssets += (Renderer renderer) =>
            {
                AssetManager.LoadTextures(renderer);
                if (icon != null)
                    Window.SetIcon(icon);
            };
            GameManager.Game = this;
            GameManager.AssetManager = AssetManager;
            GameManager.GameWindow = Window;

            GameObject.Game = this;

            UpdateStopwatch = new();
        }

        public void Start(bool shouldCreateEventsLoop = false, bool shouldInitialize = true)
        {
            //OnStart();
            IsRunning = true;
            UpdateStopwatch.Start();
            if (shouldInitialize)
                Window.Initialize();
            if (DoUpdateOnSeparateThread)
            {
                // Run update on separate thread
                new System.Threading.Thread(() =>
                {
                    while (IsRunning)
                    {
                        Update();
                    }
                }).Start();

                if (shouldCreateEventsLoop)
                    while (IsRunning)
                    {
                        Window.PollEvents();
                    }
            }
            else
            {
                // Run update on main thread
                while (IsRunning)
                {
                    Window.PollEvents();
                    Update();
                }
            }
        }

        public void Update()
        {
            var delta = (float)UpdateStopwatch.Elapsed.TotalSeconds;
            UpdateStopwatch.Restart();
            Window.Update(delta);

            if (OnUpdate != null)
                OnUpdate.Invoke(delta);
            Ticks++;
        }
        public void Quit()
        {
            IsRunning = false;
        }

        public void SetIcon(string v)
        {
            icon = v;
        }

        public virtual void OnStart() { }
    }
}