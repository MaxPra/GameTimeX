using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameTimeX.Objects;

namespace GameTimeX
{
    internal class VisualHandler
    {

        public static String defHexVal = "#0099ff";
        public static bool startStopMonitoringBtnActive = false;

        public static Brush ConvertHexToBrush(string hexVal)
        {
            // Wird null übergeben, auf default-Wert gestellt
            if (hexVal == null)
            {
                hexVal = defHexVal;
            }

            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString(hexVal);

            return brush;

        }

        public static Color ConvertHexToColor(string hexVal)
        {
            // Wird null übergeben, auf default-Wert gestellt
            if (hexVal == null)
            {
                hexVal = defHexVal;
            }

            var color = (Color)ColorConverter.ConvertFromString(hexVal);

            return color;
        }

        public static DropShadowEffect GetDropShadowEffect()
        {
            // Initialize a new DropShadowBitmapEffect that will be applied
            // to the Button.
            DropShadowEffect myDropShadowEffect = new DropShadowEffect();
            // Set the color of the shadow to Black.
            Color myShadowColor = new Color();
            myShadowColor.ScA = 1;
            myShadowColor.ScB = 0;
            myShadowColor.ScG = 0;
            myShadowColor.ScR = 0;
            myDropShadowEffect.Color = myShadowColor;

            // Set the direction of where the shadow is cast to 320 degrees.
            myDropShadowEffect.Direction = 315;

            // Set the depth of the shadow being cast.
            myDropShadowEffect.ShadowDepth = 5;

            myDropShadowEffect.BlurRadius = 10;

            // Set the shadow softness to the maximum (range of 0-1).
            //myDropShadowEffect. = 1;
            // Set the shadow opacity to half opaque or in other words - half transparent.
            // The range is 0-1.
            myDropShadowEffect.Opacity = 1;

            return myDropShadowEffect;
        }

        public static DropShadowBitmapEffect GetDropShadowEffectHover()
        {
            // Initialize a new DropShadowBitmapEffect that will be applied
            // to the Button.
            DropShadowBitmapEffect myDropShadowEffect = new DropShadowBitmapEffect();
            // Set the color of the shadow to Black.
            Color myShadowColor = ConvertHexToColor(SysProps.hexValDefHover);

            myDropShadowEffect.Color = myShadowColor;

            // Set the direction of where the shadow is cast to 320 degrees.
            myDropShadowEffect.Direction = 320;

            // Set the depth of the shadow being cast.
            myDropShadowEffect.ShadowDepth = 25;

            // Set the shadow softness to the maximum (range of 0-1).
            myDropShadowEffect.Softness = 1;
            // Set the shadow opacity to half opaque or in other words - half transparent.
            // The range is 0-1.
            myDropShadowEffect.Opacity = 0.5;

            return myDropShadowEffect;
        }

        public static void ActivateMonitoringVisualButton(Button btn)
        {
            btn.Content = SysProps.stopMonitoringText;
            btn.Background = new SolidColorBrush((Color)Application.Current.FindResource("ButtonDefaultMonitoringColor"));
            ((Path)btn.Template.FindName("Icon", btn)).Data = Geometry.Parse("M2,2 H12 V12 H2 Z");
            VisualHandler.startStopMonitoringBtnActive = true;
        }

        public static void DeactivateMonitoringVisualButton(Button btn)
        {
            btn.Content = SysProps.startMonitoringText;
            btn.Background = new SolidColorBrush((Color)Application.Current.FindResource("ButtonDefaultColor"));
            ((Path)btn.Template.FindName("Icon", btn)).Data = Geometry.Parse("M 0,0 L 0,12 L 10,6 Z");
            VisualHandler.startStopMonitoringBtnActive = false;
        }

        public static void DeactivateGameTimeSeesion(TextBlock txtBlock)
        {
            txtBlock.Visibility = Visibility.Hidden;
        }

        public static void ActivateGameTimeSeesion(TextBlock txtBlock)
        {
            txtBlock.Visibility = Visibility.Visible;
            txtBlock.Text = "Session: 0 minute(s)";
        }

        public static async void ShowToastNotification(string title, string message, int durationMillis)
        {
            ToastMessage toastMessage = new ToastMessage(title, message);
            toastMessage.Topmost = true;
            toastMessage.Focusable = false;
            toastMessage.Show();
            await Task.Delay(durationMillis);
            toastMessage.Close();
        }

        public static object GetApplicationResource(string resourceName)
        {
            if (SysProps.mainWindow != null)
                return SysProps.mainWindow.FindResource(resourceName);

            return new object();
        }

        public static BitmapImage GetBitmapImage(string uri)
        {
            BitmapImage returnImage = new BitmapImage();
            returnImage.BeginInit();
            returnImage.UriSource = new Uri(uri);
            returnImage.EndInit();

            return returnImage;
        }

        public static BitmapImage GetModePic(StartUpParms.ViewModes mode)
        {
            BitmapImage bitModePic = new BitmapImage();

            if (mode == StartUpParms.ViewModes.LIST)
            {

                bitModePic.BeginInit();
                bitModePic.UriSource = new Uri("pack://application:,,,/images/list.png");
                bitModePic.EndInit();
            }
            else
            {
                bitModePic.BeginInit();
                bitModePic.UriSource = new Uri("pack://application:,,,/images/tiles.png");
                bitModePic.EndInit();
            }

            return bitModePic;

        }

        /// <summary>
        /// Setzt für das übergebene Image einen Corner-Radius
        /// Die Höhe und Breite wird hier von Actual-Height bzw. -Width des Image gezogen.
        /// </summary>
        /// <param name="image">Image Element</param>
        /// <param name="radiusX">Radius für X</param>
        /// <param name="radiusY">Radius für Y</param>
        public static void SetCornerRadiusImage(Image image, int radiusX, int radiusY)
        {
            RectangleGeometry clipGeometry = new RectangleGeometry
            {
                RadiusX = radiusX,
                RadiusY = radiusY,
                Rect = new Rect(0, 0, image.ActualWidth, image.ActualHeight)
            };

            image.Clip = clipGeometry;
        }
    }
}
