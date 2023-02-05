using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Threading.Tasks;
using TundraEngine.Studio.Controls;

namespace TundraEngine.Studio.Dialogs
{
    public partial class RenameDialog : Window
    {
        public string Original = "";
        public string FileName { get; set; } = "";
        public string FilePath = "";
        public RenameDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public RenameDialog(string fileName)
        {
            this.FileName = fileName;
            Original = fileName;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnYes(object? sender, RoutedEventArgs e)
        {
            var oldFolder = Path.GetDirectoryName(FilePath);
            var newPath = Path.Join(oldFolder, FileName);
            if (File.Exists(newPath) || Directory.Exists(newPath))
            {
                if (await ConfirmationDialog.Show("File exists!", $"File '{FileName}' already exsists, overwrite?", this) == true)
                {
                    this.Close(FileName);
                }
            }
            else
            {
                this.Close(FileName);
            }
        }
        public void OnNo(object? sender, RoutedEventArgs e)
        {
            this.Close("");
        }

        public static async Task<string> Show(string title, string fileName, FileBrowserItem item, Window owner)
        {
            var dlg = new RenameDialog(fileName);
            dlg.Title = title;
            dlg.FilePath = item.Path;
            return await dlg.ShowDialog<string>(owner);
        }
    }
}
