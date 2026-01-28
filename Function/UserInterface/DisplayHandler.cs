using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Function.DataBase.Object;
using GameTimeX.Function.Monitoring;
using GameTimeX.Function.Utils;
using GameTimeX.Objects;
using GameTimeX.Objects.Components;
using GameTimeX.XApplication.SubDisplays;
using Color = System.Windows.Media.Color;

namespace GameTimeX.Function.UserInterface
{
    internal class DisplayHandler
    {
        private const double HoverScale = 1.03;
        private const double HoverLiftY = -2.0;
        private const double PressScaleDelta = -0.05;
        private const double PressLiftDelta = 1.0;
        private const double HoverDotWidth = 10.0;

        private static readonly TimeSpan IntroDuration = TimeSpan.FromMilliseconds(260);
        private static readonly TimeSpan HoverDuration = TimeSpan.FromMilliseconds(130);
        private static readonly TimeSpan LeaveDuration = TimeSpan.FromMilliseconds(150);
        private static readonly TimeSpan PressDuration = TimeSpan.FromMilliseconds(70);
        private static readonly TimeSpan HoverDotDuration = TimeSpan.FromMilliseconds(140);

        private static readonly Color DefaultShadowColor = Colors.Black;
        private static readonly Color SelectedShadowColor = Color.FromRgb(40, 154, 252);

        private const int IntroAnimationLimit = 16;
        private const int TileBuildBatchSize = 40;
        private const int LazyUpdateThrottleMs = 60;
        private const int InitialImageLoadCount = 12;

        private static DispatcherTimer _lazyUpdateTimer;
        private static bool _lazyUpdatePending;

        private static readonly Dictionary<string, BitmapImage> _thumbCache = new();
        private static readonly BitmapImage playableImage = new BitmapImage(new Uri("pack://application:,,,/images/playable.png"));

        private static int _pendingSwitchPid = 0;
        private static bool _isBuildingTiles = false;

        private sealed class LazyInfo
        {
            public string Path;
            public int DecodeSize;
            public bool Loaded;
        }

        private sealed class TileImageTag
        {
            public LazyInfo Info;
            public FrameworkElement Loader;
        }

        private static bool TryApplyPendingSwitch(MainWindow wnd)
        {
            if (wnd == null) return false;
            if (_pendingSwitchPid == 0) return false;
            if (wnd.grdGameProfiles.Children.Count == 0) return false;

            bool applied = SelectTileByValueReturnApplied(wnd.grdGameProfiles, wnd.scrollBarTiles, _pendingSwitchPid);
            if (applied)
            {
                _pendingSwitchPid = 0;
                BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
                return true;
            }

            return false;
        }

        private static void EnsureSelectionAfterBuild(MainWindow wnd)
        {
            if (wnd == null) return;
            if (wnd.grdGameProfiles.Children.Count == 0) return;

            // 1) Pending PID hat Vorrang
            if (_pendingSwitchPid != 0)
            {
                if (TryApplyPendingSwitch(wnd))
                    return;
            }

            // 2) Bestehende Selection erneut auf neue Tiles anwenden
            if (SysProps.currentSelectedPID != 0)
            {
                bool applied = SelectTileByValueReturnApplied(
                    wnd.grdGameProfiles,
                    wnd.scrollBarTiles,
                    SysProps.currentSelectedPID
                );

                if (applied)
                    return;

                // Wenn der PID nicht mehr existiert, reset
                SysProps.currentSelectedPID = 0;
            }

            // 3) Fallback
            SwitchToFirstGameInList(wnd, SysProps.startUpParms.ViewMode);
        }



        private static bool SelectTileByValueReturnApplied(Grid grid, ScrollViewer scrollViewer, int valueToFind)
        {
            foreach (StackPanel stackPanel in SysProps.mainWindow.grdGameProfiles.Children)
            {
                var img = GetTileImage(stackPanel);
                var txtBlock = GetTileTitle(stackPanel);
                var underline = GetTileUnderline(stackPanel);

                if (img != null && img.PID == valueToFind)
                {
                    DeselectNonCurrentProfiles(img.PID);

                    img.Selected = true;
                    img.DoBorderEffect = false;

                    if (txtBlock != null) txtBlock.FontWeight = FontWeights.Bold;

                    if (underline != null)
                    {
                        underline.BeginAnimation(FrameworkElement.WidthProperty, null);
                        underline.Width = 0;
                        underline.HorizontalAlignment = HorizontalAlignment.Left;
                        double w = SafeUnderlineWidth(img);
                        AnimateBorderWidth(underline, w, true);
                    }

                    SysProps.currentSelectedPID = img.PID;

                    UpdateSelectionGlow(stackPanel, true);

                    EnsureTileVisible(stackPanel, scrollViewer);

                    return true;
                }
            }

            return false;
        }

        private static void ValidateCurrentSelection(List<DBO_Profile> gameProfiles)
        {
            if (gameProfiles == null || gameProfiles.Count == 0)
            {
                SysProps.currentSelectedPID = 0;
                _pendingSwitchPid = 0;
                return;
            }

            int cur = SysProps.currentSelectedPID;
            if (cur == 0) return;

            bool exists = false;
            foreach (var p in gameProfiles)
            {
                if (p.ProfileID == cur)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                SysProps.currentSelectedPID = 0;
                _pendingSwitchPid = 0;
            }
        }





        public static bool CheckDisplay(bool showMessageAfterCheck, params Control[] controls)
        {
            bool emptyFields = false;

            foreach (var control in controls)
            {
                if (control is TextBox txtBox)
                {
                    if (string.IsNullOrEmpty(txtBox.Text))
                    {
                        txtBox.BorderBrush = SysProps.emptyFieldsColor;
                        emptyFields = true;
                    }
                }
            }

            return !emptyFields;
        }

