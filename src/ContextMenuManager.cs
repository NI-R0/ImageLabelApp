using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ImageLabelApp
{
    public static class ContextMenuManager
    {
        private static readonly string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static readonly string addDropdownPath = @"*\shell\Add Label";
        private static readonly string removeDropdownPath = @"*\shell\Remove Label";

        public static void InstallContextMenu()
        {
            //Add both dropdowns
            //Add edit options to both dropdowns
            //Add labels to each dropdown
            UninstallContextMenu();

            using (RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(addDropdownPath))
            {
                baseKey.SetValue("MUIVerb", "Add Label");
                baseKey.SetValue("SubCommands", "");
            }

            using (RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(removeDropdownPath))
            {
                baseKey.SetValue("MUIVerb", "Remove Label");
                baseKey.SetValue("SubCommands", "");
            }

            string commandEdit = $"\"{exePath}\" edit";
            AddCmdToMenu(addDropdownPath, commandEdit, "Edit Labels");
            AddCmdToMenu(removeDropdownPath, commandEdit, "Edit Labels");

            LabelDatabase.CreateNewLabel("Favourites");
            AddLabelEntries("Favourites");
        }

        public static void AddLabelEntries(string labelName)
        {
            //Add new label to both "Add" and "Remove"
            string commandAdd = $"\"{exePath}\" \"%1\" add " + labelName;
            AddCmdToMenu(addDropdownPath, commandAdd, labelName);

            string commandRemove = $"\"{exePath}\" \"%1\" remove " + labelName;
            AddCmdToMenu(removeDropdownPath, commandRemove, labelName);
        }

        private static void AddCmdToMenu(string menuKey, string cmd, string keyText)
        {
            using (RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey($@"{menuKey}\shell\{keyText}"))
            {
                baseKey.SetValue("MUIVerb", keyText);
                using (RegistryKey commandKey = baseKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", cmd);
                }
            }
        }

        public static void RemoveLabelEntries(string labelName)
        {
            //Remove label from both "Add" and "Remove"
            RemoveCmdFromMenu(addDropdownPath, labelName);
            RemoveCmdFromMenu(removeDropdownPath, labelName);
        }

        private static void RemoveCmdFromMenu(string menuKey, string keyText)
        {
            string labelKeyPath = menuKey + "\\shell\\" + keyText;
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(labelKeyPath, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing context menu entries: " + ex.Message);
            }
        }

        public static void UninstallContextMenu()
        {
            //Remove menu entries
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(addDropdownPath, false);
                Registry.ClassesRoot.DeleteSubKeyTree(removeDropdownPath, false);
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error removing context menu entries: " + ex.Message);
            }
        }
    }
}
