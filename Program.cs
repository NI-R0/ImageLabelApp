using System;
using System.Windows.Forms;

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

            if (args.Length == 2) 
            {
                //string imagePath = args[0];
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //MessageBox.Show("Trying to load edit window", "Success",
                //    MessageBoxButtons.OK);
                //Application.Run(new forms.LabelManagerForm(imagePath));
                try
                {
                    if (args.Length > 0)
                    {
                        string imagePath = args[0];
                        // Use Form.Show() for debugging to see any exceptions
                        var form = new forms.LabelManagerForm(imagePath);
                        Application.Run(form);
                    }
                    else
                    {
                        MessageBox.Show("No image path provided.", "Error",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}",
                                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        return;

                    default:
                        Console.WriteLine("Invalid command. Use 'add' or 'remove'.");
                        return;
                }
            }
        }
    }
}
