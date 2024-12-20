﻿using System;
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
        
        public bool markEmptyFieldsAndShowMessage(params Control[] controls)
        {

            bool emptyFields = false;

            foreach (var control in controls)
            {
                // Übergebenes Control ist Textbox
                if(control is TextBox)
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

            return emptyFields;
        }

        public static bool checkDisplay(bool showMessageAfterCheck, params Control[] controls)
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

        public static void buildInfoDisplay(int pid, MainWindow wnd)
        {
            // Objekt nochmal frisch aus der DB holen
            DBObject obj = DataBaseHandler.readPID(pid);

            wnd.lblGameName.Text = obj.GameName;

            BitmapImage bitProfilePic = new BitmapImage();
            bitProfilePic.BeginInit();
            bitProfilePic.UriSource = new Uri(SysProps.picDestPath + SysProps.separator + obj.ProfilePicFileName);
            bitProfilePic.EndInit();
            wnd.currProfileImage.Source = bitProfilePic;
            wnd.lblFirstTimePlayed.Text = formatDatePlayed(obj.FirstPlay);
            wnd.lblLastTimePlayed.Text = formatDatePlayed(obj.LastPlay);

            // Buttons enablen
            wnd.btnStartStopMonitoring.IsEnabled = true;
            wnd.btnEditProfileName.IsEnabled = true;
            wnd.lblChangeProfileImage.IsEnabled = true;

            // ToolTip setzen
            wnd.lblToolTipGameTimeText.Text = obj.GameTime.ToString("n0") + " minutes";

            // Formatieren des Spielzeittextes 
            double hours = MonitorHandler.calcGameTime(obj.GameTime);

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

        public static BitmapImage getDefaultProfileImage()
        {
            BitmapImage bitProfilePic = new BitmapImage();
            bitProfilePic.BeginInit();
            bitProfilePic.UriSource = new Uri("pack://application:,,,/images/NO_PICTURE.png");
            bitProfilePic.EndInit();

            return bitProfilePic;
        }

        private static string formatDatePlayed(DateTime date)
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

        public static void switchToFirstGameInList(MainWindow wnd)
        {
            if(wnd.dgProfiles.Items.Count != 0)
                wnd.dgProfiles.SelectedIndex = 0;
        }

        public static void buildInfoDisplayNoGame(MainWindow wnd)
        {
            // Buttons Disablen
            wnd.btnEditProfileName.IsEnabled = false;
            wnd.btnStartStopMonitoring.IsEnabled = false;
            wnd.lblChangeProfileImage.IsEnabled = false;
        }
    }
}
