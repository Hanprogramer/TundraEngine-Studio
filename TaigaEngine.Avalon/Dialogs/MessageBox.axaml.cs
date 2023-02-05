using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TundraEngine.Studio.Dialogs
{
    public partial class MessageBox : Window
    {
        public string Message { get; set; } = "This is a message";
        public MessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageBox(string message)
        {
            Message = message;
            InitializeComponent();
            DataContext = this;
        }

        public void OnOk(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string title, string message, Window? parent)
        {
            var msg = new MessageBox(message);
            msg.Title = title;
            if (parent != null)
                msg.ShowDialog(parent);
            else
                msg.Show();
        }
    }
}
