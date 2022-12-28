using Arch.Core;
using TundraEngine;
using TundraEngine.Components;

var game = new Game("My Game", "v0.0.1", "1234412");

// Add entities to the game
for (var index = 0; index < 1000; index++)
    game.World.Create(
        new Transform() { X = 2, Y = index },
        new Sprite() { textureID = 0 }
        );

// A simple update query
game.OnUpdate += (double dt) =>
{
    // Updating position
    var query = new QueryDescription { All = new Arch.Core.Utils.ComponentType[] { typeof(Transform) } };
    game.World.Query(in query, (ref Transform transform) =>
    {
        transform.X += (float)((1f / transform.Y) * dt);
    });
};

// Start the game
game.Start();