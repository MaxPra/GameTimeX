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
    public partial class  QuestionBox : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        public int pid { get; set; }

        private string infoText ="";
        private string yesText = "";
        private string noText = "";

        public enum ReturnType{
            YES,
            NO,
            NONE
        }

        public ReturnType returnType { get; set; }

        public QuestionBox(string infoText, string yesText, string noText)
        {
            InitializeComponent();

            this.infoText = infoText;
            this.yesText = yesText;
            this.noText = noText;
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

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblInfoText.Text = infoText;  
            btnYes.Content = yesText;   
            btnNo.Content = noText;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            returnType = ReturnType.NO;
            Close();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            returnType=ReturnType.YES;
            Close();
        }
    }
}
