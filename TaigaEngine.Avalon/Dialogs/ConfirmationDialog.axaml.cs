using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace TundraEngine.Studio.Dialogs
{
    public partial class ConfirmationDialog : Window
    {
        public string Message { get; set; } = "Dialog.Message";
        public ConfirmationDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ConfirmationDialog(string message)
        {
            this.Message = message;
            InitializeComponent();
            DataContext = this;
        }

        public void OnYes(object? sender, RoutedEventArgs e)
        {
            this.Close(true);
        }
        public void OnNo(object? sender, RoutedEventArgs e)
        {
            this.Close(false);
        }

        public static async Task<bool> Show(string title, string message, Window owner)
        {
            var dlg = new ConfirmationDialog(message);
            dlg.Title = title;
            return await dlg.ShowDialog<bool>(owner);
        }
    }
}
