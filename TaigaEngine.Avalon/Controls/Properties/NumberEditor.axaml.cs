using Avalonia.Controls;

namespace TundraEngine.Studio.Controls
{
    public partial class NumberEditor : UserControl
    {
        public string Label { get; set; } = "Prop.Label";
        public float Value { get => Data.Value; set { Data.Value = value; } }
        ObjectEditorPropertiesData Data;
        public NumberEditor()
        {
            InitializeComponent();
            DataContext = this;
        }
        public NumberEditor(ObjectEditorPropertiesData data)
        {
            Data = data;
            Label = data.Name;

            // Set default value
            if (Data.Value == null)
                Value = 0;

            InitializeComponent();
            DataContext = this;
        }
    }
}
