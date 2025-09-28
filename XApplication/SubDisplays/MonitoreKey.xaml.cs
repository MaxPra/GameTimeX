using System.Windows;
using System.Windows.Input;
using GameTimeX.Function;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Objects;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für CreateNew.xaml
    /// </summary>
    public partial class MonitorKey : Window
    {

        private KeyInputHandler keyInputHandler = null;

        public KeyInput.VirtualKey key;

        public MonitorKey(Settings settingsWnd)
        {
            InitializeComponent();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SysProps.keyInputHandler != null)
                SysProps.keyInputHandler.StopListening();

            // Neuen InputHandler instanziieren
            keyInputHandler = new KeyInputHandler(this);
            keyInputHandler.StartListening();
        }
    }
}
