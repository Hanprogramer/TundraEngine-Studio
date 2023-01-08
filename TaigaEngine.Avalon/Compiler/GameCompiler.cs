using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TundraEngine.Classes;

namespace TundraEngine.Studio.Compiler
{
    public enum CompileDiagnosticsItemType
    {
        Error,
        Warning,
        Log
    }
    public class CompileDiagnosticsItem
    {
        public string Title;
        public string Description;
        public CompileDiagnosticsItemType Type;

        public CompileDiagnosticsItem(string title, string description, CompileDiagnosticsItemType type)
        {
            Title = title;
            Description = description;
            Type = type;
        }
    }
    public class GameCompiler
    {
        public static async Task<List<CompileDiagnosticsItem>?> Compile(TundraProject project)
        {
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterDefaults();
            CompilerParameters cparams = new();
            
            List<CompileDiagnosticsItem> diagnostics = new();

            var result = await CompileSolution(Path.Join(project.Path, project.CSProject), Path.Join(project.Path, "bin"));
            if (result == null) return null;
            foreach (var d in result.Diagnostics)
            {
                if (d.Severity == DiagnosticSeverity.Error)
                    diagnostics.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Error));
                else if (d.Severity == DiagnosticSeverity.Warning)
                    diagnostics.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Warning));
            }

            if (result.Success)
                Console.WriteLine("Build was successful");
            else
                Console.WriteLine("Build was failed");
            return diagnostics;
        }

        private static async Task<EmitResult?> CompileSolution(string solutionUrl, string outputPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var project = await workspace.OpenProjectAsync(solutionUrl);
                var compilation = await project.GetCompilationAsync();
                if (compilation == null)
                {
                    Console.WriteLine("Can't get compilation target for " + solutionUrl);
                    return null;
                }
                var result = compilation.Emit(Path.Join(outputPath , "TundraGame.dll"));
                foreach (var d in result.Diagnostics)
                {
                    Console.WriteLine(d);
                }
                return result;
            }
        }
    }
}
