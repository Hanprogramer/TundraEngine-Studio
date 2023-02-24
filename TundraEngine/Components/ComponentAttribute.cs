namespace TundraEngine.Components
{
    [System.AttributeUsage(
        AttributeTargets.Class)]
    public class ComponentAttribute : System.Attribute
    {
        public bool DisplayOnEditor = true;
        public string? Name = null;
    }
}
