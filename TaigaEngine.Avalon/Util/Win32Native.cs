using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Win32;
using System;
using System.Runtime.InteropServices;

namespace TundraEngine.Studio.Util
{
    public static class Win32Native
    {
        public enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PassiveUpdateMode,
            UseHostBackdropBrush,
            UseImmersiveDarkMode = 20,
            WindowCornerPreference = 33,
            BorderColor,
            CaptionColor,
            TextColor,
            VisibleFrameBorderThickness,
            SystemBackdropType,
            Last
        }
        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        public static void ImplementDarkTitleBar(Control control)
        {
            var toplevel = (TopLevel)control.GetVisualRoot();
            if (toplevel.PlatformImpl is WindowImpl platformImpl)
            {
                var handle = platformImpl.Handle.Handle;
                var val = 1;
                DwmSetWindowAttribute(handle, DWMWINDOWATTRIBUTE.UseImmersiveDarkMode, ref val, 4);
            }
        }

        public static double GetScreenScaling(Control control)
        {
            var toplevel = (TopLevel)control.GetVisualRoot();
            if (toplevel.PlatformImpl is WindowImpl platformImpl)
            {
                return platformImpl.DesktopScaling;
            }
            return 1;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDPIAware();
    }
}
