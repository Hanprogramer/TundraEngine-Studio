using TundraEngine.Classes;
using TundraEngine.Components;
using TundraEngine.Rendering;

namespace TestGame1.Objects
{
    public class Plane : GameObject
    {
        Transform transform;
        Camera camera;
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
            transform = GetComponent<Transform>();
            camera = Scene.FindObject<Camera>();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            float moveSpeed = 150f * dt;
            if (Input.IsKeyPressed(Key.W)){
                transform.Y += moveSpeed;
            }
            if (Input.IsKeyPressed(Key.S))
            {
                transform.Y -= moveSpeed;
            }
            if (Input.IsKeyPressed(Key.A))
            {
                transform.X -= moveSpeed;
            }
            if (Input.IsKeyPressed(Key.D))
            {
                transform.X += moveSpeed;
            }
            camera.Position = transform;
        }
    }
}
