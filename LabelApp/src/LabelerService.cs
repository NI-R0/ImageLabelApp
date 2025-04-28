using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelApp
{
    public static class LabelerService
    {
        public static void LabelImage(string imagePath, string label)
        {
            DatabaseHandler.AddImage(imagePath);
            DatabaseHandler.AddLabelToImage(imagePath, label);
            ShortcutManager.CreateShortcut(imagePath, label);
        }

        public static void UnlabelImage(string imagePath, string label)
        {
            DatabaseHandler.RemoveLabelFromImage(imagePath, label);
            ShortcutManager.RemoveShortcut(imagePath, label);
            DatabaseHandler.RemoveImage(imagePath);
        }
    }
}
