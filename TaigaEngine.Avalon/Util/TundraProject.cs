using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TundraEngine.Classes.Data;
using TundraEngine.Studio.Compiler;
using TundraEngine.Studio.Dialogs;

namespace TundraEngine.Studio.Util
{

    public class TundraProject
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public int[] TundraVersion { get; set; }
        public string Author { get; set; }
        public string CSProject { get; set; }
        public int FormatVersion { get; set; }

        internal Dictionary<string, Resource> Resources;

        public TundraProject(string path, string title, string version, int[] tundraVersion, string author, string cSProject, int formatVersion)
        {
            Path = path;
            Title = title;
            Version = version;
            TundraVersion = tundraVersion;
            Author = author;
            CSProject = cSProject;
            FormatVersion = formatVersion;
        }

        private async void Initialize(string path)
        {
            Resources = await ResourceCompiler.Analyze(path, true);
        }
        /// <summary>
        /// Parse a tundra project.json file
        /// </summary>
        /// <param name="filePath">absolute path to project.json file</param>
        /// <param name="parent">parent window to show error message dialog</param>
        /// <returns></returns>
        public static TundraProject? Parse(string filePath, Window? parent = null)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Error", "Error: project file not found: " + filePath, parent);
                return null;
            }

            var content = File.ReadAllText(filePath);
            try
            {
                TundraProject? project = JsonConvert.DeserializeObject<TundraProject>(content);
                if (project != null)
                {
                    var dirPath = System.IO.Path.GetDirectoryName(filePath);
                    if (dirPath == null)
                    {
                        MessageBox.Show("Error", "Error: Can't get directory path of project.json", parent);
                        return null;
                    }
                    project.Path = dirPath;
                    project.Initialize(dirPath);
                    return project;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error", "Error parsing JSON: \n" + e.ToString(), parent);
            }

            // If json is null
            MessageBox.Show("Error", "Error: parsing project.json: " + filePath, parent);
            return null;
        }
    }
}
