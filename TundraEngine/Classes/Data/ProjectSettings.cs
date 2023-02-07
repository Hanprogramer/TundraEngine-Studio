using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TundraEngine.Classes.Data
{
    public class ProjectSettings
    {
        public string starting_scene;
        public string title;

        public bool fullscreen = false;
        public static ProjectSettings Load(string path)
        {
            var content = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<ProjectSettings>(content);
            if(settings != null) 
                return settings;

            // If it reaches here then it failed
            throw new Exception("Failed to parse project settings");
        }
    }
}