        private static void FadeInImage(Image img, int durationMs = 140)
        {
            if (img == null) return;

            img.BeginAnimation(UIElement.OpacityProperty, null);
            img.Opacity = 0;

            var anim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            img.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        public static void BuildInfoDisplay(int pid, MainWindow wnd)
        {
            if (pid == 0)
            {
                BuildInfoDisplayNoGame(wnd);
                return;
            }

            DBO_Profile dbo_profiles = DM_Profile.ReadPID(pid);

            wnd.lblGameName.Text = dbo_profiles.GameName;
            wnd.lblToolTipGameName.Text = dbo_profiles.GameName;

            BitmapImage bitProfilePic;
            if (File.Exists(SysProps.picDestPath + SysProps.separator + dbo_profiles.ProfilePicFileName))
            {
                bitProfilePic = new BitmapImage();
                bitProfilePic.BeginInit();
                bitProfilePic.UriSource = new Uri(SysProps.picDestPath + SysProps.separator + dbo_profiles.ProfilePicFileName);
                bitProfilePic.EndInit();
            }
            else
            {
                bitProfilePic = GetDefaultProfileImage();
            }

            wnd.currProfileImage.Source = bitProfilePic;
            wnd.lblFirstTimePlayed.Text = FormatDatePlayed(dbo_profiles.FirstPlay);
            wnd.lblLastTimePlayed.Text = FormatDatePlayed(dbo_profiles.LastPlay);

            wnd.btnStartStopMonitoring.IsEnabled = true;
            wnd.btnEditProfileName.IsEnabled = true;
            wnd.lblChangeProfileImage.IsEnabled = true;

            if (dbo_profiles.SteamAppID == 0)
                wnd.btnLaunchSteamGame.Visibility = Visibility.Collapsed;
            else
                wnd.btnLaunchSteamGame.Visibility = Visibility.Visible;

            string playedToday = "";
            double minutesToday = FN_Profile.GetTodaysPlayTime(dbo_profiles.ProfileID);

            if (minutesToday > 0)
            {
                playedToday = "Played today: " + minutesToday.ToString("n0") + " min (" +
                              string.Format("{0:F1}", MonitorHandler.CalcGameTime(minutesToday)) + " h)";
            }

            double playedTotal = FN_Profile.GetTotalPlayTime(dbo_profiles.ProfileID);

            string tooltipText = playedTotal.ToString("n0") + " minutes";
            if (minutesToday > 0)
                tooltipText += "\n" + playedToday;

            wnd.lblToolTipGameTimeText.Text = tooltipText;

            double hours = MonitorHandler.CalcGameTime(playedTotal);
            string gameTimeText =
                hours == 0.0 ? "N/A" :
                hours >= 1 ? string.Format("{0:F1}", hours) + " h" :
                "< 1 h";

            wnd.lblGameTime.Text = gameTimeText;

            if (dbo_profiles.PlaythroughStartPointDate == DateTime.MinValue)
            {
                wnd.rowPlaythrough.Height = new GridLength(0);
            }
            else
            {
                wnd.rowPlaythrough.Height = new GridLength(30);

                double actPlaythroughTime = FN_Profile.GetCurrentPlaythroughTime(dbo_profiles.ProfileID, dbo_profiles.PlaythroughStartPointDate);
                wnd.lblToolTipGameTimeTextNewPlaythrough.Text = actPlaythroughTime.ToString("n0") + " minutes";

                hours = MonitorHandler.CalcGameTime(actPlaythroughTime);
                string gameTimeText2 =
                    hours == 0.0 ? "N/A" :
                    hours >= 1 ? string.Format("{0:F1}", hours) + " h" :
                    "< 1 h";

                wnd.lblToolTipGameTimeTextNewPlaythrough.Text = gameTimeText2;
            }
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default;
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
            return date == DateTime.MinValue ? "N/A" : date.ToString();
        }

        public static void SwitchToFirstGameInList(MainWindow wnd, StartUpParms.ViewModes viewMode)
        {
            if (wnd == null) return;

            if (viewMode == StartUpParms.ViewModes.LIST)
            {
                if (wnd.dgProfiles.Items.Count != 0)
                    wnd.dgProfiles.SelectedIndex = 0;

                BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
                return;
            }

            // Tile-View
            if (wnd.grdGameProfiles.Children.Count == 0)
            {
                // später erneut versuchen (Build läuft ggf. noch)
                wnd.Dispatcher.BeginInvoke((Action)(() =>
                {
                    // pending PID hat Vorrang
                    if (!TryApplyPendingSwitch(wnd))
                    {
                        if (wnd.grdGameProfiles.Children.Count != 0)
                            SwitchToFirstGameInList(wnd, viewMode);
                    }
                }), DispatcherPriority.Background);

                return;
            }

            // pending PID hat Vorrang
            if (TryApplyPendingSwitch(wnd))
                return;

            StackPanel stackpanel = (StackPanel)wnd.grdGameProfiles.Children[0];
            var image = GetTileImage(stackpanel);
            var textBlock = GetTileTitle(stackpanel);
            var underline = GetTileUnderline(stackpanel);

            if (image != null)
            {
                image.Selected = true;
                SysProps.currentSelectedPID = image.PID;

                if (underline != null)
                {
                    underline.BeginAnimation(FrameworkElement.WidthProperty, null);
                    underline.Width = 0;
                    underline.HorizontalAlignment = HorizontalAlignment.Left;
                    double w = SafeUnderlineWidth(image);
                    AnimateBorderWidth(underline, w, true);
                }

                image.DoBorderEffect = false;
                if (textBlock != null) textBlock.FontWeight = FontWeights.Bold;

                UpdateSelectionGlow(stackpanel, true);
                wnd.scrollBarTiles.ScrollToTop();
            }
            else
            {
                SysProps.currentSelectedPID = 0;
            }

            BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
        }


        private static BitmapImage LoadTileBitmap(string path, int pixelSize)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            if (pixelSize > 0) bmp.DecodePixelWidth = pixelSize;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private static BitmapImage LoadTileBitmapCached(string path, int pixelSize)
        {
            string key = $"{path}|{pixelSize}";
            if (_thumbCache.TryGetValue(key, out var cached)) return cached;
            var bmp = LoadTileBitmap(path, pixelSize);
            _thumbCache[key] = bmp;
            return bmp;
        }

        public static void SwitchToSpecificGame(MainWindow wnd, StartUpParms.ViewModes viewMode, int pid)
        {
            if (wnd == null) return;

            _pendingSwitchPid = pid;

            if (viewMode == StartUpParms.ViewModes.LIST)
            {
                if (wnd.dgProfiles.Items.Count != 0)
                {
                    SelectRowByValue(wnd.dgProfiles, pid);
                    _pendingSwitchPid = 0;
                    BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
                }
                return;
            }

            // Tile-View
            if (wnd.grdGameProfiles.Children.Count != 0)
            {
                bool applied = SelectTileByValueReturnApplied(wnd.grdGameProfiles, wnd.scrollBarTiles, pid);
                if (applied)
                {
                    _pendingSwitchPid = 0;
                    BuildInfoDisplay(SysProps.currentSelectedPID, wnd);
                }
                // wenn nicht applied: pending bleibt gesetzt und wird beim Build nachgezogen
            }
            // wenn Children.Count == 0: pending bleibt gesetzt und wird beim Build nachgezogen
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
                var img = GetTileImage(stackPanel);
                var txtBlock = GetTileTitle(stackPanel);
                var underline = GetTileUnderline(stackPanel);

                int rowIndex = Grid.GetRow(stackPanel);

                if (img != null && img.PID == valueToFind)
                {
                    DeselectNonCurrentProfiles(img.PID);
                    img.Selected = true;
                    img.DoBorderEffect = false;

                    ScrollToProileTilesGridRow(grid, scrollViewer, rowIndex);

                    if (txtBlock != null) txtBlock.FontWeight = FontWeights.Bold;

                    if (underline != null)
                    {
                        underline.BeginAnimation(FrameworkElement.WidthProperty, null);
                        underline.Width = 0;
                        underline.HorizontalAlignment = HorizontalAlignment.Left;
                        double w = SafeUnderlineWidth(img);
                        AnimateBorderWidth(underline, w, true);
                    }

                    SysProps.currentSelectedPID = img.PID;

                    UpdateSelectionGlow(stackPanel, true);
                }
            }
        }

