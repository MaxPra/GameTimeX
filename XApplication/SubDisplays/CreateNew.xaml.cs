using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function;
using GameTimeX.Function.Utils;
using GameTimeX.Objects.Components;
using GameTimeX.XApplication.SubDisplays;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für CreateNew.xaml
    /// </summary>
    public partial class CreateNew : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        private double cropX;
        private double cropY;
        private double cropWidth;
        private double cropHeight;

        public int ProfileID { get; set; }

        string filePath = "";

        private SteamGame SteamGame { get; set; }

        public CreateNew()
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

            if (DisplayHandler.CheckDisplay(false, txtProfileName, txtPicPath))

            {
                // Dateinamen in Hash (4 Zeichen) umwandeln
                string fileNameHash = FileHandler.GetHashFromFilename(txtPicPath.Text);

                if (fileNameHash.StartsWith("-"))
                {
                    fileNameHash.TrimStart('-');
                }

                // Werte in Datenbank speichern
                DBO_Profile dbo_Profile = DM_Profile.CreateNew();
                dbo_Profile.GameName = txtProfileName.Text;
                dbo_Profile.ProfilePicFileName = fileNameHash;
                dbo_Profile.ExtGameFolder = txtGameFolderPath.Text;

                // Steam
                if (SteamGame != null)
                    dbo_Profile.SteamAppID = (int)SteamGame.AppId;

                CExecutables cExecutables = new CExecutables();
                cExecutables.Initialize(CExecutables.ConvertListToDictionary(FuncExecutables.GetAllExecutablesFromDirectory(dbo_Profile.ExtGameFolder), true));
                dbo_Profile.Executables = cExecutables.Serialize();

                DM_Profile.Save(dbo_Profile);

                // Bild croppen und abspeichern
                FileHandler.CropImageAndSave(filePath, (int)cropWidth, (int)cropHeight, SysProps.picDestPath, fileNameHash, (int)cropX, (int)cropY);

                // Executables wählen lassen
                if (dbo_Profile.ExtGameFolder != string.Empty)
                {
                    ManageExecutables manageExecutables = new ManageExecutables(dbo_Profile.ProfileID);
                    manageExecutables.Owner = this;
                    manageExecutables.ShowDialog();
                }

                // Executables zu diesem Profil holen und dem GameSwitcher mitgeben (nur wenn aktiviert)
                if (SysProps.gameRunningHandler != null)
                {
                    List<string> executables = FuncExecutables.GetAllActiveExecutablesFromDBObj(dbo_Profile);
                    SysProps.gameRunningHandler.AddExecutables(dbo_Profile.ProfileID, executables);
                }

                ProfileID = dbo_Profile.ProfileID;

                Close();
            }
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

                bSteamProfileLinked.Visibility = Visibility.Visible;
            }
            else
            {
                bSteamProfileLinked.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bSteamProfileLinked.Visibility = Visibility.Collapsed;
        }
    }
}
