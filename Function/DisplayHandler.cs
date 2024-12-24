using GameTimeX.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

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
            if(pid == 0)
            {
                BuildInfoDisplayNoGame(wnd);
                return;
            }

            // Objekt nochmal frisch aus der DB holen
            DBObject obj = DataBaseHandler.ReadPID(pid);

            wnd.lblGameName.Text = obj.GameName;

            // Tooltip für Game Namen befüllen
            wnd.lblToolTipGameName.Text = obj.GameName;

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

        public static void SwitchToFirstGameInList(MainWindow wnd, StartUpParms.ViewModes viewMode)
        {
            // Liste
            if(viewMode == StartUpParms.ViewModes.LIST)
            {
                if(wnd.dgProfiles.Items.Count != 0)
                    wnd.dgProfiles.SelectedIndex = 0;
            }
            // Kacheln
            else
            {
                if (wnd.grdGameProfiles.Children.Count != 0)
                {
                    StackPanel stackpanel = (StackPanel)wnd.grdGameProfiles.Children[0];
                    Border border = (Border)stackpanel.Children[0];
                    GTXImage image = (GTXImage)border.Child;

                    if (image != null)
                    {
                        image.Selected = true;
                        SysProps.currentSelectedPID = image.PID;
                        AnimateBorderWidth((Border)stackpanel.Children[2], image.Width, true);
                        image.DoBorderEffect = false;
                    }
                }
            }

            BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
        }

        public static void BuildInfoDisplayNoGame(MainWindow wnd)
        {
            // Buttons Disablen
            wnd.btnEditProfileName.IsEnabled = false;
            wnd.btnStartStopMonitoring.IsEnabled = false;
            wnd.lblChangeProfileImage.IsEnabled = false;
        }

        public static void BuildGameProfileGrid(MainWindow wnd)
        {
            // Alle Spielprofile holen
            List<DBObject> gameProfiles;

            wnd.grdGameProfiles.RowDefinitions.Clear();
            wnd.grdGameProfiles.ColumnDefinitions.Clear();
            wnd.grdGameProfiles.Children.Clear();

            wnd.grdGameProfiles.Margin = new Thickness(-10, 0, 0, 0);

            if (wnd.txtSearchBar.Text.Length > 0)
            {
                gameProfiles = DataBaseHandler.ReadGameName(wnd.txtSearchBar.Text);
            }
            else
            {
                gameProfiles = DataBaseHandler.ReadAll();
            }

            (int, int) colAndRows = CalculateRowsAndColumnsGameProfileGrid(gameProfiles);
            int rows = colAndRows.Item1;
            int cols = colAndRows.Item2;

            // Row-Definiton aufbauen
            BuildGameProfileGridRowDefinitons(wnd.grdGameProfiles, rows);

            // Column-Definition aufbauen
            BuildGameProfileGridColumnDefinitions(wnd.grdGameProfiles, cols);

            // Grid befüllen mit Profilen
            FillGameProfilesGrid(wnd.grdGameProfiles, gameProfiles, wnd);

            SwitchToFirstGameInList(wnd, SysProps.startUpParms.ViewMode);

            BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
            
        }

        private static void BuildDGProfiles(MainWindow wnd)
        {

            List<DBObject> profiles = null;

            if (wnd.txtSearchBar.Text == "")
            {
                profiles = DataBaseHandler.ReadAll();
            }
            else
            {
                profiles = DataBaseHandler.ReadGameName(wnd.txtSearchBar.Text);
            }

            wnd.dgProfiles.Items.Clear();

            foreach (DBObject dbprofile in profiles)
            {
                Profile profile = new Profile();
                profile.ProfileName = dbprofile.GameName;
                profile.GameTime = dbprofile.GameTime;
                profile.PID = dbprofile.ProfileID;

                wnd.dgProfiles.Items.Add(profile);
            }

            DisplayHandler.SwitchToFirstGameInList(wnd, StartUpParms.ViewModes.LIST);

            if (profiles.Count == 0)
            {
                DisplayHandler.BuildInfoDisplayNoGame(wnd);
            }
        }

        public static void BuildGameProfileView(MainWindow wnd)
        {
            // View Mode unterscheiden und je nachdem auswählen
            if (SysProps.startUpParms.ViewMode == StartUpParms.ViewModes.LIST)
            {
                wnd.grdGameProfiles.Visibility = Visibility.Collapsed;
                wnd.grdGameProfiles.IsHitTestVisible = false;
                wnd.dgProfiles.IsHitTestVisible = true;
                wnd.scrollBarTiles.Visibility = Visibility.Collapsed;
                wnd.dgProfiles.Visibility = Visibility.Visible;
                wnd.scrollBar.Visibility = Visibility.Visible;

                // DataGrid aufbauen (Gameprofile laden)
                BuildDGProfiles(wnd);
            }
            else
            {
                wnd.grdGameProfiles.Visibility = Visibility.Visible;
                wnd.dgProfiles.Visibility = Visibility.Collapsed;
                wnd.scrollBar.Visibility = Visibility.Collapsed;
                wnd.scrollBarTiles.Visibility = Visibility.Visible;
                wnd.dgProfiles.IsHitTestVisible = false;
                wnd.grdGameProfiles.IsHitTestVisible = true;

                BuildGameProfileGrid(wnd);
            }
        }



        private static (int, int) CalculateRowsAndColumnsGameProfileGrid(List<DBObject> gameProfiles)
        {
            // Maximale Spalten pro Zeile
            int maxColumnsPerRow = 4;
            int rows = 4;

            // Anzahl der Spielprofile
            int gameProfilesCount = gameProfiles.Count;

            // Wenn wir ein 4x4 Grid betrachten, müssen mind. 16 Spalten insg. erzeugt werden (4 Rows + 4 Columns)
            // Ansonsten passen die Proportienen nicht!
            // Sind es also mehrere, dann müssen wir die Rows ausrechnen!
            if(gameProfilesCount > 16)
            {
                if(gameProfilesCount % maxColumnsPerRow != 0)
                {
                    rows = (gameProfilesCount / maxColumnsPerRow) + 1;
                }
                else
                {
                    rows = gameProfilesCount / maxColumnsPerRow;
                }
            }

            return (rows, maxColumnsPerRow);
        }

        private static void BuildGameProfileGridRowDefinitons(Grid grid, int rows)
        {
            // Spielprofile durchgehen und Row-Definitons erstellen
            for(int i = 0; i < rows; i++) 
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDefinition);
            }
        }

        private static void BuildGameProfileGridColumnDefinitions(Grid grid, int columns)
        {
            // Spielprofile durchgehen und Column-Definitons erstellen
            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private static void FillGameProfilesGrid(Grid grid, List<DBObject> gameProfiles, MainWindow wnd)
        {

            // Zeilen von Grid erhalten
            int rows = grid.RowDefinitions.Count;
            int columns = grid.ColumnDefinitions.Count;
            int gameProfilesIndex = 0;
            
            // Zeilenweise durchgehen
            for(int i = 0;i < rows;i++)
            {
                // Spaltenweise durchgehen
                for(int j = 0;j < columns; j++)
                {
                    if(gameProfiles.Count - 1 >= gameProfilesIndex)
                    {
                        DBObject obj = gameProfiles[gameProfilesIndex];

                        StackPanel stackPanel = new StackPanel();
                        Border imageBorder = new Border();
                        imageBorder.Effect = VisualHandler.GetDropShadowEffect();

                        if (j == 0)
                        {
                            stackPanel.Margin = new Thickness(0, 10, 0, 0);
                        }
                        else
                        {
                            stackPanel.Margin = new Thickness(10, 10, 10, 10);
                        }

                        stackPanel.Orientation = Orientation.Vertical;

                        Border bottomBorder = new Border
                        {
                            Height = 4,
                            Background = SysProps.defButtonColor,
                            Width = 0, // Anfangsbreite der Border
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Margin = new Thickness(0),
                            CornerRadius = new CornerRadius(2)
                        };

                        GTXImage profileImage = new GTXImage();
                        profileImage.PID = obj.ProfileID;
                        profileImage.MainWnd = wnd;

                        BitmapImage bitProfilePic = new BitmapImage();
                        bitProfilePic.BeginInit();
                        bitProfilePic.UriSource = new Uri(SysProps.picDestPath + SysProps.separator + obj.ProfilePicFileName);
                        bitProfilePic.EndInit();

                        profileImage.Source = bitProfilePic;
                        profileImage.Width = GetWidthForImage(grid);
                        profileImage.Height = GetWidthForImage(grid);
                        profileImage.Margin = new Thickness(0);

                        // Cornor-Radius setzen
                        profileImage.Clip = new RectangleGeometry
                        {
                            RadiusX = 5,
                            RadiusY = 5,
                            Rect = new Rect(0, 0, profileImage.Width, profileImage.Height)
                        };

                        profileImage.Effect = VisualHandler.GetDropShadowEffect();

                        TextBlock profileTitle = new TextBlock();
                        profileTitle.Text = obj.GameName;
                        profileTitle.MaxWidth = profileImage.Width;
                        profileTitle.FontSize = 16;
                        profileTitle.Margin = new Thickness(0, 5, 0, 0);
                        profileTitle.Foreground = System.Windows.Media.Brushes.White;
                        profileTitle.TextTrimming = TextTrimming.CharacterEllipsis;
                        profileTitle.HorizontalAlignment = HorizontalAlignment.Center;

                        stackPanel.MouseEnter += ProfileImage_MouseEnter;
                        stackPanel.MouseLeave += ProfileImage_MouseLeave;
                        stackPanel.MouseDown += ProfileImage_MouseDown;

                        imageBorder.Child = profileImage;

                        stackPanel.Children.Add(imageBorder);
                        stackPanel.Children.Add(profileTitle);
                        stackPanel.Children.Add(bottomBorder);

                        Grid.SetRow(stackPanel, i);
                        Grid.SetColumn(stackPanel, j);

                        grid.Children.Add(stackPanel);

                        gameProfilesIndex++;
                    }
                }
            }
        }

        public static void DeselectNonCurrentProfiles(int currentPID)
        {
            foreach(StackPanel stackPanel in SysProps.mainWindow.grdGameProfiles.Children)
            {
                Border border = (Border)stackPanel.Children[0];
                GTXImage img = (GTXImage)border.Child;
                // Bilder durchsuchen
                if(img.PID != currentPID && img.Selected)
                {
                    img.Selected = false;
                    img.DoBorderEffect = true;
                    AnimateBorderWidth((Border)stackPanel.Children[2], img.ActualWidth, false);
                }
            }
        }

        private static void ProfileImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;

            Border border = (Border)stackPanel.Children[0];
            GTXImage image = (GTXImage)border.Child;

            if(image != null)
            {
                if (image.PID != SysProps.currentSelectedPID && MonitorHandler.CurrentlyMonitoringGameTime())
                    MonitorHandler.EndMonitoringGameTime(image.MainWnd);

                image.DoBorderEffect = false;
                image.Selected = true;
                SysProps.currentSelectedPID = image.PID;
                
                DeselectNonCurrentProfiles(image.PID);
                BuildInfoDisplay(image.PID, image.MainWnd);
            }
        }

        private static void ProfileImage_MouseLeave(object sender, MouseEventArgs e)
        {

            StackPanel stackPanel = (StackPanel)sender;
            Border border = (Border)stackPanel.Children[0];
            GTXImage image = (GTXImage)border.Child;

            if (image != null)
            {
                if(image.DoBorderEffect)
                    AnimateBorderWidth((Border)stackPanel.Children[2], image.ActualWidth, false);

                image.MainWnd.Cursor = Cursors.Arrow;
            }
        }

        private static void ProfileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            Border border = (Border)stackPanel.Children[0];
            GTXImage image = (GTXImage)border.Child;

            if (image != null)
            {
                if (image.DoBorderEffect)
                {
                    AnimateBorderWidth((Border)stackPanel.Children[2], image.ActualWidth, true);
                    image.MainWnd.Cursor = Cursors.Hand;
                } 
            }  
        }

        private static double GetWidthForImage(Grid grid)
        {
            double gridWidth = grid.ActualWidth;
            double margin = 10;

            double marginWidth = (margin * 2) * 4;

            return (gridWidth - marginWidth) / 4;
        }

        private static void AnimateBorderWidth(Border border, double targetWidth, bool isMouseEnter)
        {
            // Erstelle die Animation
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = isMouseEnter ? 0 : targetWidth, // Von 0 bei MouseEnter oder zum Zielwert bei MouseLeave
                To = isMouseEnter ? targetWidth : 0,  // Zielwert der Animation (die Breite des Bildes)
                Duration = new Duration(TimeSpan.FromSeconds(0.5)), // Dauer der Animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // Sanfte Übergänge
            };

            // Startet die Animation
            border.BeginAnimation(Border.WidthProperty, widthAnimation);
        }
    }
}
