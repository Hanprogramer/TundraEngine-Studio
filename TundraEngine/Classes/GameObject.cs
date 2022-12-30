using TundraEngine.Rendering;
using TundraEngine.Components;
using Silk.NET.GLFW;

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

        /// <summary>
        /// Trasnfrom component of the object
        /// </summary>
        public Transform Transform;

        // Static references
        public static Renderer Renderer;
        public static Game Game;
        public static AssetManager AssetManager;
        

        public GameObject(Scene scene) {
            Transform = new Transform();
            Scene = scene;
            if (Game == null) {
                Game = scene.GameWindow.Game;
                AssetManager = Game.AssetManager;
            }
        }

        /// <summary>
        /// Do parts that needs assets loading safely
        /// TODO: do a better way to load assets
        /// </summary>
        public virtual void Load() {
        
        }


        /// <summary>
        /// Runs every tick
        /// </summary>
        /// <param name="dt">Delta time, time elapsed since last frame</param>
        public virtual void Update(float dt) {

        }

        /// <summary>
        /// Called when we need to re-render the object
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void Render(Renderer renderer) {
            if(Renderer == null)
                Renderer = renderer;
        }

        /// <summary>
        /// Called when object is about to be destroyed
        /// </summary>
        public virtual void Destroy() {
            
        }
    }
}
