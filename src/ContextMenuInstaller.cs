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
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string keyPath = @"*\shell\Add Label";

            using (RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(keyPath))
            {
                baseKey.SetValue("MUIVerb", "Add Label");
                baseKey.SetValue("SubCommands", "");
            }

            using (RegistryKey favouritesKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Favourites"))
            {
                favouritesKey.SetValue("MUIVerb", "Favourites");
                using (RegistryKey commandKey = favouritesKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" \"%1\" Favourites");
                }
            }

            using (RegistryKey meKey = Registry.ClassesRoot.CreateSubKey($@"{keyPath}\shell\Me"))
            {
                meKey.SetValue("MUIVerb", "Me");
                using (RegistryKey commandKey = meKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" \"%1\" Me");
                }
            }
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
    }
}
