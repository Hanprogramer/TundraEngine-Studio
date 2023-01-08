using ReactiveUI;
using System.Collections.ObjectModel;

namespace TundraEngine.Studio
{
    public class MainWindowViewModel : ReactiveObject
    {
        ObservableCollection<EditorTab> _tabs = new();
        public ObservableCollection<EditorTab> Tabs { get => _tabs; set => this.RaiseAndSetIfChanged(ref _tabs, value); }
    }
}
