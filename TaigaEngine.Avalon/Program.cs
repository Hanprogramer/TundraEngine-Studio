using Avalonia;
using System;
using System.Runtime.InteropServices;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if DEBUG
                Win32Native.AllocConsole();
#endif
                Win32Native.SetProcessDPIAware();
            }
            BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions()
                {
                    UseWgl = true,
                    AllowEglInitialization = true,
                    UseDeferredRendering = false,
                    UseWindowsUIComposition = false
                })
                .UseSkia()
                .LogToTrace();
    }
}
