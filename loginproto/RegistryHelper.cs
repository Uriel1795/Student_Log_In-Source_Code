using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace loginproto
{
    public class RegistryHelper
    {
        public static void CreateRegistry(string registryPath)
        {
            string endPath = "\\Robot Revolution Dropbox\\students\\summit\\";

            try
            {
                // Use the registryPath property to get the registry path
                RegistryKey parentKey = Registry.CurrentUser.OpenSubKey(registryPath, true);
                
                // If the parent key doesn't exist, create it
                if (parentKey == null)
                {
                    parentKey = Registry.CurrentUser.CreateSubKey(registryPath);
                }

                parentKey.SetValue("DropboxPath",
                    @Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + 
                    endPath);

                parentKey.SetValue("MapPath",
                    @"\\" + Environment.MachineName + "\\Users\\" + Environment.UserName + 
                    endPath);

                // Check if the key already exists

                if(!parentKey.GetValueNames().Contains("Admin"))
                {
                    parentKey.SetValue("Admin", "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating Admin registry key: " + ex.Message);
            }
        }

        public static string GetRegistryValue(string path, string registryName)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        string name = key.GetValue(registryName).ToString();

                        return name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return string.Empty;
        }

        public static int AdminValidation(string path, string registryName, string hexValue)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        var value = key.GetValue(registryName);

                        if (value.Equals(hexValue))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        CreateRegistry(registryName);
                        AdminValidation(path, registryName, hexValue);   
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return 0;
        }
    }
}
