using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GameTimeX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        int currentSelectedGame = 0;
        bool startStopMonitoringBtnActive = false;

        public MainWindow()
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

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            btnMinimize.Background = Brushes.CornflowerBlue;
        }

        private void btnMaximize_MouseEnter(object sender, MouseEventArgs e)
        {
            btnMaximize.Background = Brushes.CornflowerBlue;
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Background = VisualHandler.convertHexToBrush(SysProps.hexValCloseWindow); 
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = Brushes.Transparent;
        }

        private void btnMaximize_MouseLeave(object sender, MouseEventArgs e)
        {
            btnMaximize.Background = Brushes.Transparent;
        }

        private void btnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            btnMinimize.Background = Brushes.Transparent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Bei Programmstart / Fensterstart System initialisieren
            SysProps.initializeSystem(this);

            // Bei jedem Start unbenutze Bilder löschen (ACHTUNG: Bevor diese verwendet werden!!)
            FileHandler.deleteUnusedImages();

            // DataGrid aufbauen (Gameprofile laden)
            buildDGProfiles();

            DisplayHandler.switchToFirstGameInList(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CreateNew cnWin = new CreateNew();
            cnWin.Owner = this;
            cnWin.ShowDialog();
            
            buildDGProfiles();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = dgProfiles.SelectedItem as Profile;

            if(profile != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to delete '" + profile.ProfileName + "'?", "Delete", "Cancel");
                quest.Owner = this;
                quest.ShowDialog();

                // User hat "Delete" geklickt
                if (quest.returnType == QuestionBox.ReturnType.YES)
                {
                    if (profile != null)
                    {
                        DataBaseHandler.delete(profile.PID);
                        buildDGProfiles();
                        DisplayHandler.switchToFirstGameInList(this);
                    }
                }
            }
        }

        private void buildDGProfiles()
        {

            List<DBObject> profiles = null;

            if (txtSearchBar.Text == "")
            {
                profiles = DataBaseHandler.readAll();
            }
            else
            {
                profiles = DataBaseHandler.readGameName(txtSearchBar.Text);
            }

            dgProfiles.Items.Clear();

            foreach (DBObject dbprofile in profiles)
            {
                Profile profile = new Profile();
                profile.ProfileName = dbprofile.GameName;
                profile.GameTime = dbprofile.GameTime;
                profile.PID = dbprofile.ProfileID;

                dgProfiles.Items.Add(profile);
            }

            DisplayHandler.switchToFirstGameInList(this);
        }


        private void dgProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Derzeit selektierte Zeile holen
            Profile profile = dgProfiles.SelectedItem as Profile;

            if(profile != null)
            {
                // Wird das Profil während der Zeitaufnahme gewechselt, so muss das Monitoring beendet u. gespeichert werden
                if (profile.PID != SysProps.currentSelectedPID && MonitorHandler.currentlyMonitoringGameTime())
                    MonitorHandler.endMonitoringGameTime(btnStartStopMonitoring);

                // Derzeit aktive PID wählen
                SysProps.currentSelectedPID = profile.PID;

                // Info-Display befüllen / aufbauen
                DisplayHandler.buildInfoDisplay(profile.PID, this);
            }            
        }

        private void btnStartStopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (MonitorHandler.currentlyMonitoringGameTime())
            {
                MonitorHandler.endMonitoringGameTime(btnStartStopMonitoring);
                buildDGProfiles();
                DisplayHandler.buildInfoDisplay(SysProps.currentSelectedPID, this);

                btnStartStopMonitoring.Background = new SolidColorBrush((Color)this.FindResource("ButtonDefaultColor"));
                VisualHandler.startStopMonitoringBtnActive = false;
            }
            else
            {
                MonitorHandler.startMonitoringGameTime(btnStartStopMonitoring, SysProps.currentSelectedPID);

                btnStartStopMonitoring.Background = new SolidColorBrush((Color)this.FindResource("ButtonDefaultMonitoringColor"));
                VisualHandler.startStopMonitoringBtnActive = true;
            }
        }

        private void txtSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            buildDGProfiles();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            MonitorHandler.endMonitoringGameTime(btnStartStopMonitoring);
        }

        private void btnEditProfileName_Click(object sender, RoutedEventArgs e)
        {
            Rename rename = new Rename();
            rename.CurrObject = DataBaseHandler.readPID(SysProps.currentSelectedPID);
            rename.Owner = this;
            rename.ShowDialog();

            buildDGProfiles();
            DisplayHandler.buildInfoDisplay(SysProps.currentSelectedPID, this);
        }

        private void btnEditProfileName_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btnEditProfileName_MouseLeave(object sender, MouseEventArgs e)
        {
            btnEditProfileName.Background= Brushes.Transparent;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void scrollBar_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

            MonitorHandler.endMonitoringGameTime(btnStartStopMonitoring);

            // Settings öffnen
            Settings settings = new Settings();
            settings.Owner = this;
            settings.ShowDialog();
        }

        private void btnStartStopMonitoring_MouseEnter(object sender, MouseEventArgs e)
        {

            if (VisualHandler.startStopMonitoringBtnActive)
            {
                // Monitoring aktiv Hover
                Storyboard sb = this.FindResource("StartStopButtonHoverMonitoring") as Storyboard;
                sb.Begin();
            }
            else
            {
                // Default Hover
                Storyboard sb = this.FindResource("StartStopButtonHoverNotMonitoring") as Storyboard;
                sb.Begin();
            }

        }

        private void btnStartStopMonitoring_MouseLeave(object sender, MouseEventArgs e)
        {

            if (VisualHandler.startStopMonitoringBtnActive)
            {
                // Monitoring aktiv Hover
                Storyboard sb = this.FindResource("StartStopButtonNoHoverMonitoring") as Storyboard;
                sb.Begin();
            }
            else
            {
                // Default Hover
                Storyboard sb = this.FindResource("StartStopButtonNoHoverNotMonitoring") as Storyboard;
                sb.Begin();
            }
        }

        private void btnGameTimeQuestion_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGameTimeInfo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
