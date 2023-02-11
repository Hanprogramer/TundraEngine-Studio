using Newtonsoft.Json;

namespace TundraEngine.Classes.Data
{
    public enum ResourceType
    {
        RawFile = 0,
        Object = 1,
        Scene = 2,
        Sprite = 3
    }
    public class Resource
    {
        public virtual string uuid { get; set; }
        public ResourceType resource_type { get; set; }
        public string? path { get; set; }
        public object? data { get; set; }

        public int format_version { get; set; } = 1;

        public string? filename
        {
            get
            {
                if (path != null)
                    return Path.GetFileName(path);
                return null;
            }
        }

        public Resource(string uuid, ResourceType resourceType = ResourceType.RawFile)
        {
            this.uuid = uuid;
            resource_type = resourceType;
        }

        public static async  Task<T> Load<T>(string path, string extension) where T: Resource
        {
            if (!path.EndsWith(extension)) throw new FileLoadException($"Can't load files other than {extension}");
            var res = JsonConvert.DeserializeObject<Resource>(await File.ReadAllTextAsync(path));
            if (res != null)
            {
                var data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(res.data));
                if (data != null)
                {
                    data.path = path;
                    return data;
                }
            }
            throw new Exception($"Failed to load Sprite resource {path}");
        }

    }




}
