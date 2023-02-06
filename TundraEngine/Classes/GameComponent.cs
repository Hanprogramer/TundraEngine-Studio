using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Rendering;

namespace TundraEngine.Classes
{
    public class GameComponent
    {
        public GameObject GameObject;
        public GameComponent(GameObject gameObject)
        {
            GameObject = gameObject;
        }
        public virtual void Initialize() { }
        public virtual void Update(float dt) { }
        public virtual void Draw(Renderer renderer) { }
        public virtual void Destroy() { }
    }
}
