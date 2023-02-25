using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using TundraEngine.Classes.Data;
using TundraEngine.Components;
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
        public static string[] SKIP_FOLDERS = { "bin", "obj" };

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

        private static void AnalyzeAssembly(string dllPath, ref List<Type> types)
        {
            var asl = TundraStudio.Asl;
            var asm = asl.LoadFromAssemblyPath(dllPath);
            foreach (var type in asm.GetTypes())
            {
                var isComponent = type.GetCustomAttributes(false).OfType<ComponentAttribute>().Any() || type.IsSubclassOf(typeof(TundraEngine.Components.Component));
                if(isComponent)
                    types.Add(type);
            }
            asl.Unload();
        }

        public static async Task<List<Type>> AnalyzeProject(string path, string outputFolderPath)
        {
            // Register MSBuild variables
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterDefaults();

            var types = new List<Type>();
            
            using var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(path);
            var comp = await project.GetCompilationAsync();
            var uuid = Guid.NewGuid().ToString();
            var dllPath = Path.Join(outputFolderPath, "temp_" + uuid + ".dll");
            comp.Emit(dllPath);
            AnalyzeAssembly(dllPath, ref types);
            //AnalyzeNamespace(comp.GlobalNamespace, ref types, true, comp.SourceModule);
            workspace.CloseSolution();
            return types;
        }

        /// <summary>
        /// Compile and write project settings
        /// </summary>
        /// <param name="path">project root</param>
        /// <param name="outputPath">output folder</param>
        /// <returns>path to the project settings file</returns>
        public static async Task<string> CompileProjectSettings(string path, string outputPath)
        {
            var finalPath = Path.Join(outputPath, "project_settings.json");
            var existingPath = Path.Join(path, "project_settings.json");
            if (File.Exists(existingPath))
            {
                File.Copy(existingPath, finalPath, true);
            }
            else
            {
                var content = JsonConvert.SerializeObject(new ProjectSettings());
                await File.WriteAllTextAsync(finalPath, content);
            }

            return finalPath;
        }
    }
}
