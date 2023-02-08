using TundraEngine.Classes;
using TundraEngine.Components;

namespace TestGame1.Objects
{
    public class TestObject : Component
    {
        float ticks = 0;
        Transform transform;

        public TestObject(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            transform = Object.GetComponent<Transform>();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            ticks += 1 * dt * 10000f;
            transform.X = MathF.Sin((ticks + transform.Y * 100000) / 10000f) * 100f;
            //Console.WriteLine(transform.X);
        }
    }
}
