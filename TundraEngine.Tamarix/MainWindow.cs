using Tamarix;

namespace TundraEngine.Tamarix
{
    public class MainWindow : Window
    {
        public MainWindow(string title, int width, int height, string? icon = null, Silk.NET.Windowing.IView? view = null, bool isAndroid = false) : base(title, width, height, icon, view, isAndroid)
        {
            var content = new LayoutInflater().Inflate(ResourceManager.ReadAssetFile(GetType(), "MainWindow.xml"), "MainWindow");
            SetView(content);
        }
    }
}
