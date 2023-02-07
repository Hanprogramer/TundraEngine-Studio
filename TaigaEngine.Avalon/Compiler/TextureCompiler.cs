using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TundraEngine.Studio.Compiler
{
    internal class TextureCompiler
    {
        public static string[] fileFilters = { "*.png", "*.bmp", "*.jpg" };
        /// <summary>
        /// Compiles textures in the folder
        /// </summary>
        /// <param name="path">project path</param>
        /// <param name="outputPath">output folder</param>
        /// <returns>the path to the compiled textures exact file</returns>
        public static async Task<string> Compile(string path, string outputPath)
        {
            Dictionary<string, TextureData> textures = new();
            var files = LoadResourcesFromFolder(path);

            foreach (var file in files)
            {
                var content = await File.ReadAllBytesAsync(file);
                using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(file))
                {

                    textures[file.Remove(0, path.Length)] = new TextureData() { 
                        Bytes = content, 
                        Width = img.Width, 
                        Height = img.Height 
                    };
                }
            }

            var finalPath = Path.Join(outputPath, "textures.json");
            await File.WriteAllTextAsync(finalPath, JsonConvert.SerializeObject(textures, Formatting.Indented));
            return finalPath;
        }


        public static string[] LoadResourcesFromFolder(string path)
        {
            //Dictionary<string, Resource> Resources;
            path = Path.GetFullPath(path);

            var list = new List<string>();
            getResourcesFromFolder(path, list);
            foreach (var item in list)
                Console.WriteLine(item.ToString());
            return list.ToArray();
        }

        private static void getResourcesFromFolder(string path, List<string> list)
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
                getResourcesFromFolder(folder, list);
            }
        }
    }
}
