﻿using System.Runtime.Loader;

namespace TundraEngine.Runtime
{
    public class Runner
    {
        public Game? Game;

        /// <summary>
        /// Run the game directly from instance
        /// </summary>
        /// <param name="Game"></param>
        public Runner(Game Game)
        {
            this.Game = Game;
        }
        /// <summary>
        /// Run the game from Type interface
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        public Runner(Type type, string resourcesPath, string texturesPath, string settingsPath)
        {
            this.Game = CreateGame(type, resourcesPath, texturesPath, settingsPath);
        }
        public List<string> GetAllGames()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                  .Where(x => typeof(TundraEngine.Game).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                  .Select(x => x.Name).ToList();
        }
        /// <summary>
        /// Run the game from DLL path
        /// </summary>
        /// <param name="assemblyPath"></param>
        public Runner(string assemblyPath, string resourcesPath, string texturesPath, string settingsPath, string? mainGameClass = null, IGameWindow? window = null)
        {
            Console.WriteLine(assemblyPath);
            CreateGameFromAssembly(assemblyPath, resourcesPath, texturesPath, settingsPath, mainGameClass, window);
        }

        public Game CreateGame(Type type, string resourcesPath, string texturesPath, string settingsPath, IGameWindow? window = null)
        {
            if (type.BaseType == typeof(Game))
            {
                // Create instance of the game 
                var constr = type.GetConstructor(new Type[] { typeof(IGameWindow), typeof(string), typeof(string), typeof(string) });
                if (constr == null) throw new Exception("Error: Can't get constructor(IGameWindow,string,string,string) on " + type.Name);
                var g = (Game?)constr.Invoke(new object[] { window, resourcesPath, texturesPath, settingsPath });
                if (g is Game)
                {
                    return g;
                }
                else
                    throw new Exception("The constructor of the game doesn't return TundraEngine.Game");
            }
            else
            {
                throw new Exception("Can't run the game: Type provided is not a derivative of TundraEngine.Game");
            }
        }

        public void Run()
        {
            Game.OnStart();
            Game.Start(shouldCreateEventsLoop: true);
        }

        public void Destroy()
        {
            Game.Quit();
            Game.Dispose();
            Game = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        void CreateGameFromAssembly(string assemblyPath, string resourcesPath, string texturesPath, string settingsPath, string? mainGameClass = null, IGameWindow? window = null)
        {
            var asl = new AssemblyLoadContext("GameLoader", true);
            var assm = asl.LoadFromAssemblyPath(assemblyPath);

            if (mainGameClass == null)
                foreach (var t in assm.GetTypes())
                {
                    if (t.BaseType == typeof(Game))
                    {
                        mainGameClass = t.Name;
                        break;
                    }
                }
            var type = assm.GetType(mainGameClass);
            if (type == null) throw new Exception("Can't find the game class");
            Console.WriteLine("Creating game object from " + type.Assembly.Location);
            Game = CreateGame(type, resourcesPath, texturesPath, settingsPath, window);
            asl.Unload();
        }

        static int _buildNumber = 0;
        string CreateTempAssembly(string dllPath)
        {
            var filename = Path.GetFileName(dllPath) + _buildNumber.ToString() + ".dll";
            var destfolder = Path.Join(Path.GetDirectoryName(dllPath), "Temp");
            var newFilePath = Path.Join(destfolder, filename);
            if (_buildNumber == 0 && Directory.Exists(destfolder))
            {
                Directory.Delete(destfolder, true);
            }
            if (!Directory.Exists(destfolder))
                Directory.CreateDirectory(destfolder);
            File.Copy(dllPath, newFilePath, true);
            _buildNumber++;
            return newFilePath;
        }
    }
}
