using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeX.Function
{
    class CropHandler
    {
        // X Position des Crop-Bereichs
        public double CropX {  get; set; }

        // Y Position des Crop-Bereichs
        public double CropY { get; set; }

        // Breite des Crop-Bereichs
        public double CropWidth { get; set; }
        // Höhe des CropBereichs
        public double CropHeight {  get; set; }   

        // Mauspos. X bei Klick
        public double mouseXBeginning { get; set; }
        // Mauspos. X bei Loslassen
        public double mouseXEnd {  get; set; }
        // Mauspos. Y bei Klick
        public double mouseYBeginning { get; set; }
        // Mausepos. Y bei Loslassen
        public double mouseYEnd {  get; set; }
        
        public double canvasHeight { get; set; }
        public double canvasWidth { get; set; }

        public double pictureRatioWidth { get; set; }
        public double pictureRatioHeight { get; set; }

        public MouseState mouseState { get; set; }


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
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.pictureRatioWidth = pictureRatioWidth;
            this.pictureRatioHeight = pictureRatioHeight;
            this.mouseState = MouseState.MOUSE_NONE;
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

            double mouseMovedPixelsX = mouseXBeginning - xMouseCurrent;
            double mouseMovedPixelsY = mouseYBeginning - yMouseCurrent;

            newCropX = CropX + -mouseMovedPixelsX;
            newCropY = CropY + -mouseMovedPixelsY;

            if (newCropX < 0)
                newCropX = CropX;

            if(newCropY < 0)
                newCropY = CropY;

            if (newCropX + CropWidth > canvasWidth && mouseMovedPixelsX < 0)
                newCropX = CropX;

            if (newCropY + CropHeight> canvasHeight && mouseMovedPixelsY < 0)
                newCropY = CropY;

            CropX = newCropX;
            CropY = newCropY;

            mouseXBeginning = xMouseCurrent;
            mouseYBeginning = yMouseCurrent;

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

            if (CropX + CropWidth > canvasWidth)
                return true;

            if (CropY + CropHeight > canvasHeight)
                return true;

            return false;
        }
    }
}
