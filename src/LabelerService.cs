using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLabelApp
{
    public static class LabelerService
    {
        public static void LabelImage(string imagePath, string label)
        {
            LabelDatabase.AddLabelToImage(imagePath, label);
            ShortcutManager.CreateShortcut(imagePath, label);
        }

        public static void UnlabelImage(string imagePath, string label)
        {
            try
            {
                LabelDatabase.RemoveLabelFromImage(imagePath, label);
                ShortcutManager.RemoveShortcut(imagePath, label);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
