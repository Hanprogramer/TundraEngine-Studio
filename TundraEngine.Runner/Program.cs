using TundraEngine.Runner;

string file = "";
if (args.Length > 0)
{
    // Run on the selected DLL
    file = args[0];

}
else
{
    // Find TundraGame.dll
    file = "TundraGame.dll";
}

var r = new Runner(file);
r.Run();
Console.ReadLine();