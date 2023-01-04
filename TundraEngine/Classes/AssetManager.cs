using TundraEngine.Rendering;

namespace TundraEngine.Classes
{
    /// <summary>
    /// A class for managing resources used in the game
    /// </summary>
    public class AssetManager
    {
        public Dictionary<string, Texture> Textures;
        public IGameWindow Window;
        public Renderer Renderer;

        public AssetManager(IGameWindow Window)
        {
            Textures = new();
            this.Window = Window;
            Renderer = Window.Renderer;
        }
        public void LoadTextures(Renderer renderer)
        {
            Renderer = renderer;
            foreach (var key in Textures.Keys)
            {
                Console.WriteLine("Loading texture " + key);
                Textures[key].Load(Renderer);
            }
        }
        public void AddTexture(Texture texture)
        {
            Console.WriteLine(Textures.Count);
            if (Textures.ContainsKey(texture.Path)) return;
            Textures.Add(texture.Path, texture);
        }

        public Texture AddTexture(string path)
        {
            if (Textures.ContainsKey(path))
                return Textures[path];

            var t = new Texture(path);
            Textures.Add(path, t);
            return t;
        }

        public Texture GetTexture(string path)
        {
            if (Textures.TryGetValue(path, out Texture? texture))
            {
                return texture;
            }
            else
            {
                throw new Exception("Texture " + path + " not found");
            }
        }

        public void UnloadTexture(string path)
        {
            throw new NotImplementedException("Unload texture not implemented");
        }
    }
}
