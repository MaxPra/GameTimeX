using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

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

        public static DropShadowBitmapEffect GetDropShadowEffect()
        {
            // Initialize a new DropShadowBitmapEffect that will be applied
            // to the Button.
            DropShadowBitmapEffect myDropShadowEffect = new DropShadowBitmapEffect();
            // Set the color of the shadow to Black.
            Color myShadowColor = new Color();
            myShadowColor.ScA = 1;
            myShadowColor.ScB = 0;
            myShadowColor.ScG = 0;
            myShadowColor.ScR = 0;
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
            VisualHandler.startStopMonitoringBtnActive = true;
        }

        public static void DeactivateMonitoringVisualButton(Button btn)
        {
            btn.Content = SysProps.startMonitoringText;
            btn.Background = new SolidColorBrush((Color)Application.Current.FindResource("ButtonDefaultColor"));
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
    }
}
