using GameTimeX.Function;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für ImageCropper.xaml
    /// </summary>
    public partial class ImageCropper : Window
    {
        // --- Rückgabe an Parent-Programm ---
        public double CropX { get; set; }
        public double CropY { get; set; }

        public double CropWidth { get; set; }
        public double CropHeight { get; set; }
        // ------------------------------------

        // Größe des Crop-Bereichs
        private double cropWidthHeightRec = 300;

        // Verhältnis von Original- und Anzeigebild
        private double picRatioScale = 0;

        // CropHandler
        private CropHandler cropHandler = null;

        // Bildpfad
        private string ImagePath { get; set; }

        BitmapImage bitProfilePic;

        public ImageCropper(string imagePath)
        {
            // Setzen des Bildpfades
            ImagePath = imagePath;
            InitializeComponent();

            // Dieser Code muss hier ausgeführt werden!!!
            // Ansonsten haben wir Probleme mit ActualWidth & ActualHeight!!!
            bitProfilePic = new BitmapImage();
            bitProfilePic.BeginInit();
            bitProfilePic.UriSource = new Uri(ImagePath);
            bitProfilePic.EndInit();

            imgProfilePic.Stretch = Stretch.Uniform;
            imgProfilePic.Source = bitProfilePic;
        }
        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
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
            if (!cropHandler.isCropRecOverEdge())
            {
                // Beim Schließen des Fensters Werte in Attribute speichern, 
                // damit Parent darauf zugreifen kann
                CropX = cropHandler.CropX * picRatioScale;
                CropY = cropHandler.CropY * picRatioScale;
                CropWidth = cropHandler.CropWidth * picRatioScale;
                CropHeight = cropHandler.CropHeight * picRatioScale;

                Close();
            }
            else
            {
                InfoBox info = new InfoBox("Please make sure the crop box is inside the boundries!");
                info.Owner = this;
                info.ShowDialog();
            }

        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Background = VisualHandler.convertHexToBrush(SysProps.hexValCloseWindow);
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = Brushes.Transparent;
        }

        private void recTransformArea_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Dieser Code muss im "Loaded" passieren!!!
            // Wenn Bild nicht vollständig geladen --> ActualWidth = 0 --> Probleme!!
            cvsCropper.Width = imgProfilePic.ActualWidth;
            cvsCropper.Height = imgProfilePic.ActualHeight;

            //this.SizeToContent = SizeToContent.WidthAndHeight;

            // Achtung: Hier muss PixelWidth verwendet werden (wir benötigen die Width in Pixeln)
            // Bei manchen Bildern ist Width & PixelWidth unterschiedlich
            // => das führt beim Croppen zu Problemen (Crop-Bereich größer als Bild!!)
            picRatioScale = bitProfilePic.PixelWidth / imgProfilePic.ActualWidth;

            cropHandler = new CropHandler(300, 300, cvsCropper.Width, cvsCropper.Height, picRatioScale, picRatioScale);

        }

        private void recTransformArea_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void recTransformArea_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Koordinaten (X & Y) des Crop-Bereichs holen
            double recX = Canvas.GetLeft(recTransformArea);
            double recY = Canvas.GetTop(recTransformArea);

            // Koordinaten (X & Y) der Maus holen
            var pos = Mouse.GetPosition(cvsCropper);

            // Crophandler mitgeben
            cropHandler.CropX = recX;
            cropHandler.CropY = recY;

            // Mousekoordinaten bei Click mitgeben
            cropHandler.mouseXBeginning = pos.X;
            cropHandler.mouseYBeginning = pos.Y;

            // MouseState setzen
            cropHandler.mouseState = CropHandler.MouseState.MOUSE_DOWN;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // MouseState setzen
            cropHandler.mouseState = CropHandler.MouseState.MOUSE_UP;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Wenn MouseButton nicht gedrückt --> Ausstieg!
            if (cropHandler.mouseState != CropHandler.MouseState.MOUSE_DOWN)
                return;

            // Koordinaten (X & Y) der Maus holen
            var pos = Mouse.GetPosition(cvsCropper);

            (double, double) cropPosition = cropHandler.GetNewCropPosition(pos.X, pos.Y);


            Canvas.SetLeft(recTransformArea, cropPosition.Item1);
            Canvas.SetTop(recTransformArea, cropPosition.Item2);
        }

        /// <summary>
        /// Scrollevent, um Crop-Bereich zu vergrößern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageCropperWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Vergrößern
            if (e.Delta > 0)
            {
                double cropWidthHeightRecTemp = cropWidthHeightRec;
                cropWidthHeightRecTemp += 20;
                if (cropWidthHeightRecTemp < 700)
                {
                    cropWidthHeightRec = cropWidthHeightRecTemp;
                }
            }
            // Verkleinern
            else if (e.Delta < 0)
            {
                double cropWidthHeightRecTemp = cropWidthHeightRec;
                cropWidthHeightRecTemp -= 20;
                if (cropWidthHeightRecTemp > 300)
                {
                    cropWidthHeightRec = cropWidthHeightRecTemp;
                }
            }

            cropHandler.CropWidth = cropWidthHeightRec;
            cropHandler.CropHeight = cropWidthHeightRec;

            recTransformArea.Width = cropWidthHeightRec;
            recTransformArea.Height = cropWidthHeightRec;
        }
    }
}
