using GameTimeX.Objects;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

            // Playthrough Game Time
            
            // Wenn Startpunkt des neuen Playthrough = 0 => ausblenden
            if(obj.PlayThroughStartingPoint == 0)
            {
                wnd.rowPlaythrough.Height = new GridLength(0);
            }
            else
            {
                wnd.rowPlaythrough.Height = new GridLength(15, GridUnitType.Star);
                // Ansonsten mit Daten befüllen
                int actPlaythroughTime = (int)obj.GameTime - obj.PlayThroughStartingPoint;


                // ToolTip setzen
                wnd.lblToolTipGameTimeTextNewPlaythrough.Text = actPlaythroughTime.ToString("n0") + " minutes";

                // Formatieren des Spielzeittextes 
                hours = MonitorHandler.CalcGameTime(actPlaythroughTime);

                gameTimeText = "";

                if (hours == 0.0)
                {
                    gameTimeText = "N/A";
                }
                else if (hours >= 1)
                {
                    gameTimeText = string.Format("{0:F1}", hours) + "h";
                }
                else
                {
                    gameTimeText = "< 1h";
                }

                wnd.lblToolTipGameTimeTextNewPlaythrough.Text = gameTimeText;
            }
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
                    TextBlock textBlock = (TextBlock)stackpanel.Children[1];

                    if (image != null)
                    {
                        image.Selected = true;
                        SysProps.currentSelectedPID = image.PID;
                        AnimateBorderWidth((Border)stackpanel.Children[2], image.Width, true);
                        image.DoBorderEffect = false;
                        textBlock.FontWeight = FontWeights.Bold;
                    }
                }
                else
                {
                    SysProps.currentSelectedPID = 0;
                }
            }

            BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
        }

        public static void SwitchToSpecificGame(MainWindow wnd, StartUpParms.ViewModes viewMode, int pid)
        {
            // Liste
            if (viewMode == StartUpParms.ViewModes.LIST)
            {
                if (wnd.dgProfiles.Items.Count != 0)
                    SelectRowByValue(wnd.dgProfiles, pid);
            }
            // Kacheln
            else
            {
                if (wnd.grdGameProfiles.Children.Count != 0)
                {
                    SelectTileByValue(wnd.grdGameProfiles, wnd.scrollBarTiles, pid);
                }
                else
                {
                    SysProps.currentSelectedPID = 0;
                }
            }

            BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
        }

        public static void SelectRowByValue(DataGrid dataGrid, int valueToFind)
        {
            foreach (var item in dataGrid.Items)
            {
                var profile = item as Profile;
                if (profile != null && profile.PID == valueToFind)
                {
                    dataGrid.SelectedItem = item;
                    SysProps.currentSelectedPID = profile.PID;

                    int rowIndex = GetRowIndexForItem(dataGrid, profile);
                    ScrollToDataGridRow(dataGrid, rowIndex);
                    break;
                }
            }
        }

        private static DataGridRow GetDataGridRowForItem(DataGrid dataGrid, object item)
        {
            var container = dataGrid.ItemContainerGenerator.ContainerFromItem(item);
            return container as DataGridRow;
        }

        public static void SelectTileByValue(Grid grid, ScrollViewer scrollViewer, int valueToFind)
        {
            foreach (StackPanel stackPanel in SysProps.mainWindow.grdGameProfiles.Children)
            {
                Border border = (Border)stackPanel.Children[0];
                GTXImage img = (GTXImage)border.Child;
                TextBlock txtBlock = (TextBlock)stackPanel.Children[1];

                int rowIndex = Grid.GetRow(stackPanel);
              
                // Tiles durchsuchen
                if(img.PID == valueToFind)
                {
                    DeselectNonCurrentProfiles(img.PID);
                    img.Selected = true;
                    img.DoBorderEffect = false;

                    // Zu Zeile scrollen
                    ScrollToProileTilesGridRow(grid, scrollViewer, rowIndex);

                    txtBlock.FontWeight = FontWeights.Bold;
                    AnimateBorderWidth((Border)stackPanel.Children[2], img.ActualWidth, true);
                    SysProps.currentSelectedPID = img.PID;
                }
            }
        }

        public static void ScrollToProileTilesGridRow(Grid grid, ScrollViewer scrollViewer, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < grid.RowDefinitions.Count)
            {
                // Berechne die Höhe bis zur angegebenen Zeile
                double offset = 0;
                for (int i = 0; i <= rowIndex; i++)
                {
                    offset += grid.RowDefinitions[i].ActualHeight;
                }

                // Scrollen bis zur berechneten Höhe
                scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private static int GetRowIndexForItem(DataGrid grid, Profile item)
        {
            var collection = grid.ItemsSource as IList<Profile>;
            if (collection != null)
            {
                return collection.IndexOf(item);
            }
            return -1; // Rückgabewert, falls das Element nicht gefunden wird
        }

        public static void ScrollToDataGridRow(DataGrid myDataGrid, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < myDataGrid.Items.Count)
            {
                var row = myDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as System.Windows.Controls.DataGridRow;
                if (row != null)
                {
                    // Scrollt die Zeile in den sichtbaren Bereich
                    row.BringIntoView();
                }
            }
        }

        public static void BuildInfoDisplayNoGame(MainWindow wnd)
        {
            // Buttons Disablen
            wnd.btnEditProfileName.IsEnabled = false;
            wnd.btnStartStopMonitoring.IsEnabled = false;
            wnd.lblChangeProfileImage.IsEnabled = false;
            wnd.currProfileImage.Source = DisplayHandler.GetDefaultProfileImage();
        }

        public static void BuildGameProfileGrid(MainWindow wnd)
        {
            // Alle Spielprofile holen
            List<DBObject> gameProfiles;

            wnd.grdGameProfiles.RowDefinitions.Clear();
            wnd.grdGameProfiles.ColumnDefinitions.Clear();
            wnd.grdGameProfiles.Children.Clear();

            //wnd.grdGameProfiles.Margin = new Thickness(-10, 0, 0, 0);

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

                DisplayHandler.AttachContextMenuToDataGrid(wnd.dgProfiles);
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
                        profileImage.Width = GetWidthForImage(grid, wnd);
                        profileImage.Height = GetWidthForImage(grid, wnd);
                        profileImage.Margin = new Thickness(0);

                        // Cornor-Radius setzen
                        profileImage.Clip = new RectangleGeometry
                        {
                            RadiusX = 5,
                            RadiusY = 5,
                            Rect = new Rect(0, 0, profileImage.Width, profileImage.Height)
                        };

                        profileImage.Effect = VisualHandler.GetDropShadowEffect();

                        // Kontextmenü zu Image hinzufügen
                        AttachContextMenuToImage(profileImage);

                        TextBlock profileTitle = new TextBlock();
                        profileTitle.Text = obj.GameName;
                        profileTitle.MaxWidth = profileImage.Width;
                        profileTitle.FontSize = 16;
                        profileTitle.Margin = new Thickness(0, 5, 0, 0);
                        profileTitle.Foreground = System.Windows.Media.Brushes.White;
                        profileTitle.TextTrimming = TextTrimming.CharacterEllipsis;
                        profileTitle.HorizontalAlignment = HorizontalAlignment.Center;
                        profileTitle.IsHitTestVisible = false;

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
                TextBlock txtBlock = (TextBlock)stackPanel.Children[1];
                // Bilder durchsuchen
                if(img.PID != currentPID && img.Selected)
                {
                    img.Selected = false;
                    img.DoBorderEffect = true;
                    txtBlock.FontWeight = FontWeights.Normal;
                    AnimateBorderWidth((Border)stackPanel.Children[2], img.ActualWidth, false);
                }
            }
        }

        private static void AttachContextMenuToImage(GTXImage image)
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.FontSize = 17;

            System.Windows.Controls.Image imgProperties = new System.Windows.Controls.Image();
            imgProperties.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/properties.png");

            System.Windows.Controls.Image imgDelete = new System.Windows.Controls.Image();
            imgDelete.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/delete.png");

            System.Windows.Controls.Image imgPlaythrough = new System.Windows.Controls.Image();
            imgPlaythrough.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/game_time.png");

            // Kontextmenü Einträge

            // Löschen Eintrag
            GTXMenuItem mIDelete = new GTXMenuItem();
            mIDelete.Header = "Delete";
            mIDelete.Icon = imgDelete;
            mIDelete.PID = image.PID;
            mIDelete.Click += MIDelete_Clicked;

            // Eigenschaften Eintrag
            GTXMenuItem mIProperties = new GTXMenuItem();
            mIProperties.Header = "Properties";
            mIProperties.Icon = imgProperties;
            mIProperties.PID = image.PID;
            mIProperties.Click += MIProperties_Clicked;

            // Neuer Playthrough Eintrag
            GTXMenuItem mIPlaythrough = new GTXMenuItem();
            mIPlaythrough.Header = "New playthrough startpoint";
            mIPlaythrough.Icon = imgPlaythrough;
            mIPlaythrough.PID = image.PID;
            mIPlaythrough.Click += MIPlaythrough_Clicked;

            // Zu Kontextmenü hinzufügen
            contextMenu.Items.Add(mIDelete);

            // Kontextfunktion nur hinzufügen, wenn schon Zeit aufgenommen wurde
            if(DataBaseHandler.IsPlayTimeGreaterZero(image.PID))
                contextMenu.Items.Add(mIPlaythrough);

            contextMenu.Items.Add(mIProperties);

            contextMenu.Style = VisualHandler.GetApplicationResource("contextMenuStyle") as Style;

            image.ContextMenu = contextMenu;
        }

        

        public static void AttachContextMenuToDataGrid(DataGrid dataGrid)
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.FontSize = 17;

            System.Windows.Controls.Image imgProperties = new System.Windows.Controls.Image();

            imgProperties.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/properties.png");

            System.Windows.Controls.Image imgDelete = new System.Windows.Controls.Image();
            imgDelete.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/delete.png");

            System.Windows.Controls.Image imgPlaythrough = new System.Windows.Controls.Image();
            imgPlaythrough.Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/game_time.png");

            // Kontextmenü Einträge

            // Löschen Eintrag
            GTXMenuItem mIDelete = new GTXMenuItem();
            mIDelete.Header = "Delete";
            mIDelete.Icon = imgDelete;
            mIDelete.Click += MIDelete_DataGrid_Clicked;

            // Eigenschaften Eintrag
            GTXMenuItem mIProperties = new GTXMenuItem();
            mIProperties.Header = "Properties";
            mIProperties.Icon = imgProperties;
            mIProperties.Click += MIProperties_DataGrid_Clicked;

            // Neuer Playthrough Eintrag
            GTXMenuItem mIPlaythrough = new GTXMenuItem();
            mIPlaythrough.Header = "New playthrough startpoint";
            mIPlaythrough.Icon = imgPlaythrough;
            mIPlaythrough.Click += MIPlaythrough_DataGrid_Clicked;

            // Zu Kontextmenü hinzufügen
            contextMenu.Items.Add(mIDelete);
            contextMenu.Items.Add(mIPlaythrough);
            contextMenu.Items.Add(mIProperties);

            contextMenu.Style = VisualHandler.GetApplicationResource("contextMenuStyle") as Style;

            dataGrid.ContextMenu = contextMenu;
        }

        private static void MIPlaythrough_DataGrid_Clicked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            var profile = (Profile)item.SelectedCells[0].Item;

            DBObject obj = DataBaseHandler.ReadPID(profile.PID);
            if (obj != null)
            {
                obj.PlayThroughStartingPoint = (int)obj.GameTime;
                DataBaseHandler.Save(obj);
                BuildInfoDisplay(profile.PID, SysProps.mainWindow);
            }
        }

        private static void MIPlaythrough_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            DBObject obj = DataBaseHandler.ReadPID(menuItem.PID);
            if (obj != null)
            {
                obj.PlayThroughStartingPoint = (int) obj.GameTime;
                DataBaseHandler.Save(obj);
                BuildInfoDisplay(menuItem.PID, SysProps.mainWindow);
            }
        }

        private static void MIProperties_DataGrid_Clicked(object sender, RoutedEventArgs e)
        {

            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            var profile = (Profile)item.SelectedCells[0].Item;

            Properties properties = new Properties(profile.PID);
            properties.Owner = SysProps.mainWindow;
            properties.ShowDialog();

            DisplayHandler.BuildGameProfileView(SysProps.mainWindow);
        }

        private static void MIDelete_DataGrid_Clicked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            var profile = (Profile)item.SelectedCells[0].Item;

            DBObject dbObj = DataBaseHandler.ReadPID(profile.PID);

            if (dbObj != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to delete '" + dbObj.GameName + "'?", "Delete", "Cancel");
                quest.Owner = SysProps.mainWindow;
                quest.ShowDialog();

                // User hat "Delete" geklickt
                if (quest.UsrReturnType == QuestionBox.ReturnType.YES)
                {
                    if (dbObj != null)
                    {
                        DataBaseHandler.Delete(dbObj.ProfileID);
                        DisplayHandler.BuildGameProfileView(SysProps.mainWindow);

                        if (SysProps.startUpParms.AutoProfileSwitching && SysProps.gameSwitcherHandler != null)
                            SysProps.gameSwitcherHandler.RemoveProfileAndExecutables(dbObj.ProfileID);
                    }
                }
            }
        }

        private static void MIProperties_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            Properties properties = new Properties(menuItem.PID);
            properties.Owner = SysProps.mainWindow;
            properties.ShowDialog();

            DisplayHandler.BuildGameProfileView(SysProps.mainWindow);
        }

        private static void MIDelete_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            DBObject dbObj = DataBaseHandler.ReadPID(menuItem.PID);

            if (dbObj != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to delete '" + dbObj.GameName + "'?", "Delete", "Cancel");
                quest.Owner = SysProps.mainWindow;
                quest.ShowDialog();

                // User hat "Delete" geklickt
                if (quest.UsrReturnType == QuestionBox.ReturnType.YES)
                {
                    if (dbObj != null)
                    {
                        DataBaseHandler.Delete(dbObj.ProfileID);
                        DisplayHandler.BuildGameProfileView(SysProps.mainWindow);

                        if (SysProps.startUpParms.AutoProfileSwitching && SysProps.gameSwitcherHandler != null)
                            SysProps.gameSwitcherHandler.RemoveProfileAndExecutables(dbObj.ProfileID);
                    }
                }
            }
        }

        private static void ProfileImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

            StackPanel stackPanel = (StackPanel)sender;

            Border border = (Border)stackPanel.Children[0];
            TextBlock txtBlock = (TextBlock)stackPanel.Children[1];
            GTXImage image = (GTXImage)border.Child;

            if (e.ChangedButton == MouseButton.Left)
            {


                if (image != null)
                {
                    if (image.PID != SysProps.currentSelectedPID && MonitorHandler.CurrentlyMonitoringGameTime())
                        MonitorHandler.EndMonitoringGameTime(image.MainWnd);

                    image.DoBorderEffect = false;
                    image.Selected = true;
                    txtBlock.FontWeight = FontWeights.Bold;
                    SysProps.currentSelectedPID = image.PID;

                    DeselectNonCurrentProfiles(image.PID);
                    BuildInfoDisplay(image.PID, image.MainWnd);
                }
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

        private static double GetWidthForImage(Grid grid, MainWindow wnd)
        {
            double gridWidth = grid.ActualWidth;
            double margin = 10;
            double scrollBarWidth = 20;

            double marginWidth = (margin * 2) * 4 + scrollBarWidth + 2;

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
