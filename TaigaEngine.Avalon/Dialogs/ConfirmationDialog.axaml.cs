using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace TundraEngine.Studio.Dialogs
{
    public partial class ConfirmationDialog : Window
    {
        public string Message { get; set; } = "Dialog.Message";
        public string PositiveAction { get; set; } = "Yes";
        public string NegativeAction { get; set; } = "No";
        public bool IsDangerous { get; set; } = false;
        public bool ShowCancel { get; set; } = true;
        public bool ShowNegative { get; set; } = false;
        public ConfirmationDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ConfirmationDialog(string message, string? positiveAction = null, string? negativeAction = null, bool showCancel = true, bool showNegative = false, bool isDangerous = false)
        {
            Message = message;
            PositiveAction = positiveAction ?? "Yes";
            NegativeAction = negativeAction ?? "No";
            ShowCancel = showCancel;
            IsDangerous = isDangerous;
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

        public void OnCancel(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static async Task<bool?> Show(string title, string message, Window owner, string? positiveAction = null, string? negativeAction = null, bool showCancel = true, bool showNegative = false, bool isDangerous = false)
        {
            var dlg = new ConfirmationDialog(message, positiveAction, negativeAction, showCancel, showNegative, isDangerous);
            dlg.Title = title;
            return await dlg.ShowDialog<bool?>(owner);
        }
    }
}
