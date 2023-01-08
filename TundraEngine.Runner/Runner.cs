using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace TundraEngine.Runner
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
        public Runner(Type type)
        {
            this.Game = CreateGame(type);
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
        /// <param name="path"></param>
        public Runner(string path, IGameWindow? window = null)
        {
            Console.WriteLine(path);
            CreateGameFromAssembly(path, window);
        }

        public Game CreateGame(Type type, IGameWindow? window = null)
        {
            if (type.BaseType == typeof(Game))
            {
                // Create instance of the game 
                var constr = type.GetConstructor(new Type[] { typeof(IGameWindow) });
                if (constr == null) throw new Exception("Error: Can't get constructor(IGameWindow) on " + type.Name);
                var g = (Game?)constr.Invoke(new object[] { window });
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
        void CreateGameFromAssembly(string assemblyPath, IGameWindow? window)
        {
            var asl = new AssemblyLoadContext("GameLoader", true);
            var assm = asl.LoadFromAssemblyPath(assemblyPath);
            var type = assm.GetType("TestGame1Game");
            Console.WriteLine("Creating game object from " + type.Assembly.Location);
            if (type == null) throw new Exception("Can't find the game class");
            Game = CreateGame(type,window);
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
