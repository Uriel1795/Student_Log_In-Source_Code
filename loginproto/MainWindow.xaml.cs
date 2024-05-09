using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Text.Json;
using System.Windows.Input;
using loginproto.Models;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private UpdateInfoModel? updateInfo;
        public MainWindow()
        {
            InitializeComponent();

            CheckForUpdatesAsync();
        }

        public UpdateInfoModel? _UpdateInfo
        { 
            get { return updateInfo; } 
            set {  updateInfo = value; } 
        }
        private async void CheckForUpdatesAsync()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Title = "Log In - " + currentVersion;
            _UpdateInfo = await GetLatestVersionInfoAsync();

            if (_UpdateInfo?.Version != null)
            {
                var latestVersion = new Version(_UpdateInfo.Version);


                if (latestVersion > currentVersion)
                {
                    var updateWindow =
                        new UpdateAvailableWindow(_UpdateInfo.Version);

                    updateWindow.Show();
                }
            }
        }

        private static async Task<UpdateInfoModel?> GetLatestVersionInfoAsync()
        {
            using (var client = new HttpClient())
            {
                string json = string.Empty;

                try
                {
                    // Raw GitHub URL to update_info.json
                    json = await client.GetStringAsync("https://raw.githubusercontent.com/Uriel1795/Student-Log-In-Installer-file/main/update_info.json");

                    return JsonSerializer.Deserialize<UpdateInfoModel>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("It seems you are not connected to the internet. Updates will be checked next time.", "No internet connection", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    return null;
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Error: " + ex.Message);

                    return null;
                }
            }
        }

        //Click on the login button
        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            loginHandler();
        }

        private void loginHandler()
        {
            //If textbox is empty show message box
            if (string.IsNullOrEmpty(fTxtB.Text) || string.IsNullOrEmpty(lTxtB.Text))
            {
                MessageBox.Show("Please type your full name",
                    "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            //Assign variable to a new instance of StudentListWndow passing the textboxes with the first name and last name
            var popup = new StudentListWndow(fTxtB.Text.Trim().ToString(), lTxtB.Text.Trim().ToString());

            if (popup.Found == true)
            {
                var logout = new LogoutWindow();

                Application.Current.Dispatcher.Invoke(Close);

                Process.Start("explorer.exe", @"R:\");
            }
        }
        private async void updateTab_Click(object sender, RoutedEventArgs e)
        {
            if (_UpdateInfo?.Version != null)
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var latestVersion = new Version(_UpdateInfo.Version);
                var updateWindow = new UpdateAvailableWindow(_UpdateInfo.Version);

                if (latestVersion > currentVersion)
                {
                    // Start the update process asynchronously
                    await updateWindow.StartUpdateProcess();

                }
                else
                {
                    MessageBox.Show("No updates available at the moment", "No updates available", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                loginHandler();
        }
    }
}