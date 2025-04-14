using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLabelApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "install")
            {
                ContextMenuInstaller.InstallContextMenu();
                ContextMenuInstaller.InstallRemoveContextMenus();
                Console.WriteLine("Context menu installed!");
            }
            else
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: ImageLabeler.exe <image-path> <label>");
                    return;
                }

                if (args.Length == 2)
                {
                    string imagePath = args[0];
                    string label = args[1];
                    LabelerService.LabelImage(imagePath, label);
                    return;
                }


                if (args.Length == 3 && args[0] == "--remove")
                {
                    string imagePath = args[1];
                    string label = args[2];

                    LabelDatabase.RemoveLabel(imagePath, label);
                    Console.WriteLine($"Removed label '{label}' from '{imagePath}'");
                    return;
                }
            }
        }
    }
}
