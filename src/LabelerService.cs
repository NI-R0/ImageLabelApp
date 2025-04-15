using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLabelApp
{
    public static class LabelerService
    {
        public static void LabelImage(string imagePath, string label)
        {
            LabelDatabase.AddLabel(imagePath, label);
            ShortcutManager.CreateShortcut(imagePath, label);
        }

        public static void UnlabelImage(string imagePath, string label)
        {
            LabelDatabase.RemoveLabel(imagePath, label);
            ShortcutManager.RemoveShortcut(imagePath, label);
        }

        public static void DeleteApplication()
        {
            LabelDatabase.DeleteDatabaseFile();
            ShortcutManager.RemoveLabelFolders();
        }
    }
}
