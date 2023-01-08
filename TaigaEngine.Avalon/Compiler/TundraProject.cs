using Newtonsoft.Json;
using System;
using System.IO;

namespace TundraEngine.Classes
{
    public class TundraProject
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string TundraVersion { get; set; }
        public string Author { get; set; }
        public string CSProject { get; set; }
        public string Path { get; set; }

        public TundraProject(string title, string version, string tundraVersion, string author, string cSProject, string path)
        {
            Title = title;
            Version = version;
            TundraVersion = tundraVersion;
            Author = author;
            CSProject = cSProject;
            Path = path;
        }

        public static TundraProject Parse(string projectJsonPath)
        {
            var content = File.ReadAllText(projectJsonPath);
            var project = JsonConvert.DeserializeObject<TundraProject>(content);
            if (project != null)
            {
                var path = System.IO.Path.GetDirectoryName(projectJsonPath);
                if (path == null) throw new Exception("Can't find folder path of " + projectJsonPath);
                project.Path = path;
                return project;
            }
            throw new Exception("Error parsing " + projectJsonPath);
        }
    }

}
