using TundraEngine.Classes;

namespace TundraEngine.Components
{
    public struct Vector2
    {
        public float x, y;
    }

    public class Transform : Component
    {
        public float X, Y;
        public float Width, Height;
        public float XScale, YScale;
        public float Rotation;

        public Transform(GameObject gameObject, ComponentProperties props) : base(gameObject, props)
        {
        }

        public Transform(ComponentProperties props) : base(props) { }
    }
}
