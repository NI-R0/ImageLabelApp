using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ImageLabelApp
{
    public static class ContextMenuInstaller
    {
        //public static void InstallContextMenu()
        //{
        //    string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //    string commandFavourites = $"\"{exePath}\" \"%1\" Favourites";
        //    string commandMe = $"\"{exePath}\" \"%1\" Me";

        //    string keyPath = @"*\shell\Add Label";
        //    RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(keyPath);
        //    baseKey.SetValue("MUIVerb", "Add Label");
        //    baseKey.SetValue("SubCommands", "");

        //    var favouritesKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Favourites\command");
        //    favouritesKey.SetValue("", commandFavourites);

        //    var meKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Me\command");
        //    meKey.SetValue("", commandMe);
        //}

        public static void InstallContextMenu()
        {
            UninstallContextMenu();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string keyPath = @"*\shell\Add Label";

            string commandAddFavourites = $"\"{exePath}\" \"%1\" add Favourites";
            string commandAddMe = $"\"{exePath}\" \"%1\" add Me";
            string commandRemoveFavourites = $"\"{exePath}\" \"%1\" remove Favourites";
            string commandRemoveMe = $"\"{exePath}\" \"%1\" remove Me";
            
            string commandEditLabels = $"\"{exePath}\" \"%1\" edit";
            //key.CreateSubKey(@"*\shell\Add Label\command").SetValue("", commandAddLabel);

            using (RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(keyPath))
            {
                baseKey.SetValue("MUIVerb", "Add Label");
                baseKey.SetValue("SubCommands", "");
            }

            using (RegistryKey editKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\editLabels"))
            {
                editKey.SetValue("MUIVerb", "Edit Labels");
                using (RegistryKey commandKey = editKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", commandEditLabels);
                }
            }

            using (RegistryKey favouritesKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Favourites"))
            {
                favouritesKey.SetValue("MUIVerb", "Favourites");
                using (RegistryKey commandKey = favouritesKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", commandAddFavourites);
                }
            }

            using (RegistryKey meKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Me"))
            {
                meKey.SetValue("MUIVerb", "Me");
                using (RegistryKey commandKey = meKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", commandAddMe);
                }
            }

            string removeKeyPath = @"*\shell\Remove Label";
            RegistryKey removeBaseKey = Registry.ClassesRoot.CreateSubKey(removeKeyPath);
            removeBaseKey.SetValue("MUIVerb", "Remove Label");
            removeBaseKey.SetValue("SubCommands", "");

            // Sub-items
            var removeFavouritesKey = Registry.ClassesRoot.CreateSubKey($@"{removeKeyPath}\shell\Favourites\command");
            removeFavouritesKey.SetValue("", commandRemoveFavourites);

            var removeMeKey = Registry.ClassesRoot.CreateSubKey($@"{removeKeyPath}\shell\Me\command");
            removeMeKey.SetValue("", commandRemoveMe);
        }

        public static void InstallRemoveContextMenus()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            foreach (var label in new[] { "Favourites", "Me" })
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($@"*\shell\RemoveLabel_{label}"))
                {
                    key.SetValue(null, $"Remove from {label}");
                    key.SetValue("Icon", $"\"{exePath}\"");

                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        commandKey.SetValue(null, $"\"{exePath}\" --remove \"%1\" {label}");
                    }
                }
            }
        }

        public static void AddLabelEntry(string labelName)
        {
            //Add new label to both "Add" and "Remove"
        }

        public static void RemoveLabelEntry(string labelName)
        {
            //Remove label from both "Add" and "Remove"
        }

        public static void UninstallContextMenu()
        {
            string keyPath = @"*\shell\Add Label";
            string removeFavouriteKey = @"*\shell\RemoveLabel_Favourites";
            string removeMeKey = @"*\shell\RemoveLabel_Me";
            string removeLabelKey = @"*\shell\Remove Label";

            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(keyPath, false);
                Registry.ClassesRoot.DeleteSubKeyTree(removeFavouriteKey, false);
                Registry.ClassesRoot.DeleteSubKeyTree(removeMeKey, false);
                Registry.ClassesRoot.DeleteSubKeyTree(removeLabelKey, false);
                Console.WriteLine("Context menu entries removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing context menu entries: " + ex.Message);
            }
        }
    }
}
