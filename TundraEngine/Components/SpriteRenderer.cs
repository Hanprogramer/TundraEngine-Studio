using TundraEngine.Classes;
using TundraEngine.Rendering;
using Texture = TundraEngine.Rendering.Texture;

namespace TundraEngine.Components
{
    /// <summary>
    /// A basic component to draw a texture onto the screen
    /// </summary>
    public class SpriteRenderer : Component
    {
        public Texture Texture;
        public Transform Transform;
        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
            //Texture = props.GetValue<Texture>("Texture");
            Transform = gameObject.GetComponent<Transform>();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Render(Renderer Renderer)
        {
            base.Render(Renderer);
            if (Renderer == null)
            {
                Console.WriteLine("Renderer is null");
                return;
            }
            if (Texture.IsLoaded)
                Renderer.DrawTexture(Texture, Transform);
            //else
            //Texture.Load(Renderer);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
