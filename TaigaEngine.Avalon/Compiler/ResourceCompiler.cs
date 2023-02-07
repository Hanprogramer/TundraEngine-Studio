using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Classes.Data;
using TundraEngine.Components;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Compiler
{
    public static class ResourceCompiler
    {
        public static string[] resourceFilters = { "*.tobj", "*.tscn", "*.tspr" };


        /// <summary>
        /// Compile the list of resources
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> Compile(string path, string outputFolder)
        {
            var resources = LoadResourcesFromFolder(path);
            var result = new Dictionary<string, Resource>();
            Console.WriteLine($"Compiling resources.. {path}");
            foreach (var filepath in resources)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(filepath, Encoding.UTF8);
                    var json = JsonConvert.DeserializeObject<Resource>(content);

                    if (json == null)
                    {
                        // Very unlikely, unless the JSON is empty
                        Console.WriteLine($"Json is null {filepath}");
                        continue;
                    }

                    ResourceType type = json.resource_type;
                    Resource finalData;
                    switch (type)
                    {
                        case ResourceType.Object:
                            finalData = JsonConvert.DeserializeObject<GameObjectResource>(
                                JsonConvert.SerializeObject(json.data)
                            )!;
                            break;

                        case ResourceType.Scene:
                            finalData = JsonConvert.DeserializeObject<SceneResource>(
                                JsonConvert.SerializeObject(json.data)
                            )!;
                            break;

                        default:
                            finalData = json;
                            Console.WriteLine($"Unknown resource type for {filepath}");
                            break;
                    }
                    finalData.uuid = json.uuid;
                    finalData.path = filepath.Remove(0, TundraStudio.CurrentProject.Path.Length);
                    Console.WriteLine($"Added {filepath}");
                    result.Add(json.uuid, finalData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
            // Writes out the compiled file
            var outputPath = Path.Join(outputFolder, "resources.json");
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            await File.WriteAllTextAsync(outputPath, JsonConvert.SerializeObject(result, Formatting.Indented));


            return outputPath;


            // If it reaches here, then something wrong
            throw new Exception("Failed compiling resources");
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
            foreach (var filter in resourceFilters)
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
