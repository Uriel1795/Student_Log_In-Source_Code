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

namespace loginproto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private UpdateInfoModel? updateInfo;
        private bool shutDown = true;
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            CheckForUpdatesAsync();
            StartDropboxLoginProcess();
        }

        public UpdateInfoModel? _UpdateInfo
        { 
            get { return updateInfo; } 
            set {  updateInfo = value; } 
        }

        private async void StartDropboxLoginProcess()
        {
            string authUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={App.ClientID}&response_type=code&redirect_uri={App.RedirectUri}&token_access_type=offline";
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });

            string authCode = await PollForAuthCodeWithRetry();

            if (!string.IsNullOrEmpty(authCode))
            {
                var tokenResponse = await GetAccessToken(authCode);
                App.AccessToken = tokenResponse["access_token"].ToString();

                // Update connection status ellipse and label
                ConnectionStatusEllipse.Fill = Brushes.Green;
                ConnectionStatusLabel.Content = "Connected";
            }
            else
            {
                MessageBox.Show("Failed to retrieve authorization code.");
            }
        }

        private async Task<string> PollForAuthCodeWithRetry(int maxRetries = 3, int delay = 1000)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var authCode = await PollForAuthCode();
                    if (!string.IsNullOrEmpty(authCode))
                    {
                        return authCode;
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log detailed information about the request and response
                    Debug.WriteLine($"Request failed with status code {ex.StatusCode}: {ex.Message}");
                }
                await Task.Delay(delay);
                delay *= 2; // Exponential backoff
            }
            return null;
        }

        private async Task<string> PollForAuthCode()
        {
            var response = await client.GetAsync(App.CodePollUri);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    try
                    {
                        var jsonResponse = JObject.Parse(responseContent);
                        string authCode = jsonResponse["code"]?.ToString();
                        if (!string.IsNullOrEmpty(authCode))
                        {
                            return authCode;
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        // Handle the exception if JSON parsing fails
                        Debug.WriteLine($"JSON parsing error: {ex.Message}");
                    }
                }
            }
            else
            {
                Debug.WriteLine($"Failed to get authorization code. Status code: {response.StatusCode}");
            }
            return null;
        }

        private static async Task<JObject> GetAccessToken(string code)
        {
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", App.ClientID),
                new KeyValuePair<string, string>("client_secret", App.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", App.RedirectUri),
            });

            var response = await client.PostAsync("https://api.dropboxapi.com/oauth2/token", requestContent);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
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
            //If textbox is empty show message box
            if (string.IsNullOrEmpty(fTxtB.Text) || string.IsNullOrEmpty(lTxtB.Text))
            {
                MessageBox.Show("Please type your full name",
                    "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            var dbx = new DropboxClient(App.AccessToken);

            var accountInfo = await dbx.Users.GetCurrentAccountAsync();

            var db = dbx.WithPathRoot(new PathRoot.NamespaceId(accountInfo.RootInfo.RootNamespaceId));

            string searchPattern = fTxtB.Text.Trim().ToString() + "." + lTxtB.Text.Trim().ToString();

            //MessageBox.Show(searchPattern);
            //Assign variable to a new instance of StudentListWndow passing the textboxes with the first name and last name

            var metadata = await db.Files.GetMetadataAsync("/code/" + searchPattern);

            if (metadata.IsFolder)
            {
                var logout = new LogoutWindow();

                LocalStudentLookUp(searchPattern);

                Application.Current.Dispatcher.Invoke(Close);

                Process.Start("explorer.exe", @"R:\");
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

                    DriveSettings.MapNetworkDrive("R", mapPath);

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

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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