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
            var moveSpeed = 0.1f;
            if (Input.IsKeyPressed(Key.W)){
                Transform.Y += moveSpeed;
            }
            if (Input.IsKeyPressed(Key.S))
            {
                Transform.Y -= moveSpeed;
            }
            if (Input.IsKeyPressed(Key.A))
            {
                Transform.X -= moveSpeed;
            }
            if (Input.IsKeyPressed(Key.D))
            {
                Transform.X += moveSpeed;
            }
        }
    }
}
