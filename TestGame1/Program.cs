using System;
using TestGame1;
using TestGame1.Objects;
using TundraEngine;
using TundraEngine.Components;
var game = new Game("My Game", "v0.0.1", "1234412");
var testUpdateSpeed = true;
var testCount = 500;

if (testUpdateSpeed)
{
    // Add entities to the game
    for (var index = 0; index < testCount; index++)
    {
        game.Window.Scene.AddObject(
                new TestObject(game.Window.Scene) { Transform = new Transform() { X = 0, Y = index * 16, Width = 32, Height = 32 } });

    }
    for (var index = 0; index < testCount; index++)
    {
        game.Window.Scene.AddObject(
                new TestObject(game.Window.Scene) { Transform = new Transform() { Y = 0, X = index * 16, Width = 32, Height = 32 } });

    }
}

game.Window.Scene.AddObject(
    new Plane(game.Window.Scene) { 
        Transform = new Transform() { Y = 0, X = 16, Width = 32, Height = 32 } 
    });

// Start the game
game.Start();