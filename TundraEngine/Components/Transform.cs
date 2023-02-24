using TundraEngine.Classes;

namespace TundraEngine.Components
{
    public struct Vector2
    {
        public float x, y;
    }
    
    [Component(DisplayName = "Transform", DisplayOnEditor = true)]
    public class Transform : Component
    {
        public float X { get; set; } 
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float XScale { get; set; }
        public float YScale { get; set; }
        public float Rotation { get; set; }

        public Transform(GameObject gameObject, ComponentProperties props) : base(gameObject, props)
        {
        }

        public Transform(ComponentProperties props) : base(props) { }
    }
}
