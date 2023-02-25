using TundraEngine.Classes;
using TundraEngine.Classes.Data;
namespace TundraEngine.Components
{
    /// <summary>
    /// Testing component to see all the stuffs
    /// </summary>
    [Component]
    public class TestComponent : Component
    {
        public int NumberVal { get; set; }
        public int NumberVal2;
        
        public string Text { get; set; }
        public bool YesNo { get; set; }
        public float Decimal { get; set; }

        public Resource Resource1 { get; set; }
        public SpriteResource SpriteResource1 { get; set; }
        public SceneResource SceneResource1 { get; set; }
        
        public TestComponent(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
