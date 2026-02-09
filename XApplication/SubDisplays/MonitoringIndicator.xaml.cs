using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace GameTimeX.XApplication.SubDisplays
{
    /// <summary>
    /// Interaktionslogik für MonitoringIndicator.xaml
    /// </summary>
    public partial class MonitoringIndicator : Window
    {
        public MonitoringIndicator()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var screen = Screen.PrimaryScreen;

            Left = screen.Bounds.Left + 8;
            Top = screen.Bounds.Top + 8;

            var hwnd = new WindowInteropHelper(this).Handle;
            int style = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);

            Win32.SetWindowLong(
                hwnd,
                Win32.GWL_EXSTYLE,
                style | Win32.WS_EX_NOACTIVATE | Win32.WS_EX_TOOLWINDOW
            );
        }
    }

    internal static class Win32
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
