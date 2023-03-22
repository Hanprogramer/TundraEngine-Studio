namespace TundraEngine.Studio.Controls
{
    public interface IPropertyEditor
    {
        public object? GetPropertyValue();
        public delegate void OnPropertyChangedHandler();
        public event OnPropertyChangedHandler OnPropertyChanged;
    }
}
