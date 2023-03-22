using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TundraEngine.Studio.Controls
{
    public partial class StringPropEditor : UserControl, IPropertyEditor
    {
        public string Label { get; set; } = "Prop.Label";
        public string Value { get => Data.Value; set { Data.Value = value; } }
        ObjectEditorPropertiesData Data;
        
        public StringPropEditor(ObjectEditorPropertiesData data)
        {
            Data = data;
            Label = data.Name;
            DataContext = this;
            InitializeComponent();
        }
        public StringPropEditor()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public object? GetPropertyValue()
        {
            return Value;
        }
        public event IPropertyEditor.OnPropertyChangedHandler? OnPropertyChanged;
    }
}

