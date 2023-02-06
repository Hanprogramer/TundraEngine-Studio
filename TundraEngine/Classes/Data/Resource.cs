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

        public Resource(string uuid, ResourceType resourceType = ResourceType.RawFile)
        {
            this.uuid = uuid;
            resource_type = resourceType;
        }
    }




}
