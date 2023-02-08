using TundraEngine;

public class TestGame1Game : Game
{
    public TestGame1Game(IGameWindow window, string resourcesPath, string texturesPath, string settingsPath) : base(resourcesPath, texturesPath, settingsPath, "My Game", "v0.0.1", "1234412", window)
    {
        SetIcon("Assets/Ships/ship_0000.png");
    }
}