        public static void ScrollToProileTilesGridRow(Grid grid, ScrollViewer scrollViewer, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < grid.RowDefinitions.Count)
            {
                double offset = 0;
                for (int i = 0; i <= rowIndex; i++)
                    offset += grid.RowDefinitions[i].ActualHeight;

                scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private static int GetRowIndexForItem(DataGrid grid, Profile item)
        {
            var collection = grid.ItemsSource as IList<Profile>;
            return collection != null ? collection.IndexOf(item) : -1;
        }

        public static void ScrollToDataGridRow(DataGrid myDataGrid, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < myDataGrid.Items.Count)
            {
                var row = myDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                row?.BringIntoView();
            }
        }

        private static List<DBO_Profile> GetAllPlayableGames(List<DBO_Profile> gameProfiles)
        {
            var playableGames = new List<DBO_Profile>();

            foreach (DBO_Profile gameProfile in gameProfiles)
            {
                if (Directory.Exists(gameProfile.ExtGameFolder) &&
                    FuncExecutables.GetAllExecutablesFromDirectory(gameProfile.ExtGameFolder).Count > 0)
                {
                    playableGames.Add(gameProfile);
                }
            }

            return playableGames;
        }

        public static void BuildInfoDisplayNoGame(MainWindow wnd)
        {
            wnd.btnEditProfileName.IsEnabled = false;
            wnd.btnStartStopMonitoring.IsEnabled = false;
            wnd.lblChangeProfileImage.IsEnabled = false;
            wnd.currProfileImage.Source = GetDefaultProfileImage();
            wnd.lblGameName.Text = "N/A";
            wnd.lblFirstTimePlayed.Text = "N/A";
            wnd.lblLastTimePlayed.Text = "N/A";
            wnd.lblGameTime.Text = "N/A";
            wnd.lblToolTipGameTimeText.Text = "N/A";
            wnd.lblToolTipGameTimeTextNewPlaythrough.Text = "N/A";
        }

        public static void BuildGameProfileGrid(MainWindow wnd)
        {
            ShowTilesLoading(wnd, true);

            List<DBO_Profile> gameProfiles;

            wnd.grdGameProfiles.RowDefinitions.Clear();
            wnd.grdGameProfiles.ColumnDefinitions.Clear();
            wnd.grdGameProfiles.Children.Clear();

            if (wnd.txtSearchBar.Text.Length > 0)
                gameProfiles = DM_Profile.ReadGameName(wnd.txtSearchBar.Text);
            else
                gameProfiles = DM_Profile.ReadAll();

            if (wnd.btnPlayableFilter.IsChecked == true)
                gameProfiles = GetAllPlayableGames(gameProfiles);

            wnd.emptyStateOverlay.Visibility = gameProfiles.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Selektion gegen aktuelle Liste prüfen (z.B. nach Delete)
            ValidateCurrentSelection(gameProfiles);

            (int, int) colAndRows = CalculateRowsAndColumnsGameProfileGrid(gameProfiles);
            int rows = colAndRows.Item1;
            int cols = colAndRows.Item2;

            BuildGameProfileGridRowDefinitons(wnd.grdGameProfiles, rows);
            BuildGameProfileGridColumnDefinitions(wnd.grdGameProfiles, cols);

            FillGameProfilesGrid(wnd.grdGameProfiles, gameProfiles, wnd);

            HookLazyLoading(wnd);

            int count = gameProfiles.Count;
            int introCount = Math.Min(count, IntroAnimationLimit);

            double totalMs =
                IntroDuration.TotalMilliseconds +
                Math.Max(0, introCount - 1) * 80 +
                120;

            var hideTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(totalMs) };
            hideTimer.Tick += (_, __) =>
            {
                hideTimer.Stop();
                ShowTilesLoading(wnd, false);
            };
            hideTimer.Start();
        }


