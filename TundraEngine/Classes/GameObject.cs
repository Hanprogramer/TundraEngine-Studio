using System.Reflection;
using TundraEngine.Components;
using Renderer = TundraEngine.Rendering.Renderer;

namespace TundraEngine.Classes
{
    /// <summary>
    /// Base class for all objects in the game
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Object ID. Used to find the object in the scene using ID
        /// </summary>
        public string? ID = "";

        /// <summary>
        /// The scene containing this object
        /// </summary>
        public Scene Scene;

        // Static references
        public static Renderer Renderer;
        public static Game Game;
        public static ResourceManager AssetManager;

        public List<Component> Components;
        public Transform Transform { get; set; }


        public GameObject(Scene scene)
        {
            Scene = scene;
            Transform = new Transform(this);
            if (Game != null)
            {
                //Game = scene.GameWindow.Game;
                AssetManager = Game.ResourceManager;
            }
            Components = new();
        }

        /// <summary>
        /// Do parts that needs assets loading safely
        /// TODO: do a better way to load assets
        /// </summary>
        public virtual void Initialize()
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].Initialize();
        }


        /// <summary>
        /// Runs every tick
        /// </summary>
        /// <param name="dt">Delta time, time elapsed since last frame</param>
        public virtual void Update(float dt)
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].Update(dt);
        }

        /// <summary>
        /// Called when we need to re-render the object
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void Render(Renderer renderer)
        {
            if (Renderer == null)
                Renderer = renderer;
            for (int i = 0; i < Components.Count; i++)
                Components[i].Render(renderer);
        }

        /// <summary>
        /// Called when object is about to be destroyed
        /// </summary>
        public virtual void Destroy()
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].Destroy();
        }

        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType() == typeof(T))
                {
                    return (T)Components[i];
                }
            }
            throw new Exception("Component of type " + typeof(T) + " not found");
        }

        public T AddComponent<T>(ComponentProperties? props = null) where T: Component
        {
            // Calls the static function
            GameObject obj = this;
            if (props == null)
                props = new ComponentProperties();
            return AddComponent<T>(ref obj, props);
        }

        public void AddComponent(Component comp)
        {
            this.Components.Add(comp);
        }

        public static T AddComponent<T>(ref GameObject gameObject, ComponentProperties props) where T : Component
        {
            var comp = CreateComponent<T>(gameObject, props);
            gameObject.Components.Add(comp);
            return comp;
        }

        public static T CreateComponent<T>(GameObject gameObject, ComponentProperties props) where T : Component
        {
            // TODO: this might not work when multiple derivation generation happens
            if (typeof(T).BaseType == typeof(Component))
            {
                ConstructorInfo? info = typeof(T).GetConstructor(new Type[] { typeof(GameObject) });
                if (info != null)
                {
                    var comp = info.Invoke(new object[]{
                    gameObject,
                    // TODO: do something with the props
                });
                    return (T)comp;
                }
                throw new Exception("Can't get component's constructor");
            }
            else
            {
                throw new Exception("Can't a non component class to GameObject");
            }
        }
        public static T CreateComponent<T>(ComponentProperties props) where T : Component
        {
            // TODO: this might not work when multiple derivation generation happens
            if (typeof(T).BaseType == typeof(Component))
            {
                ConstructorInfo? info = typeof(T).GetConstructor(new Type[] { typeof(ComponentProperties) });
                if (info != null)
                {
                    var comp = info.Invoke(new object[]{
                    props
                });
                    return (T)comp;
                }
                throw new Exception("Can't get component's constructor");
            }
            else
            {
                throw new Exception("Can't a non component class to GameObject");
            }
        }
    }
}
