using System.Reflection;
using TundraEngine.Components;

namespace TundraEngine.Classes.Data
{

    public class SceneResource : Resource
    {
        public static string TUNDRA_ENGINE = typeof(TundraEngine.Game).Assembly.GetName().Name;
        public static Assembly TUNDRA_ENGINE_ASSEMBLY = typeof(TundraEngine.Game).Assembly;
        public SceneResource(string uuid, ResourceType resourceType = ResourceType.Scene) : base(uuid, ResourceType.Scene)
        { }

        /// <summary>
        /// Create an instance of the scene
        /// </summary>
        /// <param name="window">Current game window</param>
        /// <param name="rm">Resource manager</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Scene Instantiate(IGameWindow window, ResourceManager rm)
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
                {
                    if (comp.component.StartsWith(TUNDRA_ENGINE))
                    {
                        var type = TUNDRA_ENGINE_ASSEMBLY.GetType(comp.component);
                        if (type != null)
                        {
                            if (type.IsSubclassOf(typeof(Component)))
                            {
                                var ctr = type.GetConstructor(new Type[] { typeof(GameObject) });
                                if (ctr == null)
                                    throw new Exception($"Class {comp.component} doesn't have a (GameObject){{}} constructor");
                                var compInst = ctr.Invoke(new object[] { gobj });
                                if (compInst is SpriteRenderer sr)
                                {
                                    var texProp = comp.properties["Texture"]!;
                                    var tex = rm.GetTexture(texProp);
                                    sr.Texture = tex;
                                }
                                gobj.AddComponent((Component)compInst);
                            }
                            else
                                throw new Exception($"Class {comp.component} is not a Component");
                        }
                        else
                        {
                            throw new Exception($"Can't find component class {comp.component}");
                        }
                    }
                }

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
