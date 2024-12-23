using GameTimeX.Function;
using GameTimeX.Objects;
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

            // KeyInputHandler stoppen
            SysProps.StopKeyInputHandler();

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
            btnClose.Background = VisualHandler.ConvertHexToBrush(SysProps.hexValCloseWindow); 
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
            SysProps.InitializeSystem(this);

            // Bei jedem Start unbenutze Bilder löschen (ACHTUNG: Bevor diese verwendet werden!!)
            FileHandler.DeleteUnusedImages();

            // DataGrid aufbauen (Gameprofile laden)
            BuildDGProfiles();

            DisplayHandler.SwitchToFirstGameInList(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CreateNew cnWin = new CreateNew();
            cnWin.Owner = this;
            cnWin.ShowDialog();
            
            BuildDGProfiles();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = dgProfiles.SelectedItem as Profile;

            if(profile != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to Delete '" + profile.ProfileName + "'?", "Delete", "Cancel");
                quest.Owner = this;
                quest.ShowDialog();

                // User hat "Delete" geklickt
                if (quest.UsrReturnType == QuestionBox.ReturnType.YES)
                {
                    if (profile != null)
                    {
                        DataBaseHandler.Delete(profile.PID);
                        BuildDGProfiles();
                        DisplayHandler.SwitchToFirstGameInList(this);
                    }
                }
            }
        }

        private void BuildDGProfiles()
        {

            List<DBObject> profiles = null;

            if (txtSearchBar.Text == "")
            {
                profiles = DataBaseHandler.ReadAll();
            }
            else
            {
                profiles = DataBaseHandler.ReadGameName(txtSearchBar.Text);
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

            DisplayHandler.SwitchToFirstGameInList(this);

            if(profiles.Count == 0)
            {
                DisplayHandler.BuildInfoDisplayNoGame(this);
            }
        }


        private void dgProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Derzeit selektierte Zeile holen
            Profile profile = dgProfiles.SelectedItem as Profile;

            if(profile != null)
            {
                // Wird das Profil während der Zeitaufnahme gewechselt, so muss das Monitoring beendet u. gespeichert werden
                if (profile.PID != SysProps.currentSelectedPID && MonitorHandler.CurrentlyMonitoringGameTime())
                    MonitorHandler.EndMonitoringGameTime(this);

                // Derzeit aktive PID wählen
                SysProps.currentSelectedPID = profile.PID;

                // Info-Display befüllen / aufbauen
                DisplayHandler.BuildInfoDisplay(profile.PID, this);
            }            
        }

        private void btnStartStopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (MonitorHandler.CurrentlyMonitoringGameTime())
            {
                MonitorHandler.EndMonitoringGameTime(this);
                BuildDGProfiles();
                DisplayHandler.BuildInfoDisplay(SysProps.currentSelectedPID, this);

                btnStartStopMonitoring.Background = new SolidColorBrush((Color)this.FindResource("ButtonDefaultColor"));
                VisualHandler.startStopMonitoringBtnActive = false;
            }
            else
            {
                MonitorHandler.StartMonitoringGameTime(this, SysProps.currentSelectedPID);

                btnStartStopMonitoring.Background = new SolidColorBrush((Color)this.FindResource("ButtonDefaultMonitoringColor"));
                VisualHandler.startStopMonitoringBtnActive = true;
            }
        }

        private void txtSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            BuildDGProfiles();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            MonitorHandler.EndMonitoringGameTime(this);
        }

        private void btnEditProfileName_Click(object sender, RoutedEventArgs e)
        {
            Rename rename = new Rename();
            rename.CurrObject = DataBaseHandler.ReadPID(SysProps.currentSelectedPID);
            rename.Owner = this;
            rename.ShowDialog();

            BuildDGProfiles();
            DisplayHandler.BuildInfoDisplay(SysProps.currentSelectedPID, this);
        }

        private void scrollBar_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Wenn derzeit die Spielzeit aufgenommen wird --> beenden
            if(MonitorHandler.CurrentlyMonitoringGameTime())
                MonitorHandler.EndMonitoringGameTime(this);

            // Vor öffnen den InputHandler beenden!
            if (SysProps.keyInputHandler != null)
                SysProps.keyInputHandler.StopListening();

            // Settings öffnen
            Settings settings = new Settings();
            settings.Owner = this;
            settings.ShowDialog();

            // Nach Schließen des Settings-Windows keyInputHanlder entweder starten oder beenden!
            if(SysProps.keyInputHandler != null)
            {
                // Wenn Monitor Key aktiv und als Key nicht (kein Key) ausgewühlt wurde --> KeyInputHandler starten
                if (SysProps.startUpParms.MonitorShortcutActive && SysProps.startUpParms.MonitorShortcut != Objects.KeyInput.VirtualKey.VK_NONE)
                {
                    SysProps.keyInputHandler = new Function.KeyInputHandler(SysProps.startUpParms.MonitorShortcut, this);
                    SysProps.keyInputHandler.StartListening();
                }
                // Wenn Monitor Key nicht aktiv oder Monitor Key = (kein Key) --> Stoppen
                else if (!SysProps.startUpParms.MonitorShortcutActive || SysProps.startUpParms.MonitorShortcut == KeyInput.VirtualKey.VK_NONE)
                {
                    SysProps.keyInputHandler.StopListening();
                }
            }
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

        private void btnGameTimeInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lblChangeProfileImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // File-Dialog öffnen
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ""; // Default file extension
            dialog.Filter = "Pictures (.txt)|*.png;*.jpg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            string filePath = string.Empty;
            double cropX = 0;
            double cropY = 0;
            double cropWidth = 0;
            double cropHeight = 0; 

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePath = dialog.FileName;

                ImageCropper imageCropper = new ImageCropper(filePath);
                imageCropper.Owner = this;
                imageCropper.ShowDialog();

                cropX = imageCropper.CropX;
                cropY = imageCropper.CropY;
                cropWidth = imageCropper.CropWidth;
                cropHeight = imageCropper.CropHeight;

                // Dateinamen in Hash (4 Zeichen) umwandeln
                string fileNameHash = FileHandler.GetHashFromFilename(filePath);

                if (fileNameHash.StartsWith("-"))
                {
                    fileNameHash.TrimStart('-');
                }

                // Werte in Datenbank speichern
                DBObject dbObj = DataBaseHandler.ReadPID(SysProps.currentSelectedPID);
                dbObj.ProfilePicFileName = fileNameHash;
                DataBaseHandler.Save(dbObj);

                // Bild croppen und abspeichern
                FileHandler.CropImageAndSave(filePath, (int)cropWidth, (int)cropHeight, SysProps.picDestPath, fileNameHash, (int)cropX, (int)cropY);

                DisplayHandler.BuildInfoDisplay(SysProps.currentSelectedPID, this);

            }
        }
    }
}
