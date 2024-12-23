using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameTimeX
{
    internal class DisplayHandler
    {
        public static bool CheckDisplay(bool showMessageAfterCheck, params Control[] controls)
        {
            bool emptyFields = false;

            foreach (var control in controls)
            {
                // Übergebenes Control ist Textbox
                if (control is TextBox)
                {
                    TextBox txtBox = (TextBox)control;
                    if (String.IsNullOrEmpty(txtBox.Text))
                    {
                        txtBox.BorderBrush = SysProps.emptyFieldsColor;
                        emptyFields = true;
                    }
                }

                // Weitere folgen....
            }

            return !emptyFields;
        }

        public static void BuildInfoDisplay(int pid, MainWindow wnd)
        {
            // Objekt nochmal frisch aus der DB holen
            DBObject obj = DataBaseHandler.ReadPID(pid);

            wnd.lblGameName.Text = obj.GameName;

            BitmapImage bitProfilePic = new BitmapImage();
            // Kommt es beim Croppen zu einem Fehler (warum auch immer) würde GameTimeX abstürzen, wenn er das Profil lädt
            // => weil er kein Bild laden kann was es nicht gibt.
            // => vorher prüfen, ob File auch exisitert, was er hier laden möchte
            // => wenn ja, dann File laden, ansonsten wird das Default-Image verwendet
            if (System.IO.File.Exists(SysProps.picDestPath + SysProps.separator + obj.ProfilePicFileName))
            {
                bitProfilePic = new BitmapImage();
                bitProfilePic.BeginInit();
                bitProfilePic.UriSource = new Uri(SysProps.picDestPath + SysProps.separator + obj.ProfilePicFileName);
                bitProfilePic.EndInit();
            }
            else
            {
                bitProfilePic = GetDefaultProfileImage();
            }

          
            wnd.currProfileImage.Source = bitProfilePic;
            wnd.lblFirstTimePlayed.Text = FormatDatePlayed(obj.FirstPlay);
            wnd.lblLastTimePlayed.Text = FormatDatePlayed(obj.LastPlay);

            // Buttons enablen
            wnd.btnStartStopMonitoring.IsEnabled = true;
            wnd.btnEditProfileName.IsEnabled = true;
            wnd.lblChangeProfileImage.IsEnabled = true;

            // ToolTip setzen
            wnd.lblToolTipGameTimeText.Text = obj.GameTime.ToString("n0") + " minutes";

            // Formatieren des Spielzeittextes 
            double hours = MonitorHandler.CalcGameTime(obj.GameTime);

            string gameTimeText = "";

            if(hours == 0.0)
            {
                gameTimeText = "N/A";
            }
            else if(hours >= 1)
            {
                gameTimeText = string.Format("{0:F1}", hours) + "h";
            }
            else
            {
                gameTimeText = "< 1h";
            }

            wnd.lblGameTime.Text = gameTimeText;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) child = GetVisualChild<T>(v);
                if (child != null) break;
            }
            return child;
        }

        public static BitmapImage GetDefaultProfileImage()
        {
            BitmapImage bitProfilePic = new BitmapImage();
            bitProfilePic.BeginInit();
            bitProfilePic.UriSource = new Uri("pack://application:,,,/images/NO_PICTURE.png");
            bitProfilePic.EndInit();

            return bitProfilePic;
        }

        private static string FormatDatePlayed(DateTime date)
        {
            if(date == DateTime.MinValue)
            {
                return "N/A";
            }
            else
            {
                return date.ToString();
            }
        }

        public static void SwitchToFirstGameInList(MainWindow wnd)
        {
            if(wnd.dgProfiles.Items.Count != 0)
                wnd.dgProfiles.SelectedIndex = 0;
        }

        public static void BuildInfoDisplayNoGame(MainWindow wnd)
        {
            // Buttons Disablen
            wnd.btnEditProfileName.IsEnabled = false;
            wnd.btnStartStopMonitoring.IsEnabled = false;
            wnd.lblChangeProfileImage.IsEnabled = false;
        }
    }
}
