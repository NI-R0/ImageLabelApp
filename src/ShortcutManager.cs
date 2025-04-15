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

        public static void CreateShortcut(string targetPath, string label)
        {
            string labelFolder = Path.Combine(baseFolder, label);
            if (!Directory.Exists(labelFolder))
            {
                Directory.CreateDirectory(labelFolder);
            }

            string shortcutName = Path.GetFileName(targetPath) + ".lnk";
            string shortcutPath = Path.Combine(labelFolder, shortcutName);

            var shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        public static void RemoveShortcut(string imagePath, string label)
        {
            string labelFolder = Path.Combine(baseFolder, label);
            string shortcutPath = Path.Combine(labelFolder, Path.GetFileName(imagePath) + ".lnk");

            if (System.IO.File.Exists(shortcutPath))
            {
                System.IO.File.Delete(shortcutPath);
            }
        }
    }
}
