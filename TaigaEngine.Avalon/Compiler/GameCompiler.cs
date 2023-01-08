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

    public class CompileDiagnostics {
        public List<CompileDiagnosticsItem> Items;
        public bool Success = false;
        public string DllPath = "";
        public CompileDiagnostics() {
            Items = new();
        }
    }
    public class GameCompiler
    {
        public static async Task<CompileDiagnostics?> Compile(TundraProject project)
        {
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterDefaults();

            CompileDiagnostics diagnostics = new();
            var outputPath = Path.Join(project.Path, "bin", "TundraGame.dll");
            var result = await CompileSolution(Path.Join(project.Path, project.CSProject), outputPath);
            if (result == null) return null;
            foreach (var d in result.Diagnostics)
            {
                if (d.Severity == DiagnosticSeverity.Error)
                    diagnostics.Items.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Error));
                else if (d.Severity == DiagnosticSeverity.Warning)
                    diagnostics.Items.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Warning));
            }

            if (result.Success)
                Console.WriteLine("Build was successful");
            else
                Console.WriteLine("Build was failed");
            diagnostics.Success = result.Success;
            diagnostics.DllPath = result.Success? outputPath : "";
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
                var result = compilation.Emit(outputPath);
                foreach (var d in result.Diagnostics)
                {
                    Console.WriteLine(d);
                }
                return result;
            }
        }
    }
}
