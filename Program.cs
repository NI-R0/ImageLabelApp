using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLabelApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "install":
                        ContextMenuInstaller.InstallContextMenu();
                        Console.WriteLine("Context menu installed!");
                        return;

                    case "uninstall":
                        ContextMenuInstaller.UninstallContextMenu();
                        LabelerService.DeleteApplication();
                        Console.WriteLine("Context menu uninstalled!");
                        return;

                    default:
                        Console.WriteLine("Invalid argument. Use 'install' or 'uninstall'.");
                        return;
                }
            }

            if (args.Length == 3)
            {
                string imagePath = args[0];
                string command = args[1].ToLower();
                string label = args[2];

                if (!System.IO.File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found.");
                    return;
                }

                switch (command)
                {
                    case "add":
                        LabelerService.LabelImage(imagePath, label);
                        Console.WriteLine($"Added label '{label}' to '{imagePath}'");
                        return;

                    case "remove":
                        LabelerService.UnlabelImage(imagePath, label);
                        Console.WriteLine($"Removed label '{label}' from '{imagePath}'");
                        //if (LabelDatabase.IsImageLabeled(imagePath, label))
                        //{
                        //    LabelerService.UnlabelImage(imagePath, label);
                        //    Console.WriteLine($"Removed label '{label}' from '{imagePath}'");
                        //}
                        //else
                        //{
                        //    Console.WriteLine($"'{imagePath}' is not labeled as '{label}'");
                        //}
                        return;

                    default:
                        Console.WriteLine("Invalid command. Use 'add' or 'remove'.");
                        return;
                }
            }

            Console.WriteLine("Usage:");
            Console.WriteLine("  ImageLabelApp.exe install");
            Console.WriteLine("  ImageLabelApp.exe uninstall");
            Console.WriteLine("  ImageLabelApp.exe <image-path> add|remove <label>");
        }
    }
}
