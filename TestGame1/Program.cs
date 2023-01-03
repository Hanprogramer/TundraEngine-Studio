using System;
using System.Reflection;
using TestGame1;
using TestGame1.Objects;
using TundraEngine;
using TundraEngine.Components;
var game = new Game("My Game", "v0.0.1", "1234412");
var testUpdateSpeed = true;
var testCount = 50000;

GameManager.AssetManager.AddTexture("Assets/Ships/ship_0002.png");
if (testUpdateSpeed)
{
    // Add entities to the game
    for (var index = 0; index < testCount; index++)
    {
        var obj2 = new TestObject(game.Window.Scene);
        Transform objt = obj2.AddComponent<Transform>();
        objt.X = 10;
        objt.Y = 32 * index;
        objt.Width = 32;
        objt.Height = 32;

        SpriteRenderer sprite2 = obj2.AddComponent<SpriteRenderer>();
        sprite2.Texture = GameManager.AssetManager.AddTexture("Assets/silk.png");
        game.Window.Scene.AddObject(obj2);
    }
    //for (int i = 0; i < 200; i++)
    //{
    //    game.Window.Scene.AddObject(
    //            new TestObject(game.Window.Scene) { Transform = new Transform() { X = 0, Y = i * 16, Width = 32, Height = 32 } });
    //}

    //for (var index = 0; index < testCount; index++)
    //{
    //    game.Window.Scene.AddObject(
    //            new TestObject(game.Window.Scene) { Transform = new Transform() { Y = 0, X = index * 16, Width = 32, Height = 32 } });

    //}
}


var obj = new Plane(game.Window.Scene);
var transform = obj.AddComponent<Transform>();
transform.Width = 32;
transform.Height = 32;

SpriteRenderer sprite = obj.AddComponent<SpriteRenderer>();
sprite.Texture = GameManager.AssetManager.AddTexture("Assets/Ships/ship_0000.png");
game.Window.Scene.AddObject(obj);

GameManager.AssetManager.AddTexture("Assets/Ships/ship_0006.png");

game.SetIcon("Assets/Ships/ship_0000.png");

// Start the game
game.Start();