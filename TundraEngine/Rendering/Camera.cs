using Silk.NET.Input;
using Silk.NET.Maths;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Classes;
using TundraEngine.Components;

namespace TundraEngine.Rendering
{
    public class Camera : GameObject
    {
        IGameWindow _window;
        public Transform Position { get; set; }
        public float Zoom { get; set; }
        public Matrix4X4<float> ProjectionMatrix
        {
            get
            {
                var left = Position.X - _window.Width / 2f;
                var right = Position.X + _window.Width / 2f;
                var top = Position.Y + _window.Height / 2f;
                var bottom = Position.Y - _window.Height / 2f;

                return Matrix4X4.CreateOrthographicOffCenter(left,right,bottom,top, 0.01f,10f) * Matrix4X4.CreateScale(Zoom);
            }
        }
        public Camera(Scene scene) : base(scene)
        {
            _window = Game.Window;
            Zoom = 1f;
            Position = AddComponent<Transform>();
            Position.X = 0;
            Position.Y = 0;
        }
    }
}
