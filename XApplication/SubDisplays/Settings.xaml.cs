using GameTimeX.Function;
using GameTimeX.Objects;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using static GameTimeX.Objects.KeyInput;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        public Settings()
        {
            InitializeComponent();
        }

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
            // StartUpParms speichern
            SysProps.startUpParms.SessionGameTime = (bool)cbSessionGameTime.IsChecked;
            SysProps.startUpParms.AutoProfileSwitching = (bool)cbGameSwitcher.IsChecked;
            SysProps.startUpParms.ShowToastNotification = (bool)cbShowToastNotification.IsChecked;
            
            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

            Close();
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Background = VisualHandler.ConvertHexToBrush(SysProps.hexValCloseWindow);
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = Brushes.Transparent;
        }

        private void btnChooseGameFolder_Click(object sender, RoutedEventArgs e)
        {

            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();   
            if (result == CommonFileDialogResult.Ok)
            {
                txtBackupPath.Text = dialog.FileName;
            }

            window.Focus();

            if (txtBackupPath.Text.Length > 0)
            {
                btnCreateBackup.IsEnabled = true;
                cbAutoBackup.IsEnabled = true;
            }
                
        }

        private void btnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            // JSON befüllen (Backup Typ + Backup Pfad)
            // Danach speichern
            SysProps.startUpParms.BackupType = Objects.StartUpParms.BackupTypes.CREATE_BACKUP;
            SysProps.startUpParms.BackupPath = txtBackupPath.Text;
            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

            // Zeige Info Meldung bezüglich automatisches Neustarten der Applikation
            InfoBox info = new InfoBox("GameTimeX will be restarted automatically now!");
            info.Owner = this;
            info.ShowDialog();

            SysProps.RestartApplication();
        }

        private void btnChooseBackUpFile_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = false;
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                txtBackupPathImport.Text = dialog.FileName;
            }

            window.Focus();

            if (txtBackupPathImport.Text.Length > 0)
            {
                btnImportBackup.IsEnabled = true;
                lblBackUpDate.Text = "Backup date of file: " + File.GetCreationTime(txtBackupPathImport.Text).ToString("f");
            }
                
        }

        private void btnImportBackup_Click(object sender, RoutedEventArgs e)
        {
            // JSON befüllen (Backup Typ + Backup Pfad)
            // Danach speichern
            SysProps.startUpParms.BackupType = Objects.StartUpParms.BackupTypes.IMPORT_BACKUP;
            SysProps.startUpParms.BackUpImportPath = txtBackupPathImport.Text;
            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

            // Zeige Info Meldung bezüglich automatisches Neustarten der Applikation
            InfoBox info = new InfoBox("GameTimeX will be restarted automatically now!");
            info.Owner = this;
            info.ShowDialog();

            SysProps.RestartApplication();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // SessionGameTime laden
            cbSessionGameTime.IsChecked = SysProps.startUpParms.SessionGameTime;

            // Monitor Key
            cbMonitoringKeyActive.IsChecked = SysProps.startUpParms.MonitorShortcutActive;
            lblCurrentKey.Text = "Current key: " + KeyInput.virtualKeyMap[SysProps.startUpParms.MonitorShortcut];
            btnMonitoringKey.IsEnabled = SysProps.startUpParms.MonitorShortcutActive;
            cbGameSwitcher.IsChecked = SysProps.startUpParms.AutoProfileSwitching;
            cbShowToastNotification.IsChecked = SysProps.startUpParms.ShowToastNotification;

            if ((bool)cbMonitoringKeyActive.IsChecked)
            {
                cbShowToastNotification.IsEnabled = true;
            }

            // Pfade befüllen
            string backUpPath = SysProps.startUpParms.BackupPath;
            if (Directory.Exists(backUpPath))
            {
                txtBackupPath.Text = backUpPath;
            }

            string backUpImportPath = SysProps.startUpParms.BackUpImportPath;
            if (File.Exists(backUpImportPath))
            {
                txtBackupPathImport.Text = backUpImportPath;
            }
            
            // Button Enabled handeln
            if(txtBackupPath.Text != string.Empty)
            {
                btnCreateBackup.IsEnabled = true;
                cbAutoBackup.IsEnabled = true;
                cbAutoBackup.IsChecked = SysProps.startUpParms.AutoBackup;
            }

            if(txtBackupPathImport.Text != string.Empty)
            {
                btnImportBackup.IsEnabled=true;
                lblBackUpDate.Text = "Backup date of file: " + File.GetCreationTime(txtBackupPathImport.Text).ToString("f");
               
            }
        }

        private void btnMonitorShortcut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMonitoringKey_Click(object sender, RoutedEventArgs e)
        {
            MonitorKey monitorKeyWnd = new MonitorKey(this);
            monitorKeyWnd.Owner = this;
            monitorKeyWnd.Focus();
            monitorKeyWnd.ShowDialog();
            
            

            KeyInput.VirtualKey key = monitorKeyWnd.key;

            lblCurrentKey.Text = "Current key: " + KeyInput.virtualKeyMap[key];

            SysProps.startUpParms.MonitorShortcut = key;
            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);
        }

        private void cbMonitoringKeyActive_Click(object sender, RoutedEventArgs e)
        {
            if (cbMonitoringKeyActive.IsChecked == true)
            {
                btnMonitoringKey.IsEnabled = true;
                SysProps.startUpParms.MonitorShortcutActive = true;
                cbShowToastNotification.IsEnabled = true;
            }
            else
            {
                btnMonitoringKey.IsEnabled = false;
                SysProps.startUpParms.MonitorShortcutActive = false;
                cbShowToastNotification.IsEnabled = false;
            }

            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);
        }

        private void cbAutoBackup_Click(object sender, RoutedEventArgs e)
        {
            if(cbAutoBackup.IsChecked == true)
            {
                SysProps.startUpParms.AutoBackup = true;
            }
            else
            {
                SysProps.startUpParms.AutoBackup = false;
            }

            SysProps.startUpParms.BackupPath = txtBackupPath.Text;
            FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);
        }

        private void btnShowDataFolder_Click(object sender, RoutedEventArgs e)
        {
            // Explorer zum Datenordner öffnen
            Process.Start("explorer.exe", SysProps.programPathFolder);
        }
    }
}
