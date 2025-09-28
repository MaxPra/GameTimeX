using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GameTimeX.Function.DataBaseObjectFunctions;

namespace GameTimeX.XApplication.SubDisplays
{
    public partial class YearGameStats : Window
    {
        public YearGameStats()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Bindable data
        public List<YearGameRow> Games { get; set; } = new();

        public string HeaderText => $"Year in Review – {LastYear}";
        public string SubHeaderText => $"Games monitored {LastYear}";

        public double TotalHoursLastYear { get; set; }

        private int LastYear { get; set; }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LastYear = DateTime.Now.Year;

            // TODO: Replace demo data with your actual aggregation
            var all = FN_Profile.GetYearStats();

            // Only games with hours in last year
            Games = all.Where(g => g.HoursLastYear > 0.0)
                       .OrderByDescending(g => g.HoursLastYear)
                       .ToList();

            TotalHoursLastYear = Math.Round(Games.Sum(g => g.HoursLastYear), 1);

            // refresh binding
            dgGames.ItemsSource = null;
            dgGames.ItemsSource = Games;

            // update labels
            DataContext = null; DataContext = this;
        }
    }

    // DTO
    public class YearGameRow
    {
        public string? Title { get; set; }
        public double HoursTotalOverall { get; set; }
        public double HoursLastYear { get; set; }
        public string? CoverUri { get; set; }
    }
}
