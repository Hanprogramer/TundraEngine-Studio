using TundraEngine.Runtime;

string assemblyPath = "";
string resourcePath = "";
string texturesPath = "";
if (args.Length > 0)
{
    // Run on the selected DLL
    assemblyPath = args[0];
    resourcePath = args[1];
    texturesPath = args[2];

}
else
{
    throw new NotImplementedException();
    // Find TundraGame.dll
    assemblyPath = "TundraGame.dll";
    assemblyPath = "D:\\Programming\\C#\\TaigaEngine.Avalon\\TestGame1\\bin\\TundraGame0.dll";
}

var r = new Runner(assemblyPath, resourcePath, texturesPath);
r.Run();
Console.ReadLine();