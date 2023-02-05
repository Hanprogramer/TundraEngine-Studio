using Avalonia.Controls;

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

        public static void Show(string message, Window? parent)
        {
            var msg = new MessageBox();
            msg.Message = message;
            if (parent != null)
                msg.ShowDialog(parent);
            else
                msg.Show();
        }
    }
}
