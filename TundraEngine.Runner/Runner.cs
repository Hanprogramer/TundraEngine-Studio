using System.Reflection;

namespace TundraEngine.Runner
{
    public class Runner
    {
        public Game Game;
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
        public Runner(string path)
        {
            var assm = Assembly.LoadFrom(path);
            var types = assm.GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(Game))
                {
                    this.Game = CreateGame(type);
                    break;
                }
            }
            if (this.Game == null)
            {
                throw new Exception("Can't find Game on the assembly");
            }
        }

        public Game CreateGame(Type type)
        {
            if (type.BaseType == typeof(Game))
            {
                // Create instance of the game 
                var constr = type.GetConstructor(new Type[] { typeof(IGameWindow) });
                if (constr == null) throw new Exception("Error: Can't get constructor(IGameWindow) on " + type.Name);
                var g = (Game?)constr.Invoke(new object[] { null });
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
    }
}