        private static bool EnsureInitialTileSelection(MainWindow wnd)
        {
            if (wnd == null) return false;
            if (wnd.grdGameProfiles.Children.Count == 0) return false;

            // Wenn bereits ein PID gesetzt ist, nichts überschreiben
            if (SysProps.currentSelectedPID != 0) return true;

            SwitchToFirstGameInList(wnd, SysProps.startUpParms.ViewMode);
            return true;
        }



        private static int CalcLoaderTileCount(int count)
        {
            return count - 1;
        }

        private static void BuildDGProfiles(MainWindow wnd)
        {
            List<DBO_Profile> profiles = wnd.txtSearchBar.Text == ""
                ? DM_Profile.ReadAll()
                : DM_Profile.ReadGameName(wnd.txtSearchBar.Text);

            wnd.dgProfiles.Items.Clear();

            foreach (DBO_Profile dbprofile in profiles)
            {
                Profile profile = new Profile
                {
                    ProfileName = dbprofile.GameName,
                    GameTime = dbprofile.GameTime,
                    PID = dbprofile.ProfileID
                };
                wnd.dgProfiles.Items.Add(profile);
            }

            SwitchToFirstGameInList(wnd, StartUpParms.ViewModes.LIST);

            if (profiles.Count == 0)
                BuildInfoDisplayNoGame(wnd);
        }

        public static void BuildGameProfileView(MainWindow wnd)
        {
            if (SysProps.startUpParms.ViewMode == StartUpParms.ViewModes.LIST)
            {
                wnd.grdGameProfiles.Visibility = Visibility.Collapsed;
                wnd.grdGameProfiles.IsHitTestVisible = false;
                wnd.dgProfiles.IsHitTestVisible = true;
                wnd.scrollBarTiles.Visibility = Visibility.Collapsed;
                wnd.dgProfiles.Visibility = Visibility.Visible;
                wnd.scrollBar.Visibility = Visibility.Visible;

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

                AttachContextMenuToDataGrid(wnd.dgProfiles);
            }
        }

        private static (int, int) CalculateRowsAndColumnsGameProfileGrid(List<DBO_Profile> gameProfiles)
        {
            int maxColumnsPerRow = 4;
            int rows = 4;

            int gameProfilesCount = gameProfiles.Count;

            if (gameProfilesCount > 16)
            {
                rows = gameProfilesCount % maxColumnsPerRow != 0
                    ? gameProfilesCount / maxColumnsPerRow + 1
                    : gameProfilesCount / maxColumnsPerRow;
            }

            return (rows, maxColumnsPerRow);
        }

        private static void BuildGameProfileGridRowDefinitons(Grid grid, int rows)
        {
            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        private static void BuildGameProfileGridColumnDefinitions(Grid grid, int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private static void FillGameProfilesGrid(Grid grid, List<DBO_Profile> gameProfiles, MainWindow wnd)
        {
            int columns = grid.ColumnDefinitions.Count;
            if (columns <= 0) return;

            int total = gameProfiles.Count;
            if (total == 0) return;

            _isBuildingTiles = true;

            int index = 0;

            void BuildNextBatch()
            {
                int end = Math.Min(index + TileBuildBatchSize, total);

                for (; index < end; index++)
                {
                    int row = index / columns;
                    int col = index % columns;

                    while (grid.RowDefinitions.Count <= row)
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    var sp = CreateTileStackPanel(grid, gameProfiles[index], wnd, index);

                    Grid.SetRow(sp, row);
                    Grid.SetColumn(sp, col);
                    grid.Children.Add(sp);
                }

                // Selektion während des Builds nachziehen (sobald möglich)
                EnsureSelectionAfterBuild(wnd);

                if (index < total)
                {
                    wnd.Dispatcher.BeginInvoke((Action)BuildNextBatch, DispatcherPriority.Background);
                }
                else
                {
                    _isBuildingTiles = false;

                    wnd.Dispatcher.BeginInvoke((Action)(() => UpdateLazyImages(wnd)), DispatcherPriority.Background);

                    // Nach vollständigem Aufbau Selektion final sicherstellen
                    wnd.Dispatcher.BeginInvoke((Action)(() => EnsureSelectionAfterBuild(wnd)), DispatcherPriority.Loaded);
                }
            }

            wnd.Dispatcher.BeginInvoke((Action)BuildNextBatch, DispatcherPriority.Background);
        }



        private static void EnsureTileVisible(StackPanel tile, ScrollViewer sv)
        {
            if (tile == null || sv == null) return;

            // Nach Layout sicherstellen, dass das Element wirklich messbar ist
            tile.Dispatcher.BeginInvoke((Action)(() =>
            {
                tile.BringIntoView();

                // Zusätzlich: Offset-Korrektur, damit es wirklich im Viewport landet
                try
                {
                    // Position relativ zum ScrollViewer
                    Point p = tile.TransformToAncestor(sv).Transform(new Point(0, 0));
                    // Etwas Abstand nach oben
                    const double topPadding = 10;

                    double target = sv.VerticalOffset + p.Y - topPadding;
                    if (target < 0) target = 0;

                    sv.ScrollToVerticalOffset(target);
                }
                catch
                {
                    // Ignorieren, wenn VisualTree gerade im Umbau ist
                }
            }), DispatcherPriority.Loaded);
        }




        private static StackPanel CreateTileStackPanel(Grid grid, DBO_Profile obj, MainWindow wnd, int gameProfilesIndex)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var tg = new TransformGroup();
            tg.Children.Add(new TranslateTransform(0, 0));
            stackPanel.RenderTransform = tg;

            Border imageBorder = new Border
            {
                CornerRadius = new CornerRadius(5),
                Background = Brushes.Transparent,
                SnapsToDevicePixels = true,
                Effect = VisualHandler.GetDropShadowEffect()
            };

            Border bottomBorder = new Border
            {
                Height = 4,
                Background = SysProps.defButtonColor,
                Width = 0,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                CornerRadius = new CornerRadius(2)
            };

            GTXImage profileImage = new GTXImage
            {
                PID = obj.ProfileID,
                MainWnd = wnd
            };

            double tileSize = GetWidthForImage(grid, wnd);

            string imgPath = SysProps.picDestPath + SysProps.separator + obj.ProfilePicFileName;

            var lazyInfo = new LazyInfo
            {
                Path = File.Exists(imgPath) ? imgPath : null,
                DecodeSize = (int)Math.Ceiling(tileSize * HoverScale),
                Loaded = false
            };

            profileImage.Source = null;
            profileImage.Opacity = 0;

            RenderOptions.SetBitmapScalingMode(profileImage, BitmapScalingMode.HighQuality);
            profileImage.SnapsToDevicePixels = true;

            profileImage.Width = tileSize;
            profileImage.Height = tileSize;
            profileImage.Margin = new Thickness(0);
            profileImage.Clip = new RectangleGeometry(new Rect(0, 0, tileSize, tileSize), 5, 5);

            AttachContextMenuToImage(profileImage);

            TextBlock profileTitle = new TextBlock
            {
                Text = obj.GameName,
                MaxWidth = tileSize,
                FontSize = 16,
                Margin = new Thickness(0, 5, 0, 0),
                Foreground = Brushes.White,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsHitTestVisible = false
            };

            var overlay = new Grid
            {
                Width = tileSize,
                Height = tileSize
            };

            overlay.RenderTransformOrigin = new Point(0.5, 0.5);
            overlay.RenderTransform = new ScaleTransform(1, 1);

            var loader = CreateDotsLoader();

            overlay.Children.Add(profileImage);
            overlay.Children.Add(loader);

            profileImage.Tag = new TileImageTag
            {
                Info = lazyInfo,
                Loader = loader
            };

            if (IsGameInstalled(obj))
            {
                var installedIcon = new GTXImage
                {
                    Source = playableImage,
                    Width = 24,
                    Height = 24,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 5, 5, 0),
                    IsHitTestVisible = false
                };

                overlay.Children.Add(installedIcon);
            }

            imageBorder.Child = overlay;

            stackPanel.Children.Add(imageBorder);
            stackPanel.Children.Add(profileTitle);
            stackPanel.Children.Add(bottomBorder);

            stackPanel.MouseEnter += ProfileImage_MouseEnter;
            stackPanel.MouseLeave += ProfileImage_MouseLeave;
            stackPanel.MouseDown += ProfileImage_MouseDown;

            if (gameProfilesIndex < IntroAnimationLimit)
                ApplyTileIntroAnimation(stackPanel, gameProfilesIndex);
            else
                stackPanel.Opacity = 1;

            return stackPanel;
        }

