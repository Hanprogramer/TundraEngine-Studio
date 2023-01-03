using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Classes;
using TundraEngine.Components;
using TundraEngine.Rendering;

namespace TestGame1.Objects
{
    public class TestObject : GameObject
    {
        float ticks = 0;
        Transform transform;
        public TestObject(Scene scene) : base(scene)
        {
        }

        public override void Load()
        {
            base.Load();
            transform = GetComponent<Transform>();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            ticks += 1*dt * 10000f;
            transform.X = MathF.Sin((ticks + transform.Y * 100000) / 10000f) * 100f;
            //Console.WriteLine(transform.X);
        }
    }
}
