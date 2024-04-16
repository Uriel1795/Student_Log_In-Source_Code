using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO.Compression;
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
        }

        public string UpdateVersion { get; set; }

        private async void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            string downloadUrl = "https://raw.githubusercontent.com/Uriel1795/Student-Log-In-Installer-file/main/v" + UpdateVersion;

            string tempPath = Path.Combine(Path.GetTempPath(), "setup.exe");

            MessageBox.Show(tempPath);

            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Download the file
                    var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

                    MessageBox.Show(response.ToString());

                    using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        // Read the response content and write it to file
                        await response.Content.CopyToAsync(fs);
                    }
                }

                // File download complete, proceed to extract and update
                string tempDirectory = Path.Combine(Path.GetTempPath(), "Student Log In");
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
                ZipFile.ExtractToDirectory(tempPath, tempDirectory);

                // Assuming the updater is a separate executable or script that you include in the ZIP
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    Arguments = applicationPath,
                    FileName = Path.Combine(tempDirectory, "setup.exe"),
                    UseShellExecute = true
                };
                Process.Start(startInfo);

                // Close the current application so the updater can replace files
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update: " + ex.Message);
            }
        }

        private void Later_Click(object sender, RoutedEventArgs e)
        {
            SaveCheckboxState();
            this.Close(); // Close the notification window
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