        private static bool IsGameInstalled(DBO_Profile gameProfile)
        {
            return Directory.Exists(gameProfile.ExtGameFolder) &&
                   FuncExecutables.GetAllExecutablesFromDirectory(gameProfile.ExtGameFolder).Count > 0;
        }

        public static void DeselectNonCurrentProfiles(int currentPID)
        {
            foreach (StackPanel stackPanel in SysProps.mainWindow.grdGameProfiles.Children)
            {
                var img = GetTileImage(stackPanel);
                var txtBlock = GetTileTitle(stackPanel);
                var underline = GetTileUnderline(stackPanel);

                if (img != null && img.PID != currentPID && img.Selected)
                {
                    img.Selected = false;
                    img.DoBorderEffect = true;
                    if (txtBlock != null) txtBlock.FontWeight = FontWeights.Normal;

                    if (underline != null) AnimateUnderlineHoverDot(underline, false);

                    UpdateSelectionGlow(stackPanel, false);
                }
            }
        }

        private static void AttachContextMenuToImage(GTXImage image)
        {
            var contextMenu = new ContextMenu { FontSize = 22 };

            Image MakeIcon(string uri)
            {
                var img = new Image
                {
                    Source = VisualHandler.GetBitmapImage(uri),
                    Width = 19,
                    Height = 19,
                    SnapsToDevicePixels = true
                };
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
                return img;
            }

            var imgProperties = MakeIcon(@"pack://application:,,,/images/properties.png");
            var imgDelete = MakeIcon(@"pack://application:,,,/images/delete.png");
            var imgPlaythrough = MakeIcon(@"pack://application:,,,/images/startpoint.png");
            var imgExecutable = MakeIcon(@"pack://application:,,,/images/executable.png");

            var mIDelete = new GTXMenuItem { Header = "Delete", Icon = imgDelete, PID = image.PID };
            mIDelete.Click += MIDelete_Clicked;

            var mIProperties = new GTXMenuItem { Header = "Properties", Icon = imgProperties, PID = image.PID };
            mIProperties.Click += MIProperties_Clicked;

            var mIPlaythrough = new GTXMenuItem { Header = "New playthrough startpoint", Icon = imgPlaythrough, PID = image.PID };
            mIPlaythrough.Click += MIPlaythrough_Clicked;

            var mIExecutables = new GTXMenuItem { Header = "Change active executables", Icon = imgExecutable, PID = image.PID };
            mIExecutables.Click += MIExecutables_Clicked;

            contextMenu.Items.Add(mIDelete);
            if (DM_Profile.IsPlayTimeGreaterZero(image.PID))
                contextMenu.Items.Add(mIPlaythrough);
            contextMenu.Items.Add(mIProperties);

            DBO_Profile dbo_Profile = DM_Profile.ReadPID(image.PID);
            if (dbo_Profile.ExtGameFolder != string.Empty &&
                Directory.Exists(dbo_Profile.ExtGameFolder) &&
                FuncExecutables.GetAllExecutablesFromDirectory(dbo_Profile.ExtGameFolder).Count > 0)
            {
                contextMenu.Items.Add(mIExecutables);
            }

            contextMenu.Style = VisualHandler.GetApplicationResource("contextMenuStyle") as Style;
            contextMenu.ItemContainerStyle = Application.Current.FindResource("ContextMenuItemLarge") as Style;

            image.ContextMenu = contextMenu;
        }

        private static void MIExecutables_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            if (SysProps.gameRunningHandler != null)
                SysProps.gameRunningHandler.Stop();

