using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using GameTimeX.Function.AppEnvironment;

namespace GameTimeX.Function.Windows
{
    internal class ClipBoardManager
    {
        public static BitmapSource GetImageFromClipboard()
        {
            if (Clipboard.ContainsImage())
                return Clipboard.GetImage();

            return null;
        }

        public static string SaveClipboardImageToDisk()
        {
            string filePath = Path.Combine(SysProps.tempImgFolder, "TIMG_" + GenerateRandomHashHex(16) + ".jpg");

            try
            {
                BitmapSource clipImage = GetImageFromClipboard();

                if (clipImage == null)
                    return null;

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(clipImage));
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return filePath;
        }

        public static string GenerateRandomHashHex(int byteLength = 32)
        {
            byte[] bytes = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return BytesToHex(bytes);
        }

        private static string BytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}
