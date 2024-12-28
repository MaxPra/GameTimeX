using System;
using System.Runtime.InteropServices;
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für CreateNew.xaml
    /// </summary>
    public partial class ToastMessage : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        public int Pid { get; set; }

        private string message = "";
        private string title = "";

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOACTIVATE = 0x0010;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public ToastMessage(string title, string message)
        {
            InitializeComponent();

            this.title = title;
            this.message = message;

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = 0;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }

        public void ShowWithoutFocus()
        {
            this.Show();

            IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // Positioniere das Fenster ohne Aktivierung
            SetWindowPos(hWnd, HWND_TOPMOST, (int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height, SWP_NOACTIVATE);
        }

        /// <summary>
        /// Event wird aufgerufen, wenn linker Mousebutton geklickt wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Text = this.title;
            Message.Text = this.message;
        }
    }
}
