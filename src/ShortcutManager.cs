using System;
using System.IO;

namespace ImageLabelApp
{
    public static class ShortcutManager
    {
        private static readonly string baseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ImageLabels");

        public static void CreateShortcut(string imagePath, string label)
        {
            string labelFolder = Path.Combine(baseFolder, label);
            if (!Directory.Exists(labelFolder))
            {
                Directory.CreateDirectory(labelFolder);
            }

            string destinationPath = Path.Combine(labelFolder, Path.GetFileName(imagePath));

            if (!File.Exists(destinationPath))
            {
                File.Copy(imagePath, destinationPath);
            }
        }

        public static void RemoveShortcut(string imagePath, string label)
        {
            string labelFolder = Path.Combine(baseFolder, label);
            if (!Directory.Exists(labelFolder)) 
            {
                return; 
            }

            string copyPath = Path.Combine(labelFolder, Path.GetFileName(imagePath));

            if (File.Exists(copyPath))
            {
                File.Delete(copyPath);
            }
        }

        public static void RemoveLabelFolders()
        {
            if (Directory.Exists(baseFolder))
            {
                Directory.Delete(baseFolder, true);
            }
        }
    }
}
