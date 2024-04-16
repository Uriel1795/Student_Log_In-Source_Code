using System.Diagnostics;
using System.Windows;

namespace loginproto
{
    public class MonitorNotAllowedPrograms
    {
        public static void StartMonitoring()
        {
            Task.Run(async () => await MonitorPrograms());
        }

        private static async Task MonitorPrograms()
        {
            string[] notAllowedPrograms = { "chrome", "firefox", "iexplore", "edge" }; // Add more as needed

            while (true) // Consider implementing a more graceful exit condition
            {
                foreach (string program in notAllowedPrograms)
                {
                    Process[] processes = Process.GetProcessesByName(program);

                    foreach (Process process in processes)
                    {
                        try
                        {
                            process.Kill(); // Forcefully closes the process

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error closing {program}: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                }

                /*MessageBox.Show($"{DateTime.Now}: Closed {programo}.",
                "Program not allowed", MessageBoxButton.OK, 
                MessageBoxImage.Stop);*/

                await Task.Delay(3000); // Check every 3 seconds, adjust as needed
            }
        }
    }
}