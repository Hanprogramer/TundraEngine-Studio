using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<SpriteResource> New(string uuid,string rootPath, string path)
        {
            using (var img = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(Path.Join(rootPath, path)))
            {
                var spr = new SpriteResource(uuid);

                spr.width = img.Width;
                spr.height = img.Height;
                spr.format = "1";
                spr.content = path;
                return spr;
            }
            throw new Exception("Failed to import texture " + path);
        }

        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
        public string content { get; set; }
    }

}
