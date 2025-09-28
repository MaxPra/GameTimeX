using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GameTimeX.Function.Steam;

namespace GameTimeX.XApplication.SubDisplays
{
    public partial class SteamImportWindow : Window
    {
        public SteamImportWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<SteamGame> Games { get; set; } = new();
        public SteamGame? SelectedGame { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public bool InstalledOnly { get; set; }
        public bool IsScanning { get; set; }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SelectedGame = null;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Defaultmäßig Installierte Spiele anhaken
            cbInstalledOnly.IsChecked = true;

            LoadGames();
            txtSearch?.Focus();
            txtSearch?.SelectAll();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e) => LoadGames();

        private void LoadGames()
        {
            try
            {
                IsScanning = true;

                var root = SteamLocatorHandler.GetSteamRoot();
                if (string.IsNullOrEmpty(root))
                {
                    var info = new InfoBox("Steam installation not found.");
                    info.Owner = this;
                    info.ShowDialog();
                    return;
                }

                var libs = SteamLibrariesHandler.GetLibraryPaths(root);
                var games = SteamManifestsHandler.ScanAllGames(libs);

                // Tools/Redistributables ausblenden
                string[] noiseTokens =
                {
                    "steamworks", "redistributable", "proton", "steam linux runtime",
                    "shader", "benchmark", "dedicated server"
                };

                games = games
                    .Where(g => !noiseTokens.Any(t =>
                               g.Name?.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();

                // Nur installierte, wenn Checkbox aktiv
                if (InstalledOnly)
                    games = games.Where(g => SteamManifestsHandler.ResolveInstallPath(g) != null).ToList();

                // Suche
                if (!string.IsNullOrWhiteSpace(SearchText))
                    games = games.Where(g => g.Name?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                // Duplikate entfernen (pro AppID ein Eintrag; bevorzugt mit gültigem InstallPath)
                games = games
                    .GroupBy(g => g.AppId)
                    .Select(grp =>
                        grp.OrderByDescending(x => !string.IsNullOrEmpty(SteamManifestsHandler.ResolveInstallPath(x)))
                           .ThenByDescending(x => !string.IsNullOrEmpty(x.InstallDir))
                           .ThenBy(x => x.LibraryPath)
                           .First())
                    .OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                Games = games;
                SelectedGame = null;

                // Binding refresh
                dgGames.ItemsSource = null;
                dgGames.ItemsSource = Games;
            }
            finally
            {
                IsScanning = false;
            }
        }


        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGame == null)
            {
                var info = new InfoBox("Please select a game to import.");
                info.Owner = this;
                info.ShowDialog();
                return;
            }

            DialogResult = true;
            Close();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Bei Enter Aktualisieren
                LoadGames();
                e.Handled = true;
            }
        }

        private void dgGames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Wenn Selektion geändert prüfen, ob Spiel ausgewählt
            if (SelectedGame != null)
            {
                btnImport.IsEnabled = true;
            }
            else
            {
                btnImport.IsEnabled = false;
            }
        }
    }
}
