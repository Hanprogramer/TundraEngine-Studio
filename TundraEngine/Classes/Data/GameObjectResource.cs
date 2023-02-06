using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TundraEngine.Classes.Data
{

    public class GameComponentResource
    {
        public string component { get; set; }
        public object properties { get; set; }
    }

    public class GameObjectResource : Resource
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public string description { get; set; }
        public GameComponentResource[] components { get; set; }
        public object[] children { get; set; }

        public GameObjectResource(string uuid, string name, string description, GameComponentResource[] components, GameObjectResource[] children) : base(uuid, ResourceType.Object)
        {
            this.uuid = uuid;
            this.name = name;
            this.description = description;
            this.components = components;
            this.children = children;
        }
    }



}
