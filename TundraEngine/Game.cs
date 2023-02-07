using System.Diagnostics;
using TundraEngine.Classes;
using TundraEngine.Classes.Data;
using TundraEngine.Rendering;

namespace TundraEngine
{
    public class Game : IDisposable
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

        public System.Threading.Thread? UpdateThread;

        public float Ticks = 0;
        private double _updatePeriod;

        public double UpdatesPerSecond
        {
            get => _updatePeriod <= double.Epsilon ? 0 : 1 / _updatePeriod;
            set => _updatePeriod = value <= double.Epsilon ? 0 : 1 / value;
        }

        public ResourceManager ResourceManager;
        public ProjectSettings ProjectSettings;

        private Stopwatch UpdateStopwatch;

        public bool DoUpdateOnSeparateThread = true;

        string? icon; //TODO: better icon loading

        public Game(string resourcesPath, string texturesPath, string settingsPath, string title, string version, string buildNumber, IGameWindow? window = null)
        {
            Title = title;
            Version = version;
            BuildNumber = buildNumber;
            if (window == null)
                // If no Window, create one
                Window = new GameWindow(this);
            else
                Window = window;

            ResourceManager = new ResourceManager(Window);
            Window.OnLoadAssets += (Renderer renderer) =>
            {
                ResourceManager.LoadResourcesFromFile(resourcesPath);
                ResourceManager.LoadTexturesFromFile(texturesPath);
                ResourceManager.LoadTextures(renderer);
                if (icon != null)
                    Window.SetIcon(icon);
            };
            ProjectSettings = ProjectSettings.Load(settingsPath);
            GameManager.Game = this;
            GameManager.AssetManager = ResourceManager;
            GameManager.GameWindow = Window;

            GameObject.Game = this;

            UpdateStopwatch = new();
            UpdatesPerSecond = 120;
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
                UpdateThread = new System.Threading.Thread(() =>
                {
                    while (IsRunning)
                    {
                        Update();
                    }
                });
                UpdateThread.Start();

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
            if (delta > _updatePeriod)
            {
                UpdateStopwatch.Restart();
                Window.Update(delta);
                OnUpdate?.Invoke(delta);
                Ticks++;
            }

        }
        public void Quit()
        {
            IsRunning = false;
            Dispose();
        }

        public void SetIcon(string v)
        {
            icon = v;
        }

        public virtual void OnStart() { }

        public void Dispose()
        {
            if (UpdateThread != null && UpdateThread.ThreadState == System.Threading.ThreadState.Running)
            {
                Console.WriteLine("Stopping update thread");
                UpdateThread?.Join();
            }
        }
    }
}