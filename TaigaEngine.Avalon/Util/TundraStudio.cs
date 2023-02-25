using System.Runtime.Loader;
namespace TundraEngine.Studio.Util
{
    public static class TundraStudio
    {
        public static TundraProject? CurrentProject;
        public static readonly ComponentRegistry ComponentRegistry = new ComponentRegistry();
        public static readonly AssemblyLoadContext Asl = new AssemblyLoadContext("TundraProject", true);
    }
}
