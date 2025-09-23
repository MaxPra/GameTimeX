using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GameTimeX.Function;
using GameTimeX.Objects;
using GameTimeX.XApplication.SubDisplays;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für CreateNew.xaml
    /// </summary>
    public partial class Properties : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        private double cropX;
        private double cropY;
        private double cropWidth;
        private double cropHeight;

        private string oldPicPath = "";
        private string oldGameFolderPath = "";

        string filePath = "";

        private int pid = 0;

        DBObject dBObject = null;

        public SteamGame SteamGame { get; private set; }

        public Properties(int pid)
        {
            this.pid = pid;

            this.dBObject = DataBaseHandler.ReadPID(pid);

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
            if (WindowState == WindowState.Maximized)
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


        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Background = Brushes.CornflowerBlue;
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = Brushes.Transparent;
        }

        private void btnCreateNewProfile_Click(object sender, RoutedEventArgs e)
        {


        }

        private void btnShowFileDialog_Click(object sender, RoutedEventArgs e)
        {
            // File-Dialog öffnen
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ""; // Default file extension
            dialog.Filter = "Pictures (.txt)|*.png;*.jpg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePath = dialog.FileName;
                txtPicPath.Text = filePath;

                ImageCropper imageCropper = new ImageCropper(filePath);
                imageCropper.Owner = this;
                imageCropper.ShowDialog();

                cropX = imageCropper.CropX;
                cropY = imageCropper.CropY;
                cropWidth = imageCropper.CropWidth;
                cropHeight = imageCropper.CropHeight;

            }
        }

        private void btnShowFileDialogExe_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                txtGameFolderPath.Text = dialog.FileName;
            }

            window.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayHandler.CheckDisplay(false, txtProfileName, txtPicPath))

            {
                // Dateinamen in Hash (4 Zeichen) umwandeln
                string fileNameHash = FileHandler.GetHashFromFilename(txtPicPath.Text);

                if (fileNameHash.StartsWith("-"))
                {
                    fileNameHash.TrimStart('-');
                }

                // Werte in Datenbank speichern
                dBObject.GameName = txtProfileName.Text;
                dBObject.ExtGameFolder = txtGameFolderPath.Text;

                // Profil Einstellungen
                CProfileSettings profileSettings = new CProfileSettings();
                profileSettings.HDREnabled = cbEnableHdr.IsChecked == true;
                profileSettings.SteamGameArgs = txtSteamParams.Text.Replace(" ", ";");

                dBObject.ProfileSettings = profileSettings.Serialize();

                // Steam
                if (SteamGame != null)
                    dBObject.SteamAppID = (int)SteamGame.AppId;


                if (txtPicPath.Text != oldPicPath)
                {
                    dBObject.ProfilePicFileName = fileNameHash;

                    // Bild croppen und abspeichern
                    FileHandler.CropImageAndSave(filePath, (int)cropWidth, (int)cropHeight, SysProps.picDestPath, fileNameHash, (int)cropX, (int)cropY);
                }

                DataBaseHandler.Save(dBObject);

                // Executables wählen lassen
                if (oldGameFolderPath != dBObject.ExtGameFolder)
                {
                    CExecutables cExecutables = new CExecutables();
                    cExecutables.Initialize(CExecutables.ConvertListToDictionary(FuncExecutables.GetAllExecutablesFromDirectory(dBObject.ExtGameFolder), true));
                    dBObject.Executables = cExecutables.Serialize();

                    DataBaseHandler.Save(dBObject);

                    ManageExecutables manageExecutables = new ManageExecutables(dBObject.ProfileID);
                    manageExecutables.Owner = this;
                    manageExecutables.ShowDialog();
                }

                // Executables zu diesem Profil holen und dem GameSwitcher mitgeben (nur wenn aktiviert)
                if (oldGameFolderPath != txtGameFolderPath.Text)
                {
                    if (SysProps.gameRunningHandler != null)
                    {
                        List<string> executables = FuncExecutables.GetAllActiveExecutablesFromDBObj(dBObject);
                        SysProps.gameRunningHandler.AddExecutables(dBObject.ProfileID, executables);
                    }
                }

                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtGameFolderPath.Text = dBObject.ExtGameFolder;
            txtPicPath.Text = dBObject.ProfilePicFileName;
            txtProfileName.Text = dBObject.GameName;

            if (SteamLocatorHandler.GetSteamRoot() == null)
            {
                btnImportFromSteam.IsEnabled = false;
                btnImportFromSteam.ToolTip = "Steam is not installed";
            }


            // Steam Profil linked Text
            if (dBObject.SteamAppID == 0)
                bSteamProfileLinked.Visibility = Visibility.Collapsed;

            // Profil Settings laden
            CProfileSettings profileSettings = new CProfileSettings(dBObject.ProfileSettings).Dezerialize();
            cbEnableHdr.IsChecked = profileSettings.HDREnabled;
            txtSteamParams.Text = profileSettings.SteamGameArgs.Replace(";", " ");

            oldPicPath = txtPicPath.Text;
            oldGameFolderPath = txtGameFolderPath.Text;

            if (dBObject.SteamAppID == 0)
            {
                gbProfileSettings.IsEnabled = false;

            }
        }

        private void btnImportFromSteam_Click(object sender, RoutedEventArgs e)
        {
            // Import Fenster öffnen

            SteamImportWindow steamImportWnd = new SteamImportWindow();
            steamImportWnd.Owner = this;
            steamImportWnd.ShowDialog();

            if (steamImportWnd.SelectedGame != null)
            {
                SteamGame = steamImportWnd.SelectedGame;

                txtProfileName.Text = SteamGame.Name;
                txtGameFolderPath.Text = SteamManifestsHandler.ResolveInstallPath(SteamGame);
                cbEnableHdr.Visibility = Visibility.Visible;
            }
            else
            {
                DisableProfileSettings();
            }

            // Wenn kein Steamprofil hinterlegt
            if (SteamGame == null && dBObject.SteamAppID == 0)
            {
                DisableProfileSettings();
                bSteamProfileLinked.Visibility = Visibility.Collapsed;
            }
            else if (SteamGame == null && dBObject.SteamAppID != 0)
            {
                cbEnableHdr.Visibility = Visibility.Visible;
                gbProfileSettings.IsEnabled = true;
                bSteamProfileLinked.Visibility = Visibility.Visible;
            }
            else
            {
                cbEnableHdr.Visibility = Visibility.Visible;
                gbProfileSettings.IsEnabled = true;
                bSteamProfileLinked.Visibility = Visibility.Visible;
            }
        }

        private void btnUnlinkSteamProfile_Click(object sender, RoutedEventArgs e)
        {
            if (dBObject != null)
            {
                dBObject.SteamAppID = 0;

                // Profilsettings
                CProfileSettings profileSettings = new CProfileSettings(dBObject.ProfileSettings).Dezerialize();

                // Hier Steam-abhängige Settings deaktivieren
                profileSettings.HDREnabled = false;

                // Profile Settings serialisieren und in Feld speichern
                dBObject.ProfileSettings = profileSettings.Serialize();

                // Profileinstellungen auf Enabled = false stellen
                DisableProfileSettings();

                bSteamProfileLinked.Visibility = Visibility.Collapsed;
            }
        }

        private void DisableProfileSettings()
        {
            // Enable HDR darf nicht sichtbar sein, wenn Steam-Profil nicht verlinkt
            cbEnableHdr.IsChecked = false;

            txtSteamParams.Text = string.Empty;

            gbProfileSettings.IsEnabled = false;
        }
    }
}
