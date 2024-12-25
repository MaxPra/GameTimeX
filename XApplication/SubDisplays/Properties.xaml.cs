using GameTimeX.Function;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        public Properties(int pid)
        {
            this.pid = pid;

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
                DBObject dbObj = DataBaseHandler.ReadPID(pid);
                dbObj.GameName = txtProfileName.Text;
                dbObj.ExtGameFolder = txtGameFolderPath.Text;
                

                if(txtPicPath.Text != oldPicPath)
                {
                    dbObj.ProfilePicFileName = fileNameHash;

                    // Bild croppen und abspeichern
                    FileHandler.CropImageAndSave(filePath, (int)cropWidth, (int)cropHeight, SysProps.picDestPath, fileNameHash, (int)cropX, (int)cropY);
                }
                

                // Executables zu diesem Profil holen und dem GameSwitcher mitgeben (nur wenn aktiviert)
                if (SysProps.startUpParms.AutoProfileSwitching && (oldGameFolderPath != txtGameFolderPath.Text))
                {
                    if (SysProps.gameSwitcherHandler != null)
                    {
                        List<string> executables = GameSwitcherHandler.GetAllExecutablesFromDirectory(txtGameFolderPath.Text);
                        SysProps.gameSwitcherHandler.AddExecutables(dbObj.ProfileID, executables);
                    }
                }

                DataBaseHandler.Save(dbObj);



                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DBObject obj = DataBaseHandler.ReadPID(pid);
            txtGameFolderPath.Text = obj.ExtGameFolder;
            txtPicPath.Text = obj.ProfilePicFileName;
            txtProfileName.Text = obj.GameName;

            oldPicPath = txtPicPath.Text;
            oldGameFolderPath = txtGameFolderPath.Text;
        }
    }
}
