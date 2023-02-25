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
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Width { get; set; } = 1;
        public float Height { get; set; } = 1;
        public float XScale { get; set; } = 1;
        public float YScale { get; set; } = 1;
        public float Rotation { get; set; } = 0;

        public Transform(GameObject gameObject) : base(gameObject)
        {
            
        }
    }
}
