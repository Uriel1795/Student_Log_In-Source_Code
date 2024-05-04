using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string dropboxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Robot Revolution Dropbox", "code");

            try
            {
                if (Directory.Exists(dropboxPath))
                {
                    //MessageBox.Show("You good homie");

                    if (DriveSettingsHelper.IsDriveMapped("R"))
                    {
                        DriveSettingsHelper.DisconnectNetworkDrive("R", true);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Student dropbox path is not found. Please ensure Dropbox is set up correctly.", "Dropbox not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown(1);
            }
        }
    }
}
