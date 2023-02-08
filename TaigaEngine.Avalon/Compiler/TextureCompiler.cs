using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TundraEngine.Classes.Data;
using TundraEngine.Rendering;

namespace TundraEngine.Studio.Compiler
{
    internal class TextureCompiler
    {
        /// <summary>
        /// Filter to all supported textures formats. Includes wildcard
        /// </summary>
        public static string[] fileFilters = { "*.png" }; // TODO: implement more format


        /// <summary>
        /// Compiles textures in the folder
        /// </summary>
        /// <param name="path">project path</param>
        /// <param name="outputPath">output folder</param>
        /// <returns>the path to the compiled textures exact file</returns>
        public static async Task<string> Compile(string path, string outputPath)
        {
            Dictionary<string, TextureData> textures = new();
            var files = FindTexturesInFolder(path);

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var resPath = Path.Join(Path.GetDirectoryName(file), name + ".tspr");

                string uuid;
                // If a texture doesn't have .tspr, generate one
                if (!File.Exists(resPath))
                {
                    uuid = Guid.NewGuid().ToString();
                    var resource = new Resource(uuid, ResourceType.Sprite);
                    var sprResource = await SpriteResource.New(uuid, path, file.Remove(0, path.Length));
                    resource.data = sprResource;
                    var resContent = JsonConvert.SerializeObject(resource, Formatting.Indented);
                    await File.WriteAllTextAsync(resPath, resContent);
                }
                else
                {
                    var resContent = await File.ReadAllTextAsync(resPath);
                    var res = JsonConvert.DeserializeObject<Resource>(resContent);
                    var sprResource = JsonConvert.DeserializeObject<SpriteResource>(JsonConvert.SerializeObject(res.data));
                    uuid = sprResource.uuid;
                }

                var content = await File.ReadAllBytesAsync(file);
                using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(file))
                {
                    //var bytes = img.ToArray<Rgba32>(PngFormat.Instance);
                    var bytes = img.ToBase64String(PngFormat.Instance);
                    textures[uuid] = new TextureData()
                    {
                        Content = bytes.Remove(0, "data:image/png;base64,".Length),
                        Width = img.Width,
                        Height = img.Height
                    };
                }
            }

            var finalPath = Path.Join(outputPath, "textures.json");
            await File.WriteAllTextAsync(finalPath, JsonConvert.SerializeObject(textures, Formatting.Indented));
            return finalPath;
        }

        /// <summary>
        /// Search for every single textures in the game
        /// </summary>
        /// <param name="path">root path to search for</param>
        /// <returns>list of textures path</returns>
        public static string[] FindTexturesInFolder(string path)
        {
            var list = new List<string>();
            findTextures(path, path, list);
            return list.ToArray();
        }

        private static void findTextures(string rootPath, string path, List<string> list)
        {
            foreach (var filter in fileFilters)
            {
                foreach (var file in Directory.GetFiles(path, filter))
                {
                    list.Add(file);
                }
            }

            foreach (var folder in Directory.GetDirectories(path))
            {
                // Skip certain folders
                if (GameCompiler.SKIP_FOLDERS.Any(Path.GetFileName(folder).Equals))
                    continue;
                findTextures(rootPath, folder, list);

            }
        }
    }
}
