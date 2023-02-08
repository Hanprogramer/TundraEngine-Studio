using Avalonia.Controls;

namespace TundraEngine.Studio.Controls
{
    public partial class NumberEditor : UserControl
    {
        public string Label { get; set; } = "Prop.Label";
        public float Value { get; set; } = 0;
        public NumberEditor()
        {
            InitializeComponent();
            DataContext = this;
        }
        public NumberEditor(string label, float value)
        {
            Label = label;
            Value = value;
            InitializeComponent();
            DataContext = this;
        }
    }
}
