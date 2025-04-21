using System;
using System.IO;

namespace ImageLabelApp
{
    public static class ShortcutManager
    {
        private static readonly string baseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Labels");

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

        public static void RemoveLabelFolder(string labelName)
        {
            string labelFolder = Path.Combine(baseFolder, labelName);
            if (Directory.Exists(labelFolder))
            {
                Directory.Delete(labelFolder, true);
            }
        }

        public static void RemoveLabelFolders()
        {
            if (Directory.Exists(baseFolder))
            {
                Directory.Delete(baseFolder, true);
            }
        }

        public static bool ImageInLabelfolder(string imagePath)
        {
            DirectoryInfo labelDir = new DirectoryInfo(baseFolder);
            DirectoryInfo imageDir = new DirectoryInfo(imagePath);

            while (imageDir.Parent != null)
            {
                if (labelDir.FullName == imageDir.FullName)
                {
                    return true;
                }
                imageDir = imageDir.Parent;
            }
            return false;
        }
    }
}