            DBO_Profile dbo_Profile = DM_Profile.ReadPID(menuItem.PID);

            if (dbo_Profile.ExtGameFolder != string.Empty)
            {
                CExecutables cExecutables = new CExecutables(dbo_Profile.Executables).Dezerialize();

                if (cExecutables.KeyValuePairs.Count == 0)
                {
                    cExecutables.Initialize(CExecutables.ConvertListToDictionary(
                        FuncExecutables.GetAllExecutablesFromDirectory(dbo_Profile.ExtGameFolder), true));
                    dbo_Profile.Executables = cExecutables.Serialize();
                }

                DM_Profile.Save(dbo_Profile);

                ManageExecutables manageExecutables = new ManageExecutables(dbo_Profile.ProfileID)
                {
                    Owner = SysProps.mainWindow
                };
                manageExecutables.ShowDialog();
            }

            if (SysProps.gameRunningHandler != null)
            {
                List<string> executables = FuncExecutables.GetAllActiveExecutablesFromDBObj(dbo_Profile);
                SysProps.gameRunningHandler.AddExecutables(dbo_Profile.ProfileID, executables);
            }

            if (SysProps.gameRunningHandler != null && !SysProps.gameRunningHandler.IsRunning())
            {
                SysProps.gameRunningHandler.Initialize(DM_Profile.ReadAll());
                SysProps.gameRunningHandler.Start(SysProps.waitTimeGameRunningHandler);
            }
        }

        public static void AttachContextMenuToDataGrid(DataGrid dataGrid)
        {
            ContextMenu contextMenu = new ContextMenu { FontSize = 17 };

            Image imgProperties = new Image
            {
                Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/properties.png")
            };

            Image imgDelete = new Image
            {
                Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/delete.png")
            };

            Image imgPlaythrough = new Image
            {
                Source = VisualHandler.GetBitmapImage(@"pack://application:,,,/images/game_time.png")
            };

            GTXMenuItem mIDelete = new GTXMenuItem { Header = "Delete", Icon = imgDelete };
            mIDelete.Click += MIDelete_DataGrid_Clicked;

            GTXMenuItem mIProperties = new GTXMenuItem { Header = "Properties", Icon = imgProperties };
            mIProperties.Click += MIProperties_DataGrid_Clicked;

            GTXMenuItem mIPlaythrough = new GTXMenuItem { Header = "New playthrough startpoint", Icon = imgPlaythrough };
            mIPlaythrough.Click += MIPlaythrough_DataGrid_Clicked;

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

            DBO_Profile obj = DM_Profile.ReadPID(profile.PID);
            if (obj != null)
            {
                obj.PlaythroughStartPointDate = DateTime.Now;
                DM_Profile.Save(obj);
                BuildInfoDisplay(profile.PID, SysProps.mainWindow);
            }
        }

        private static void MIPlaythrough_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            DBO_Profile dboProfile = DM_Profile.ReadPID(menuItem.PID);
            if (dboProfile != null)
            {
                dboProfile.PlaythroughStartPointDate = DateTime.Now;
                DM_Profile.Save(dboProfile);
                BuildInfoDisplay(menuItem.PID, SysProps.mainWindow);
            }
        }

        private static void MIProperties_DataGrid_Clicked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGrid)contextMenu.PlacementTarget;
            var profile = (Profile)item.SelectedCells[0].Item;

            Properties properties = new Properties(profile.PID)
            {
                Owner = SysProps.mainWindow
            };
            properties.ShowDialog();

            BuildGameProfileView(SysProps.mainWindow);
        }

        private static void MIDelete_DataGrid_Clicked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGrid)contextMenu.PlacementTarget;
            var profile = (Profile)item.SelectedCells[0].Item;

            DBO_Profile dbo_Profile = DM_Profile.ReadPID(profile.PID);

            if (dbo_Profile != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to delete '" + dbo_Profile.GameName + "'?", "Delete", "Cancel")
                {
                    Owner = SysProps.mainWindow
                };
                quest.ShowDialog();

                if (quest.UsrReturnType == QuestionBox.ReturnType.YES)
                {
                    DM_Profile.Delete(dbo_Profile.ProfileID);
                    BuildGameProfileView(SysProps.mainWindow);

                    if (SysProps.gameRunningHandler != null)
                        SysProps.gameRunningHandler.RemoveProfileAndExecutables(dbo_Profile.ProfileID);
                }
            }
        }

        private static void MIProperties_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            if (SysProps.gameRunningHandler != null)
                SysProps.gameRunningHandler.Stop();

            Properties properties = new Properties(menuItem.PID)
            {
                Owner = SysProps.mainWindow
            };
            properties.ShowDialog();

            if (SysProps.gameRunningHandler != null && !SysProps.gameRunningHandler.IsRunning())
                SysProps.gameRunningHandler.Start(SysProps.waitTimeGameRunningHandler);

            BuildGameProfileView(SysProps.mainWindow);
        }

        private static void MIDelete_Clicked(object sender, RoutedEventArgs e)
        {
            GTXMenuItem menuItem = sender as GTXMenuItem;

            DBO_Profile dbo_Profile = DM_Profile.ReadPID(menuItem.PID);

            if (dbo_Profile != null)
            {
                QuestionBox quest = new QuestionBox("Do you really want to delete '" + dbo_Profile.GameName + "'?", "Delete", "Cancel")
                {
                    Owner = SysProps.mainWindow
                };
                quest.ShowDialog();

                if (quest.UsrReturnType == QuestionBox.ReturnType.YES)
                {
                    DM_Profile.Delete(dbo_Profile.ProfileID);
                    BuildGameProfileView(SysProps.mainWindow);

                    if (SysProps.gameRunningHandler != null)
                        SysProps.gameRunningHandler.RemoveProfileAndExecutables(dbo_Profile.ProfileID);
                }
            }
        }

        private static void ProfileImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;

            var image = GetTileImage(stackPanel);
            var txtBlock = GetTileTitle(stackPanel);
            var underline = GetTileUnderline(stackPanel);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (image != null)
                {
                    if (image.PID != SysProps.currentSelectedPID && MonitorHandler.CurrentlyMonitoringGameTime())
                        MonitorHandler.EndMonitoringGameTime(image.MainWnd);

                    image.DoBorderEffect = false;
                    image.Selected = true;
                    if (txtBlock != null) txtBlock.FontWeight = FontWeights.Bold;
                    SysProps.currentSelectedPID = image.PID;

                    DeselectNonCurrentProfiles(image.PID);
                    BuildInfoDisplay(image.PID, image.MainWnd);

                    PulsePress(stackPanel);

                    if (underline != null)
                    {
                        underline.BeginAnimation(FrameworkElement.WidthProperty, null);
                        underline.Width = 0;
                        underline.HorizontalAlignment = HorizontalAlignment.Left;
                        double w = SafeUnderlineWidth(image);
                        AnimateBorderWidth(underline, w, true);
                    }

                    UpdateSelectionGlow(stackPanel, true);
                }
            }
        }

        private static void ProfileImage_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            var image = GetTileImage(stackPanel);
            var underline = GetTileUnderline(stackPanel);

            if (image != null)
            {
                if (image.DoBorderEffect)
                {
                    if (underline != null) AnimateUnderlineHoverDot(underline, false);
                }

                ApplyHoverLift(stackPanel, false);

                image.MainWnd.Cursor = Cursors.Arrow;
            }
        }

        private static void ProfileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            var image = GetTileImage(stackPanel);
            var underline = GetTileUnderline(stackPanel);

            if (image != null)
            {
                if (image.DoBorderEffect && !image.Selected)
                {
                    if (underline != null) AnimateUnderlineHoverDot(underline, true);
                    image.MainWnd.Cursor = Cursors.Hand;
                }

                ApplyHoverLift(stackPanel, true);
            }
        }

        private static double GetWidthForImage(Grid grid, MainWindow wnd)
        {
            const int COLS = 4;
            const double TILE_MARGIN = 10;

            double available = wnd.scrollBarTiles.ActualWidth;
            if (available <= 0) available = wnd.tilesAreaFrame.ActualWidth;

            available -= SystemParameters.VerticalScrollBarWidth;

            available -= wnd.tilesAreaFrame.BorderThickness.Left + wnd.tilesAreaFrame.BorderThickness.Right;
            available -= wnd.grdGameProfiles.Margin.Left + wnd.grdGameProfiles.Margin.Right;

            double totalSideMargins = COLS * 2 * TILE_MARGIN;

            double w = Math.Floor((available - totalSideMargins) / COLS);
            return Math.Max(60, w);
        }

        private static double SafeUnderlineWidth(GTXImage image)
        {
            double w = image.Width;
            if (double.IsNaN(w) || w <= 0) w = image.ActualWidth;
            return w > 0 ? w : 0;
        }

        private static void AnimateBorderWidth(Border border, double targetWidth, bool isMouseEnter)
        {
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = isMouseEnter ? 0 : targetWidth,
                To = isMouseEnter ? targetWidth : 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            border.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
        }

        private static void AnimateUnderlineHoverDot(Border border, bool show)
        {
            border.BeginAnimation(FrameworkElement.WidthProperty, null);
            border.HorizontalAlignment = HorizontalAlignment.Center;

            double from = show ? 0 : border.ActualWidth > 0 ? border.ActualWidth : HoverDotWidth;
            double to = show ? HoverDotWidth : 0;

            var anim = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = HoverDotDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            border.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        private static void ApplyTileIntroAnimation(StackPanel sp, int index)
        {
            sp.Opacity = 0;

            var tg = sp.RenderTransform as TransformGroup;
            var translate = tg?.Children[0] as TranslateTransform;

            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = IntroDuration,
                BeginTime = TimeSpan.FromMilliseconds(80 * index),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            sp.BeginAnimation(UIElement.OpacityProperty, fade);

            if (translate != null)
            {
                var drop = new DoubleAnimation
                {
                    From = 6,
                    To = 0,
                    Duration = IntroDuration,
                    BeginTime = fade.BeginTime,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                translate.BeginAnimation(TranslateTransform.YProperty, drop);
            }
        }

        private static void ApplyHoverLift(StackPanel sp, bool enter)
        {
            double targetScale = enter ? HoverScale : 1.0;
            double targetY = enter ? HoverLiftY : 0.0;

            var overlay = GetTileOverlay(sp);
            var scale = overlay?.RenderTransform as ScaleTransform;
            if (scale != null)
            {
                var anim = new DoubleAnimation
                {
                    To = targetScale,
                    Duration = enter ? HoverDuration : LeaveDuration,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim.Clone());
            }

            var tg = sp.RenderTransform as TransformGroup;
            var translate = tg?.Children[0] as TranslateTransform;

            if (translate != null)
            {
                var animT = new DoubleAnimation
                {
                    To = targetY,
                    Duration = enter ? HoverDuration : LeaveDuration,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                translate.BeginAnimation(TranslateTransform.YProperty, animT);
            }

            var border = (Border)sp.Children[0];
            if (border?.Effect is DropShadowEffect dse)
            {
                var depthAnim = new DoubleAnimation
                {
                    To = enter ? 8 : 5,
                    Duration = enter ? HoverDuration : LeaveDuration
                };
                var blurAnim = new DoubleAnimation
                {
                    To = enter ? 16 : 10,
                    Duration = enter ? HoverDuration : LeaveDuration
                };
                var opAnim = new DoubleAnimation
                {
                    To = enter ? 0.9 : 0.7,
                    Duration = enter ? HoverDuration : LeaveDuration
                };

                dse.BeginAnimation(DropShadowEffect.ShadowDepthProperty, depthAnim);
                dse.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurAnim);
                dse.BeginAnimation(DropShadowEffect.OpacityProperty, opAnim);
            }
        }

        private static void PulsePress(StackPanel sp)
        {
            var overlay = GetTileOverlay(sp);
            var scale = overlay?.RenderTransform as ScaleTransform;

            if (scale != null)
            {
                var a = new DoubleAnimation
                {
                    By = PressScaleDelta,
                    Duration = PressDuration,
                    AutoReverse = true,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, a);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, a.Clone());
            }

            var tg = sp.RenderTransform as TransformGroup;
            var translate = tg?.Children[0] as TranslateTransform;

            if (translate != null)
            {
                var t = new DoubleAnimation
                {
                    By = PressLiftDelta,
                    Duration = PressDuration,
                    AutoReverse = true,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                translate.BeginAnimation(TranslateTransform.YProperty, t);
            }
        }

        private static void UpdateSelectionGlow(StackPanel sp, bool selected)
        {
            var border = (Border)sp.Children[0];
            if (border?.Effect is DropShadowEffect dse)
            {
                var colorAnim = new ColorAnimation
                {
                    To = selected ? SelectedShadowColor : DefaultShadowColor,
                    Duration = TimeSpan.FromMilliseconds(160),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                var blurAnim = new DoubleAnimation
                {
                    To = selected ? 18 : 10,
                    Duration = TimeSpan.FromMilliseconds(160)
                };
                var opAnim = new DoubleAnimation
                {
                    To = selected ? 0.95 : 0.7,
                    Duration = TimeSpan.FromMilliseconds(160)
                };

                var shadowBrush = new SolidColorBrush(dse.Color);
                shadowBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                colorAnim.Completed += (_, __) => dse.Color = selected ? SelectedShadowColor : DefaultShadowColor;

                dse.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurAnim);
                dse.BeginAnimation(DropShadowEffect.OpacityProperty, opAnim);
            }
        }

        private static void HookLazyLoading(MainWindow wnd)
        {
            wnd.scrollBarTiles.ScrollChanged -= Tiles_ScrollChanged;
            wnd.scrollBarTiles.ScrollChanged += Tiles_ScrollChanged;

            if (_lazyUpdateTimer == null)
            {
                _lazyUpdateTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(LazyUpdateThrottleMs)
                };

                _lazyUpdateTimer.Tick += (_, __) =>
                {
                    _lazyUpdateTimer.Stop();

                    if (_lazyUpdatePending && SysProps.mainWindow != null)
                    {
                        _lazyUpdatePending = false;
                        UpdateLazyImages(SysProps.mainWindow);
                    }
                };
            }

            wnd.Dispatcher.InvokeAsync(() => UpdateLazyImages(wnd), DispatcherPriority.Loaded);
        }

        private static void Tiles_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (SysProps.mainWindow == null) return;

            _lazyUpdatePending = true;

            _lazyUpdateTimer?.Stop();
            _lazyUpdateTimer?.Start();
        }

        private static void UpdateLazyImages(MainWindow wnd)
        {
            var sv = wnd.scrollBarTiles;
            var grid = wnd.grdGameProfiles;

            if (sv.ViewportHeight <= 0) return;

            const double preload = 200;
            int index = 0;

            foreach (StackPanel sp in grid.Children)
            {
                var img = GetTileImage(sp);
                if (img?.Tag is not TileImageTag tag) { index++; continue; }

                var info = tag.Info;
                if (info == null) { index++; continue; }
                if (info.Loaded) { index++; continue; }

                bool shouldLoad;

                if (index < InitialImageLoadCount)
                {
                    shouldLoad = true;
                }
                else
                {
                    Point p;
                    try
                    {
                        p = sp.TransformToAncestor(sv).Transform(new Point(0, 0));
                    }
                    catch
                    {
                        index++;
                        continue;
                    }

                    double top = p.Y;
                    double bottom = top + sp.ActualHeight;

                    shouldLoad =
                        bottom >= -preload &&
                        top <= sv.ViewportHeight + preload;
                }

                if (shouldLoad)
                {
                    if (!string.IsNullOrEmpty(info.Path))
                        img.Source = LoadTileBitmapCached(info.Path, info.DecodeSize);
                    else
                        img.Source = GetDefaultProfileImage();

                    FadeInImage(img, 140);
                    info.Loaded = true;

                    if (tag.Loader?.Parent is Panel panel)
                        panel.Children.Remove(tag.Loader);
                }

                index++;
            }
        }

        private static void ShowTilesLoading(MainWindow wnd, bool show)
        {
            var overlay = wnd.loadingOverlay;
            var target = show ? 1.0 : 0.0;

            overlay.Visibility = Visibility.Visible;
            var anim = new DoubleAnimation
            {
                To = target,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            anim.Completed += (_, __) =>
            {
                if (!show) overlay.Visibility = Visibility.Collapsed;
            };
            overlay.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private static GTXImage GetTileImage(StackPanel sp)
        {
            if (sp == null || sp.Children.Count == 0) return null;

            var border = sp.Children[0] as Border;
            if (border == null) return null;

            if (border.Child is GTXImage giDirect) return giDirect;

            if (border.Child is Grid overlay)
            {
                foreach (UIElement child in overlay.Children)
                {
                    if (child is GTXImage gi) return gi;
                }
            }

            return null;
        }

        private static TextBlock GetTileTitle(StackPanel sp) => sp?.Children.Count > 1 ? sp.Children[1] as TextBlock : null;
        private static Border GetTileUnderline(StackPanel sp) => sp?.Children.Count > 2 ? sp.Children[2] as Border : null;

        private static Grid GetTileOverlay(StackPanel sp)
        {
            if (sp == null || sp.Children.Count == 0) return null;
            var border = sp.Children[0] as Border;
            return border?.Child as Grid;
        }

        private static FrameworkElement CreateDotsLoader()
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            for (int i = 0; i < 3; i++)
            {
                var dot = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Margin = new Thickness(2, 0, 2, 0),
                    Fill = Brushes.Gray,
                    Opacity = 0.3
                };

                var anim = new DoubleAnimation
                {
                    From = 0.3,
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(450),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromMilliseconds(i * 150),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                dot.BeginAnimation(UIElement.OpacityProperty, anim);
                panel.Children.Add(dot);
            }

            return panel;
        }
    }
}
