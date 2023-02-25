using TundraEngine.Classes;
using TundraEngine.Components;
using TundraEngine.Rendering;

namespace TestGame1.Objects
{
	// Hey this is a comment
    public class PlaneController : Component
    {
        Transform transform;
        public Camera camera { get; set; }
        public PlaneController(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Initialize()
        {
            base.Initialize();
            transform = Object.GetComponent<Transform>();
            camera = Object.Scene.FindObject<Camera>();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            float moveSpeed = 150f * dt;
            if (Input.IsKeyPressed(Key.W))
            {
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
