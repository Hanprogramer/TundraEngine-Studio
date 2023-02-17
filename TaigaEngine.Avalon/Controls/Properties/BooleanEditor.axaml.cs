using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TundraEngine.Studio.Controls
{
    public partial class BooleanEditor : UserControl
    {
        public string Label { get; set; } = "Prop.Label";
        public bool Value { get => Data.Value; set => Data.Value = value; } 
        ObjectEditorPropertiesData Data;
        public BooleanEditor(ObjectEditorPropertiesData data)
        {
            DataContext = this;
            Data = data;
            Label = data.Name;
            InitializeComponent();
        }

        public BooleanEditor()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

