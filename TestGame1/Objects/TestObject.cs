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
        Texture texture;
        int ticks = 0;
        public TestObject(Scene scene) : base(scene)
        {
        }

        public override void Load()
        {
            texture = AssetManager.AddTexture("Assets/silk.png");
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            renderer.DrawTexture(texture, Transform);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            ticks++;
            Transform.X = MathF.Sin((ticks + Transform.Y * 100000) / 10000f) * 100f;
            //Console.WriteLine(Transform.X);
        }
    }
}
