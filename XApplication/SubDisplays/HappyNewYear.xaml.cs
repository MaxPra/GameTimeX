using System.Windows;

namespace GameTimeX.XApplication.SubDisplays
{
    /// <summary>
    /// Interaktionslogik für HappyNewYear.xaml
    /// </summary>
    public partial class HappyNewYear : Window
    {
        public HappyNewYear()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
