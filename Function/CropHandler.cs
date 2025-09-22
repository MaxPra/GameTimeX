using System.Windows.Controls;

namespace GameTimeX.Function
{
    class CropHandler
    {
        // X Position des Crop-Bereichs
        public double CropX { get; set; }

        // Y Position des Crop-Bereichs
        public double CropY { get; set; }

        // Breite des Crop-Bereichs
        public double CropWidth { get; set; }
        // Höhe des CropBereichs
        public double CropHeight { get; set; }

        // Mauspos. X bei Klick
        public double MouseXBeginning { get; set; }
        // Mauspos. X bei Loslassen
        public double MouseXEnd { get; set; }
        // Mauspos. Y bei Klick
        public double MouseYBeginning { get; set; }
        // Mausepos. Y bei Loslassen
        public double MouseYEnd { get; set; }

        public double CanvasHeight { get; set; }
        public double CanvasWidth { get; set; }

        public double PictureRatioWidth { get; set; }
        public double PictureRatioHeight { get; set; }

        public MouseState CurrMouseState { get; set; }


        public enum MouseState
        {
            MOUSE_DOWN,
            MOUSE_UP,
            MOUSE_NONE
        }

        public CropHandler(double cropWidth, double cropHeight, double canvasWidth, double canvasHeight, double pictureRatioWidth, double pictureRatioHeight)
        {
            this.CropWidth = cropWidth;
            this.CropHeight = cropHeight;
            this.CanvasWidth = canvasWidth;
            this.CanvasHeight = canvasHeight;
            this.PictureRatioWidth = pictureRatioWidth;
            this.PictureRatioHeight = pictureRatioHeight;
            this.CurrMouseState = MouseState.MOUSE_NONE;
        }

        /// <summary>
        /// Berechnet in Abhängigkeit der vorhanden Werte die Koordinaten des Crop-Bereichs, wenn die Maus bewegt wurde
        /// </summary>
        /// <param name="xMouseCurrent"></param>
        /// <param name="yMouseCurrent"></param>
        public (double newCropX, double newCropY) GetNewCropPosition(double xMouseCurrent, double yMouseCurrent)
        {
            // Werte umrechnen
            double newCropX = 0;
            double newCropY = 0;

            double mouseMovedPixelsX = MouseXBeginning - xMouseCurrent;
            double mouseMovedPixelsY = MouseYBeginning - yMouseCurrent;

            newCropX = CropX + -mouseMovedPixelsX;
            newCropY = CropY + -mouseMovedPixelsY;

            if (newCropX < 0)
                newCropX = CropX;

            if (newCropY < 0)
                newCropY = CropY;

            if (newCropX + CropWidth > CanvasWidth && mouseMovedPixelsX < 0)
                newCropX = CropX;

            if (newCropY + CropHeight > CanvasHeight && mouseMovedPixelsY < 0)
                newCropY = CropY;

            CropX = newCropX;
            CropY = newCropY;

            MouseXBeginning = xMouseCurrent;
            MouseYBeginning = yMouseCurrent;

            return (newCropX, newCropY);

        }

        /// <summary>
        /// Prüft, ob der Crop-Bereich innerhalb des Bildes sitzt
        /// Ansonsten gibt es dann Probleme beim wirklichen croppen des Bildes (Out-of-Bounds)
        /// </summary>
        /// <returns></returns>
        public bool isCropRecOverEdge()
        {
            if (CropX < 0)
                return true;

            if (CropY < 0)
                return true;

            if (CropX + CropWidth > CanvasWidth)
                return true;

            if (CropY + CropHeight > CanvasHeight)
                return true;

            return false;
        }

        public static int CalculateMinWidthHeight(Image image, int currWidthHeight, double picRatio)
        {
            int minWidth = currWidthHeight;
            int minHeight = currWidthHeight;

            if (currWidthHeight > image.ActualWidth)
            {
                minWidth = (int)image.ActualWidth - 20;
            }

            if (currWidthHeight > image.ActualHeight)
            {
                minHeight = (int)image.ActualHeight - 20;
            }

            return (int)(minWidth < minHeight ? minWidth : minHeight);
        }
    }
}
