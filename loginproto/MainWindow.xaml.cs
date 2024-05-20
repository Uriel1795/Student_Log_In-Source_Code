using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Text.Json;
using System.Windows.Input;
using loginproto.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Dropbox.Api;
using Dropbox.Api.Common;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Media;
using System;
using loginproto.Helpers;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private UpdateInfoModel? _UpdateInfo { get; set; }
        private bool shutDown = true;

        public MainWindow()
        {
            InitializeComponent();
            CheckForUpdatesAsync();
            StartDropboxLoginProcess();
        }

        private async void StartDropboxLoginProcess()
        {
            string authUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={App.ClientID}&response_type=code&redirect_uri={App.RedirectUri}";
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });

            string authCode = await DropboxHelper.PollForAuthCode();

            if (!string.IsNullOrEmpty(authCode))
            {
                var tokenResponse = await DropboxHelper.GetAccessToken(authCode);

                App.AccessToken = tokenResponse.AccessToken;

                // Update connection status ellipse and label
                ConnectionStatusEllipse.Fill = Brushes.Green;
                ConnectionStatusLabel.Content = "Dropbox Connected";
            }
            else
            {
                MessageBox.Show("Failed to retrieve authorization code.");
            }
        }
                    
        private async void CheckForUpdatesAsync()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

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

            cts.Cancel();
        }

        private static async Task<UpdateInfoModel?> GetLatestVersionInfoAsync()
        {
            using(var cts = new CancellationTokenSource())
            { 
                var token = cts.Token;

                using (var client = new HttpClient())
                {
                    string json = string.Empty;

                    try
                    {
                        // Raw GitHub URL to update_info.json
                        json = await client.GetStringAsync("https://raw.githubusercontent.com/Uriel1795/Student-Log-In-Installer-file/main/update_info.json");

                        return System.Text.Json.JsonSerializer.Deserialize<UpdateInfoModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("It seems you are not connected to the internet. Updates will be checked next time.", "No internet connection", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        return null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);

                        return null;
                    }
                }
            }
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            loginHandler();
        }

        private async void loginHandler()
        {
            try
            {
                //If textbox is empty show message box
                if (string.IsNullOrEmpty(fTxtB.Text) || string.IsNullOrEmpty(lTxtB.Text))
                {
                    MessageBox.Show("Please type your full name",
                        "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                var db = await DropboxHelper.Connector();

                string searchPattern = $"{fTxtB.Text.Trim()}.{lTxtB.Text.Trim()}";

                var metadata = await db.Files.GetMetadataAsync("/code/" + searchPattern);

                if (metadata.IsFolder)
                {
                    LocalStudentLookUp(searchPattern);

                    Process.Start("explorer.exe", @"R:\");

                    var logout = new LogoutWindow();

                    Application.Current.Dispatcher.Invoke(Close);
                }
            }
            catch(ApiException<Dropbox.Api.Files.GetMetadataError>)
            {
                MessageBox.Show("Student could not be found, make sure: your name is typed correctly or have an account.",
                    "Login Unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LocalStudentLookUp(string searchPattern)
        {
            // Dropbox path
            var dropboxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Robot Revolution Dropbox", "code");

            string[] dirs = Directory.GetDirectories(dropboxPath, $"{searchPattern}");

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            if (dirs.Length > 0)
            {
                try
                {
                    var mapPath = @"\\" + Environment.MachineName + "\\Users\\" +
                        Environment.UserName + "\\Robot Revolution Dropbox\\code\\" +
                        searchPattern;

                    DriveSettingsHelper.MapNetworkDrive("R", mapPath);

                    var result = MessageBox.Show("Log in successful", "Success", MessageBoxButton.OK,
                         MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Some Error: " + ex.Message, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Stop);
                    });
                }
            }
        }

        private async void updateTab_Click(object sender, RoutedEventArgs e)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

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

            cts.Cancel();
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                loginHandler();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(shutDown == true)
            Application.Current.Shutdown();
        }
    }
}