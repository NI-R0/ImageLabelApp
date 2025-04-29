using System;
using System.IO;
using System.Windows.Forms;

namespace LabelApp
{
    public static class CopyManager
    {
        public static void CreateCopyFolder()
        {
            string path = DatabaseHandler.GetCopyFolder();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateCopy(string imagePath)
        {
            if (DatabaseHandler.ImageExists(imagePath))
            {
                return;
            }

            string labelFolder = DatabaseHandler.GetCopyFolder();
            if (!Directory.Exists(labelFolder))
            {
                Directory.CreateDirectory(labelFolder);
            }

            string extension = Path.GetExtension(imagePath);
            string destinationPath = Path.Combine(labelFolder, DatabaseHandler.ConvertPathToHash(imagePath) + extension);

            if (!File.Exists(destinationPath))
            {
                File.Copy(imagePath, destinationPath);
            }
        }

        public static void RemoveCopy(string imagePath)
        {
            if (DatabaseHandler.ImageExists(imagePath))
            {
                return;
            }
            string labelFolder = DatabaseHandler.GetCopyFolder();
            if (!Directory.Exists(labelFolder)) 
            {
                return; 
            }

            string extension = Path.GetExtension(imagePath);
            string copyPath = Path.Combine(labelFolder, DatabaseHandler.ConvertPathToHash(imagePath) + extension);

            if (File.Exists(copyPath))
            {
                File.Delete(copyPath);
            }
        }

        public static void RemoveCopyFolder()
        {
            string baseFolder = DatabaseHandler.GetCopyFolder();
            if (Directory.Exists(baseFolder))
            {
                Directory.Delete(baseFolder, true);
            }
        }

        public static bool ImageInCopyfolder(string imagePath)
        {
            string baseFolder = DatabaseHandler.GetCopyFolder();
            DirectoryInfo labelDir = new DirectoryInfo(baseFolder);
            DirectoryInfo imageDir = new DirectoryInfo(imagePath);

            return (labelDir.FullName == imageDir.Parent.FullName);

        }
    }
}
