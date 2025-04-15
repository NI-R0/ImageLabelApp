using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;

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

            if (!System.IO.File.Exists(destinationPath))
            {
                System.IO.File.Copy(imagePath, destinationPath);
            }


            //string shortcutName = Path.GetFileName(targetPath) + ".lnk";
            //string shortcutPath = Path.Combine(labelFolder, shortcutName);

            //var shell = new WshShell();
            //IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            //shortcut.TargetPath = targetPath;
            //shortcut.Save();
        }

        public static void RemoveShortcut(string imagePath, string label)
        {
            string labelFolder = Path.Combine(baseFolder, label);
            if (!Directory.Exists(labelFolder)) 
            {
                return; 
            }

            string copyPath = Path.Combine(labelFolder, Path.GetFileName(imagePath));

            if (System.IO.File.Exists(copyPath))
            {
                System.IO.File.Delete(copyPath);
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
