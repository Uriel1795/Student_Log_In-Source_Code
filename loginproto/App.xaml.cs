using System;
using System.Diagnostics;
using System.Security.Principal;
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

            //Check if the app is running on admin mode
            /*if(!IsRunningAsAdmin())
            {
                //If not, restart as admin
                RestartAsAdmin();
                Shutdown();
                return;
            }*/

            if (DriveSettings.IsDriveMapped("R"))
            {
                DriveSettings.DisconnectNetworkDrive("R", true);
            }
        }

        //Function to check if app is running as admin
        private bool IsRunningAsAdmin()
        {
            var currentIdentify = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(currentIdentify);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        //Function to restart as admin
        private void RestartAsAdmin()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = System.Reflection.Assembly.GetExecutingAssembly().Location,
                UseShellExecute = true,
                Verb = "runas",
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Failed to restart with admin privileges: {ex.Message}");
            }
        }


    }
}
