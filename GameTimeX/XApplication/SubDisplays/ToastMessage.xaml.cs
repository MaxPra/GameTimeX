using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        private const uint SWP_NOACTIVATE = 0x0010;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        // P/Invoke Deklaration für ShowWindow
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        // Constants für ShowWindow
        private const int SW_SHOWNOACTIVATE = 4; // Fenster anzeigen, ohne Fokus zu stehlen

        public new void Show()
        {
            // Handle des Fensters holen
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            IntPtr hWnd = helper.Handle;

            // Fenster anzeigen, ohne Fokus zu stehlen
            ShowWindow(hWnd, SW_SHOWNOACTIVATE);

            // Basis-Show-Funktion aufrufen (damit es sichtbar ist)
            base.Show();
        }

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

            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // Setze das Fenster in den nicht-aktivierbaren Modus
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_NOACTIVATE);
        }

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

            AnimateOpen(this.Left, 0.5);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Verhindere das sofortige Schließen des Fensters
            e.Cancel = true;

            // Starte die Schließen-Animation
            AnimateClose(SystemParameters.PrimaryScreenWidth, 0.5);
        }



        // Methode für die Öffnen-Animation
        public void AnimateOpen(double targetLeft, double durationInSeconds)
        {
            // Position (von rechts nach Zielposition)
            var animationLeft = new DoubleAnimation
            {
                From = SystemParameters.PrimaryScreenWidth, // Start außerhalb des Bildschirms
                To = targetLeft,                            // Zielposition
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            // Transparenz (von unsichtbar zu sichtbar)
            var animationOpacity = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            // Animation starten
            this.BeginAnimation(Window.LeftProperty, animationLeft);
            this.BeginAnimation(Window.OpacityProperty, animationOpacity);
        }

        // Methode für die Schließen-Animation
        public void AnimateClose(double targetLeft, double durationInSeconds)
        {
            // Position (aktuelle Position nach rechts hinaus)
            var animationLeft = new DoubleAnimation
            {
                From = this.Left,       // Start bei der aktuellen Position
                To = targetLeft,        // Ziel außerhalb des Bildschirms
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            // Transparenz (von sichtbar zu unsichtbar)
            var animationOpacity = new DoubleAnimation
            {
                From = this.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            // Nach Abschluss der Schließen-Animation das Fenster schließen
            animationOpacity.Completed += (s, e) => this.Close();

            // Animation starten
            this.BeginAnimation(Window.LeftProperty, animationLeft);
            this.BeginAnimation(Window.OpacityProperty, animationOpacity);
        }
    }
}
