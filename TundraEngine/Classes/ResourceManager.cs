using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using TundraEngine.Classes.Data;
using TundraEngine.Rendering;

namespace TundraEngine.Classes
{
    /// <summary>
    /// A class for managing resources used in the game
    /// </summary>
    public class ResourceManager
    {
        public Dictionary<string, Texture> Textures;
        public Dictionary<string, Resource> Resources;
        public IGameWindow Window;
        public Renderer Renderer;

        public ResourceManager(IGameWindow Window)
        {
            Textures = new();
            Resources = new();
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

        public string GenerateUUIDv4()
        {
            return Guid.NewGuid().ToString();
        }



        public Resource GetResource(string uuid)
        {
            if (Resources.ContainsKey(uuid))
            {
                return Resources[uuid];
            }
            throw new Exception("Resource not found");
        }

        public void LoadResourcesFromFile(string path)
        {
            var content = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(content);

            foreach (var pair in json)
            {
                Resource finalResource;
                var res = pair.Value.ToObject<Resource>()!;
                switch (res.resource_type)
                {
                    case ResourceType.Object:
                        finalResource = pair.Value.ToObject<GameObjectResource>()!;
                        break;
                    case ResourceType.Scene:
                        finalResource = pair.Value.ToObject<SceneResource>()!;
                        break;
                    case ResourceType.RawFile:
                        finalResource = pair.Value.ToObject<Resource>()!;
                        break;

                    default:
                        Console.WriteLine($"This resource type can't be imported yet [{res.resource_type}] {res.path}");
                        continue;
                        break;
                }
                Resources.Add(res.uuid, finalResource);
            }
        }
    }
}
