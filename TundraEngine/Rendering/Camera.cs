using Silk.NET.Maths;
using TundraEngine.Classes;
using TundraEngine.Components;

namespace TundraEngine.Rendering
{
    public class Camera : GameObject
    {
        IGameWindow _window;
        public Transform Position { get; set; }
        public float Zoom { get; set; }
        public bool FlipY { get; set; } = false;
        public Matrix4X4<float> ProjectionMatrix
        {
            get
            {
                var left = Position.X - _window.Width / 2f;
                var right = Position.X + _window.Width / 2f;
                var top = Position.Y + _window.Height / 2f;
                var bottom = Position.Y - _window.Height / 2f;
                if (FlipY)
                    return Matrix4X4.CreateOrthographicOffCenter(left, right, bottom, top, 0.01f, 10f) * Matrix4X4.CreateScale(Zoom, -Zoom, 1);
                return Matrix4X4.CreateOrthographicOffCenter(left, right, bottom, top, 0.01f, 10f) * Matrix4X4.CreateScale(Zoom);
            }
        }
        public Camera(Scene scene, IGameWindow window) : base(scene)
        {
            _window = window;
            Zoom = 1f;
            Position = AddComponent<Transform>();
            Position.X = 0;
            Position.Y = 0;
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            renderer.SetProjectionMatrix(ProjectionMatrix);
        }
    }
}
