using TundraEngine.Classes;
using TundraEngine.Rendering;

namespace TundraEngine.Components
{
    public class Component
    {
        public Component(GameObject gameObject) { Object = gameObject; }
        public Component(GameObject gameObject, ComponentProperties props) { Object = gameObject; }
        public Component(ComponentProperties props) { }
        public GameObject Object;
        public virtual void Initialize() { }
        public virtual void Update(float dt) { }
        public virtual void Render(Renderer Renderer) { }
        public virtual void Destroy() { }
    }
}
