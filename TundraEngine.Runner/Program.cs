using TundraEngine.Runtime;

string assemblyPath = "";
string resourcePath = "";
string texturesPath = "";
string settingsPath = "";
if (args.Length > 0)
{
    // Run on the selected DLL
    assemblyPath = args[0];
    resourcePath = args[1];
    texturesPath = args[2];
    settingsPath = args[3];

}
else
{
    throw new NotImplementedException();
    // Find TundraGame.dll
    assemblyPath = "TundraGame.dll";
    assemblyPath = "D:\\Programming\\C#\\TaigaEngine.Avalon\\TestGame1\\bin\\TundraGame0.dll";
}

var r = new Runner(assemblyPath, resourcePath, texturesPath, settingsPath);
r.Run();
Console.ReadLine();