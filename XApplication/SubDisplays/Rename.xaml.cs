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
    public partial class Rename : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        public int Pid { get; set; }

        public DBObject CurrObject { get; set; }   

        public Rename()
        {
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

        private void btnRenameProfile_Click(object sender, RoutedEventArgs e)
        {

            if (DisplayHandler.CheckDisplay(false, txtProfileName))

            {
                // Werte in Datenbank speichern
                DBObject dbObject = DataBaseHandler.ReadPID(CurrObject.ProfileID);
                dbObject.GameName = txtProfileName.Text;
                DataBaseHandler.Save(dbObject);
                Close();
            }
        }

        private void btnRenameProfile_MouseEnter(object sender, MouseEventArgs e)
        {
            btnRename.Background = SysProps.defButtonHoverColor;
        }

        private void btnRenameProfile_MouseLeave(object sender, MouseEventArgs e)
        {
            btnRename.Background = SysProps.defButtonColor;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(CurrObject != null)
                txtProfileName.Text = CurrObject.GameName;
        }
    }
}
