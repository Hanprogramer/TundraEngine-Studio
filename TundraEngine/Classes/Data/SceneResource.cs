using System.Reflection;
using System.Runtime.Loader;
using TundraEngine.Components;

namespace TundraEngine.Classes.Data
{

    public class SceneResource : Resource
    {
        public SceneResource(string uuid, ResourceType resourceType = ResourceType.Scene) : base(uuid, ResourceType.Scene)
        { }

        /// <summary>
        /// Create an instance of the scene
        /// </summary>
        /// <param name="window">Current game window</param>
        /// <param name="rm">Resource manager</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Scene Instantiate(IGameWindow window, ResourceManager rm, AssemblyLoadContext? asl = null)
        {
            var scene = new Scene(window);
            foreach (var data in objects)
            {
                var res = rm.GetResource<GameObjectResource>(data.uuid);
                var gobj = new GameObject(scene);

                var transform = gobj.AddComponent<Transform>();
                transform.X = data.position[0];
                transform.Y = data.position[1];
                transform.Width = 32;
                transform.Height = 32;

                foreach (var comp in res.components)
                    comp.Instantiate(gobj, rm, asl);
                
                scene.AddObject(gobj);

                //TODO: implement children
            }
            return scene;
        }
        public SceneObjectData[] objects { get; set; }

        public static async Task<SceneResource> Load(string path)
        {
            return await Load<SceneResource>(path, ".tscn");
        }
    }

    public class SceneObjectData
    {
        public string uuid { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public object properties { get; set; }
        public int[] position { get; set; }
    }

}
