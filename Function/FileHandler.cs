using GameTimeX.Objects;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameTimeX
{ 
    internal class FileHandler
    {

        /// <summary>
        /// Gibt zum übergebenen Dateinamen einen Hash zurück
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetHashFromFilename(string filepath)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
            int hash = filename.GetHashCode() % 10000;
            return hash.ToString("0000") + ".jpg";
        }

        /// <summary>
        /// Schneided das übergebene Bild nach den übergebenen Parametern zu und speichert das Bild am
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="widthIn"></param>
        /// <param name="heightIn"></param>
        /// <param name="outputPath"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        public static bool CropImageAndSave(string filePath, int widthIn, int heightIn, string outputPath, string outputFileName, int startX, int startY)
        {
            // get the image
            using SKImage sKImage = SKImage.FromEncodedData(filePath);

            // check if the given sides are not larger than the image size
            if (widthIn < sKImage.Width && heightIn < sKImage.Height)
            {
                //// find the center
                //int centerX = sKImage.Width / 2;
                //int centerY = sKImage.Height / 2;

                //// find the Start points
                //int startX = centerX - widthIn / 2;
                //int startY = centerY - heightIn / 2;

                // crop the image
                SKImage croppedImage = sKImage.Subset(SKRectI.Create(startX, startY, widthIn, heightIn));
                using SKData sKData = croppedImage.Encode(SKEncodedImageFormat.Jpeg, 100);

                using (var stream = File.OpenWrite(Path.Combine(outputPath, outputFileName)))
                {
                    // Save the data to a stream
                    sKData.SaveTo(stream);
                }

                return true;
            }
            else return false;
        }

        /// <summary>
        /// Erstellt alle nötigen Ordner für das Programm
        /// </summary>
        public static void CreateProgramFoldersAndFiles()
        {
            if (!Directory.Exists(SysProps.programPathFolder))
            {
                Directory.CreateDirectory(SysProps.programPathFolder);
            }

            if (!Directory.Exists(SysProps.picDestPath))
            {
                Directory.CreateDirectory(SysProps.picDestPath);
            }

            if (!File.Exists(SysProps.startUpParmsPath))
            {
                // Leere JSON-Datei erstellen
                StartUpParms parms = new StartUpParms();
                string jsonParms = JsonSerializer.Serialize(parms);
                File.WriteAllText(SysProps.startUpParmsPath, jsonParms);
            }
        }

        public static void DeleteUnusedImages()
        {
            List<DBObject> profiles = DataBaseHandler.ReadAll();

            string[] files =  Directory.GetFiles(SysProps.picDestPath);

            foreach (string file in files)
            {
                bool used = false;
                foreach (DBObject obj in profiles)
                {
                   if(obj.ProfilePicFileName == Path.GetFileName(file))
                    {
                        used = true;
                    }
                }

                if (!used)
                    File.Delete(file);
            }
        }

        private static void DeleteAllProgramFolders()
        {
            // Alle Ordner werden gelöscht
            Directory.Delete(SysProps.programPathFolder, true);
        }

        /// <summary>
        /// Erstellt ein Backup der DB und Bilder
        /// </summary>
        /// <param name="backPath"></param>
        public static void CreateBackup()
        {
            // Backup-Pfad
            string backPath = SysProps.startUpParms.BackupPath;

            // Vor dem Backup muss die StartUpParms-JSON angepasst werden
            // Ansonsten würden wir hier in eine Endlosschleife geraten, da GameTimeX immer versuchen würde
            // ein Backup zu ziehen
            SysProps.startUpParms.BackupType = StartUpParms.BackupTypes.NO_BACKUP;
            SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

            // Pfad vorhanden?
            if (!Directory.Exists(backPath))
            {
                return;
            }

            // Zip-Datei erstellen
            // Name der Zip-Datei
            string zipName = "GameTimeX_Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
            ZipFile.CreateFromDirectory(SysProps.programPathFolder, backPath + SysProps.separator + zipName);
        }

        public static void ImportBackup()
        {
            //Backup Pfad --> Pfad + name.zip
            string backImportPath = SysProps.startUpParms.BackUpImportPath;

            //Prüfen ob angegebene Datei exisitert
            if (!File.Exists(backImportPath))
                return;

            // -- BACKUP IMPORTIEREN --
            // Vorher gesamten Programmordner löschen
            DeleteAllProgramFolders();

            // Zip-File extrahieren
            ZipFile.ExtractToDirectory(backImportPath, SysProps.programPathFolder);
        }

        public static StartUpParms ReadStartParms(string startParmsPath)
        {
            // Prüfen ob startParmsPath Wert enthält
            if (startParmsPath.Equals(string.Empty))
                return null;

            // Json einlesen
            string startParmsJSON = File.ReadAllText(startParmsPath);

            if(startParmsJSON != null)
            {
                StartUpParms startUpParms = JsonSerializer.Deserialize<StartUpParms>(startParmsJSON);

                return startUpParms;
            }

            return null;
        }

        public static void SaveStartParms(string startParmsPath, StartUpParms startUpParms)
        {
            // Prüfen ob startParmsPath Wert enthält
            if (startParmsPath.Equals(string.Empty))
                return;

            // JSON-Objekt in String umwandeln
            string startParmsJSON = JsonSerializer.Serialize(startUpParms);

            // In File abspeichern
            File.WriteAllText(startParmsPath, startParmsJSON);
        }
    }
}
