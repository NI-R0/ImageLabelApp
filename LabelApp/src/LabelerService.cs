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
            try
            {
                CopyManager.CreateCopy(imagePath);
                DatabaseHandler.AddImage(imagePath);
                DatabaseHandler.AddLabelToImage(imagePath, label);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void UnlabelImage(string imagePath, string label)
        {
            DatabaseHandler.RemoveLabelFromImage(imagePath, label);
            DatabaseHandler.RemoveImage(imagePath);
            CopyManager.RemoveCopy(imagePath);
        }
    }
}
