using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TundraEngine.Classes.Data;

namespace TundraEngine.Studio.Controls.Editor
{

    public partial class SceneEditor : UserControl
    {
        public SceneResource Scene { get; set; }
        public SceneEditor()
        {
            DataContext = this;
            InitializeComponent();
        }
        public SceneEditor(SceneResource scene)
        {
            Scene = scene;
            DataContext = this;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

