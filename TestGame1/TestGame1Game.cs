using TestGame1.Objects;
using TundraEngine;
using TundraEngine.Components;
using TundraEngine.Rendering;

public class TestGame1Game : Game
{
    bool testUpdateSpeed = true;
    int testCount = 5000;
    public TestGame1Game(IGameWindow window) : base("My Game", "v0.0.1", "1234412", window)
    {
        SetIcon("Assets/Ships/ship_0000.png");
    }

    public override void OnStart()
    {
        base.OnStart();

        if (testUpdateSpeed)
        {
            // Add entities to the game
            for (var index = 0; index < testCount; index++)
            {
                var obj2 = new TestObject(Window.Scene);
                Transform objt = obj2.AddComponent<Transform>();
                objt.X = 10;
                objt.Y = 32 * index;
                objt.Width = 32;
                objt.Height = 32;

                SpriteRenderer sprite2 = obj2.AddComponent<SpriteRenderer>();
                sprite2.Texture = GameManager.AssetManager.AddTexture("Assets/silk.png");
                Window.Scene.AddObject(obj2);
            }
        }


        var obj = new Plane(Window.Scene);
        var transform = obj.AddComponent<Transform>();
        transform.Width = 32;
        transform.Height = 32;

        SpriteRenderer sprite = obj.AddComponent<SpriteRenderer>();
        sprite.Texture = GameManager.AssetManager.AddTexture("Assets/Ships/ship_0000.png");
        Window.Scene.AddObject(obj);


        Camera Camera = (Camera)Window.Scene.AddObject(new Camera(Window.Scene));
    }
}