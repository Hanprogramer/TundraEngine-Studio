using TundraEngine.Components;

namespace TundraEngine.Classes.Data
{

    public class SceneResource : Resource
    {
        public SceneResource(string uuid, ResourceType resourceType = ResourceType.Scene) : base(uuid, resourceType)
        { }

        public Scene Instantiate(IGameWindow window)
        {
            var scene = new Scene(window);
            foreach (var data in objects)
            {
                var gobj = new GameObject(scene);

                var transform = gobj.AddComponent<Transform>();
                transform.X = data.position[0];
                transform.Y = data.position[1];
                transform.Width = 32;
                transform.Height = 32;
            }
            return scene;
        }
        public SceneObjectData[] objects { get; set; }
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
