using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TundraEngine.Components
{
    public class Component
    {
        public Component(GameObject gameObject) { Object = gameObject; }
        public GameObject Object;
        public virtual void Initialize() { }
        public virtual void Update(float dt) { }
        public virtual void Render(Renderer Renderer) { }
        public virtual void Destroy() { }
        public virtual void Assign(Dictionary<string, object> properties){}
    }
}
