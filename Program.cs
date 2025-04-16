﻿using System;
using System.Windows.Forms;
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
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new forms.InstallForm());
                return;
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
                        return;

                    default:
                        Console.WriteLine("Invalid command. Use 'add' or 'remove'.");
                        return;
                }
            }
        }
    }
}
