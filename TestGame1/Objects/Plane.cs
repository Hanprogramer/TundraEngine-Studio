using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TestGame1.Objects
{
    public class Plane : GameObject
    {
        Texture texture;
        public Plane(Scene scene) : base(scene)
        {
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Load()
        {
            base.Load();
            texture = AssetManager.AddTexture("Assets/Ships/ship_0000.png");
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            renderer.camera.Position = Transform;
            renderer.camera.Zoom = 2;
            renderer.DrawTexture(texture, Transform);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
