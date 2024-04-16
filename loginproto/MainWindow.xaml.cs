using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CheckForUpdatesAsync();
        }

        private async void CheckForUpdatesAsync()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var updateInfo = await GetLatestVersionInfoAsync();

            if (updateInfo?.Version != null)
            {
                var latestVersion = new Version(updateInfo.Version);


                if (latestVersion > currentVersion)
                {
                    var updateWindow =
                        new UpdateAvailableWindow(updateInfo.Version);

                    updateWindow.Show();

                    /*if (result == MessageBoxResult.Yes)
                    {
                        // Note: UseShellExecute must be true to open the URL in a browser.
                        Process.Start(new ProcessStartInfo(updateInfo.Url) { UseShellExecute = true });
                    }*/
                }
            }
        }

        private static async Task<UpdateInfo?> GetLatestVersionInfoAsync()
        {
            using (var client = new HttpClient())
            {
                // Example URL - replace with the actual raw GitHub URL to your update_info.json
                var json = await client.GetStringAsync("https://raw.githubusercontent.com/Uriel1795/Student-Log-In-Installer-file/main/update_info.json");

                return JsonSerializer.Deserialize<UpdateInfo>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        //Click on the login button
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //If textbox is empty show message box
            if (string.IsNullOrEmpty(fTxtB.Text) || string.IsNullOrEmpty(lTxtB.Text))
            {
                MessageBox.Show("Please type your full name", 
                    "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            //Assign variable to a new instance of popupwindow passing the textboxes with the first name and last name

            var popup = new PopupWindow(fTxtB.Text.Trim().ToString(), lTxtB.Text.Trim().ToString());

            if (popup.Valid == 1)
            {
                var logout = new LogoutWindow();

                Close();

                await Task.Delay(500);

                Process.Start("explorer.exe", @"R:\");
            }
        }
    }
}