using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using TundraEngine.Rendering;

namespace TundraEngine.Classes.Data
{
    public class SpriteResource : Resource
    {
        public SpriteResource(string uuid, ResourceType resourceType = ResourceType.Sprite) : base(uuid, ResourceType.Sprite)
        {
        }

        /// <summary>
        /// Generates a SpriteResource based of a file
        /// </summary>
        /// <param name="rootPath">Project root path</param>
        /// <param name="path">Relative path to the file</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<SpriteResource> New(string uuid, string rootPath, string path)
        {
            using (var img = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(Path.Join(rootPath, path)))
            {
                var spr = new SpriteResource(uuid);

                spr.width = img.Width;
                spr.height = img.Height;
                spr.content = path;
                return spr;
            }
            throw new Exception("Failed to import texture " + path);
        }

        public static async Task<SpriteResource> Load(string path)
        {
            return await Load<SpriteResource>(path, ".tspr");
        }

        public static SpriteResource LoadSync(string path)
        {
            return LoadSync<SpriteResource>(path, ".tspr");
        }

        public Texture CreateTexture(Renderer renderer)
        {
            //TODO: use the content property instead
            var contentPath = path.Remove(path.Length-".tspr".Length);
            var texture = new Texture(contentPath);
            texture.Load(renderer);
            return texture;
        }

        

        public int width { get; set; }
        public int height { get; set; }
        public string content { get; set; }
    }

}
