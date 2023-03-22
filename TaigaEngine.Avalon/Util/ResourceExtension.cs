using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TundraEngine.Classes.Data;
namespace TundraEngine.Studio.Util
{
    public static class ResourceExtension
    {
        public static async Task SaveToFile(this Resource res)
        {
            var resource = new Resource(res.uuid, res.resource_type)
            {
                data = res
            };
            var resContent = JsonConvert.SerializeObject(resource, Formatting.Indented);
            await File.WriteAllTextAsync(res.path, resContent);
        }
    }
}
