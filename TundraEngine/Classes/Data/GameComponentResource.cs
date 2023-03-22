using System.Reflection;
using System.Runtime.Loader;
using TundraEngine.Components;
namespace TundraEngine.Classes.Data
{
    public class GameComponentResource
    {
        public static string TUNDRA_ENGINE = typeof(TundraEngine.Game).Assembly.GetName().Name;
        public static Assembly TUNDRA_ENGINE_ASSEMBLY = typeof(TundraEngine.Game).Assembly;
        public string component { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public Component Instantiate(GameObject obj, ResourceManager rm, AssemblyLoadContext? asl = null)
        {
            Type? type = null;
            // First look on Tundra Engine assemblies
            if (component.StartsWith(TUNDRA_ENGINE))
            {
                type = TUNDRA_ENGINE_ASSEMBLY.GetType(component);
            }
            // Find the component from another assembly (still in the engine)
            var asm = GetAssemblyByName(component);
            if (asm != null)
            {
                type = asm.GetType(component);
            }
            else
            {
                // Find the assembly in asl if exists
                if (asl != null)
                {
                    asm = GetAssemblyByName(component, asl);
                    if (asm != null)
                    {
                        type = asm.GetType(component);
                    }
                }
                //throw new Exception($"Can't find the assembly host of {component} or assembly isn't loaded");
            }
            //throw new Exception($"Failed to create component ({component})");
            
            
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(Component)))
                {
                    var ctr = type.GetConstructor(new Type[] { typeof(GameObject) });
                    if (ctr == null)
                        throw new Exception($"Class {this.component} doesn't have a (GameObject){{}} constructor");
                    var compInst = ctr.Invoke(new object[] { obj });
                    if (compInst is SpriteRenderer sr)
                    {
                        var texProp = (string)properties["Sprite"]!;
                        var tex = rm.GetTexture(texProp);
                        sr.Texture = tex;
                    }
                    obj.AddComponent((Component)compInst);
                    return (Component)compInst;
                }
                throw new Exception($"Class {component} is not a Component");
            }
            throw new Exception($"Can't find component class {component} in any assemblies");
        }
        
        public static Assembly? GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                FirstOrDefault(assembly => assembly.GetName().Name == name || assembly.GetName().Name.StartsWith(name) || name.StartsWith(assembly.GetName().Name));
        }
        public static Assembly? GetAssemblyByName(string name, AssemblyLoadContext asl)
        {
            return asl.Assemblies.
                FirstOrDefault(assembly => assembly.GetName().Name == name || assembly.GetName().Name.StartsWith(name) || name.StartsWith(assembly.GetName().Name));
        }
    }
}
