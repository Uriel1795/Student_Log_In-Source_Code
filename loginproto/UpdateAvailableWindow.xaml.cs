using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Net.Http;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for UpdateAvailableWindow.xaml
    /// </summary>
  
    public partial class UpdateAvailableWindow : Window
    {

        public UpdateAvailableWindow(string updateVersion)
        {
            InitializeComponent();

            UpdateVersion = updateVersion;

            Topmost = true;
        }

        public string UpdateVersion { get; set; }

        public async void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            await StartUpdateProcess();
        }

        public async Task StartUpdateProcess()
        {
            using (var cts = new CancellationTokenSource())
            {
                var token = cts.Token;
                // URL to download the executable
                string downloadUrl = "https://raw.githubusercontent.com/Uriel1795/Student-Log-In-Installer-file/main/v" + UpdateVersion + "/Student Log In.msi";

                // Path to store the downloaded executable
                string tempPath = Path.Combine(Path.GetTempPath(), "Student Log In.msi");

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Download the executable file
                        var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Failed to download update. Please try again later.", "Download Error", MessageBoxButton.OK, MessageBoxImage.Error);

                            return;
                        }

                        using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                        {
                            await response.Content.CopyToAsync(fs); // Write the response content to the file
                        }
                    }

                    // Start the downloaded executable
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = tempPath, // Path to the downloaded executable
                        UseShellExecute = true // Ensures the file is executed properly
                    };

                    Process.Start(startInfo); // Start the process

                    // Close the current application to allow the updater to run
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Shutdown();

                    });   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to update: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Later_Click(object sender, RoutedEventArgs e)
        {
            SaveCheckboxState();
            Close(); // Close the notification window
        }

        private void SaveCheckboxState()
        {
            CheckBox doNotShowAgain = (CheckBox)FindName("DoNotShowAgainCheckbox");

            if (doNotShowAgain.IsChecked == true)
            {
                // Save this setting to your configuration file or settings
                Properties.Settings.Default.DoNotShowAgain = true;
                Properties.Settings.Default.Save();
            }
        }
    }
}
