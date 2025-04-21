using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ImageLabelApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                if (!IsAdministrator()) 
                {
                    RestartAsAdmin("");
                    return;
                }
                Application.Run(new forms.InstallForm());
            }

            else if (args.Length == 1) 
            {
                var operation = args[0];
                if (operation == "--install-context-menu")
                {
                    if (!IsAdministrator())
                    {
                        RestartAsAdmin("--install-context-menu");
                        return;
                    }
                    DatabaseHandler.CreateDatabase();
                    ContextMenuManager.InstallContextMenu();
                    MessageBox.Show("Context menu entries installed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else if (operation == "--uninstall-context-menu")
                {
                    if (!IsAdministrator())
                    {
                        RestartAsAdmin("--uninstall-context-menu");
                        return;
                    }
                    ShortcutManager.RemoveLabelFolders();
                    DatabaseHandler.DeleteDatabase();
                    ContextMenuManager.UninstallContextMenu();
                    MessageBox.Show("Context menu entries removed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else if (operation == "edit")
                {
                    if (!IsAdministrator())
                    {
                        RestartAsAdmin("edit");
                        return;
                    }
                    Application.Run(new forms.LabelManagerForm());
                }
            }

            else if (args.Length == 3)
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
                        if (ShortcutManager.ImageInLabelfolder(imagePath))
                        {
                            return;
                        }
                        LabelerService.LabelImage(imagePath, label);
                        Console.WriteLine($"Added label '{label}' to '{imagePath}'");
                        return;

                    case "remove":
                        if (ShortcutManager.ImageInLabelfolder(imagePath))
                        {
                            return;
                        }
                        LabelerService.UnlabelImage(imagePath, label);
                        Console.WriteLine($"Removed label '{label}' from '{imagePath}'");
                        return;

                    default:
                        Console.WriteLine("Invalid command. Use 'add' or 'remove'.");
                        return;
                }
            }
        }

        static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        static void RestartAsAdmin(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // User canceled UAC prompt
                MessageBox.Show("This operation requires administrator privileges.",
                               "Elevation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
