using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TundraEngine.Classes;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Compiler
{
    /// <summary>
    /// Compiler diagnostic type
    /// </summary>
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

    public class CompileDiagnostics
    {
        public List<CompileDiagnosticsItem> Items;
        public bool Success = false;
        public string DllPath = "";
        public CompileDiagnostics()
        {
            Items = new();
        }
    }
    public class GameCompiler
    {
        public static int _buildNumber = 0;
        public delegate void LogFunction(string message);
        /// <summary>
        /// Compiles the game project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static async Task<CompileDiagnostics?> Compile(TundraProject project, LogFunction log)
        {
            // Register MSBuild variables
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterDefaults();

            // Create diagnostic result 
            CompileDiagnostics diagnostics = new();

            // Output path for the dll
            // build number needed because we still locking the previous build's dll (if any)
            var outputPath = Path.Join(project.Path, "bin", $"TundraGame{_buildNumber++}.dll");

            // Compile the solution/project
            var result = await CompileCSProject(Path.Join(project.Path, project.CSProject), outputPath);

            if (result == null) return null;

            // List the diagnostics as TundraEngine Studio's diagnostics
            foreach (var d in result.Diagnostics)
            {
                if (d.Severity == DiagnosticSeverity.Error)
                    diagnostics.Items.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Error));
                else if (d.Severity == DiagnosticSeverity.Warning)
                    diagnostics.Items.Add(new CompileDiagnosticsItem(d.Descriptor.Title.ToString(), d.Descriptor.Description.ToString(), CompileDiagnosticsItemType.Warning));
            }

            if (result.Success)
                log("Build was successful");
            else
                log.Invoke("Build was failed");

            // Set the success state and the dllpath of the output
            diagnostics.Success = result.Success;
            diagnostics.DllPath = result.Success ? outputPath : "";
            return diagnostics;
        }

        /// <summary>
        /// Compile the project
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="outputDllPath"></param>
        /// <returns></returns>
        private static async Task<EmitResult?> CompileCSProject(string projectPath, string outputDllPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                // Delete output dll if exists
                if (File.Exists(outputDllPath))
                    File.Delete(outputDllPath);

                // Open the project
                Project project = await workspace.OpenProjectAsync(projectPath);
                Compilation? compilation = await project.GetCompilationAsync();

                if (compilation == null)
                {
                    Console.WriteLine("Can't get compilation target for " + projectPath);
                    return null;
                }

                // Run compilation
                var result = compilation.Emit(outputDllPath);

                // List diagnostics
                foreach (var d in result.Diagnostics)
                {
                    Console.WriteLine(d);
                }

                // Cleanup
                workspace.CloseSolution();
                workspace.Dispose();
                return result;
            }
        }
    }
}
