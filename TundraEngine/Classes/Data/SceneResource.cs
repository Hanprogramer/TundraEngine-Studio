using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TundraEngine.Classes.Data
{

    public class SceneResource : Resource
    {
        public SceneResource(string uuid, ResourceType resourceType = ResourceType.Scene) : base(uuid, resourceType)
        {}
        public SceneObjectData[] objects { get; set; }
    }

    public class SceneObjectData
    {
        public string uuid { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public object properties { get; set; }
    }

}
