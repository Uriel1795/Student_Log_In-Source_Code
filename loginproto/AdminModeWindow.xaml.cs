using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Dropbox.Api.Team.DesktopPlatform;
using static Dropbox.Api.TeamLog.EventCategory;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace loginproto
{
    public partial class AdminModeWindow : Window
    {
        private AuthenticationService _authService;

        public ICommand RenameCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public AdminModeWindow()
        {
            InitializeComponent();

            // Initialize the authentication service with your Dropbox app details
            _authService = new AuthenticationService(
            RegistryHelper.GetRegistryValue("ApiKey"), RegistryHelper.GetRegistryValue("ApiSecret"),
            RegistryHelper.GetRegistryValue("RedirectUri"));

            RenameCommand = new RelayCommand(param => OnRename(param));
            DeleteCommand = new RelayCommand(param => OnDelete(param));

            DataContext = this;

        }

        private async void AdminModeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initiate the authorization request to Dropbox
                _authService.RequestAuthorization();

                // TODO: Implement the logic to capture the authorization code from the user or redirect
                string authorizationCode = PromptUserForAuthorizationCode(); // This method needs to be implemented based on your app's flow

                // Exchange the authorization code for tokens
                var tokenResponse = await _authService.ExchangeCodeForToken(authorizationCode);

                // Use the access token to fetch and display folders
                var folders = await FetchFoldersAsync(tokenResponse.AccessToken);
                FoldersItemsControl.ItemsSource = folders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // This method simulates prompting the user to enter the authorization code manually.
        // Replace this with your actual implementation.
        private string PromptUserForAuthorizationCode()
        {
            // You might open a new window or dialog here asking the user to paste the authorization code.

            InputWindow inputWindow = new InputWindow(InputType.ResponseUri);

            string authorizationCode = string.Empty;

            if(inputWindow.ShowDialog() == true)
            {
                authorizationCode = inputWindow.UserInput;
            }

            
            return authorizationCode;
        }

        private async Task<List<FolderViewModel>> FetchFoldersAsync(string accessToken)
        {
            var items = new List<FolderViewModel>();

            using (var dbx = new DropboxClient(accessToken))
            {
                var list = await dbx.Files.ListFolderAsync("");

                MessageBox.Show(list.Entries.ToString());

                foreach (var item in list.Entries)
                {
                    BitmapImage thumbnail = null;

                    if (!item.IsFolder)
                    {
                        thumbnail = await FetchThumbnailAsync(dbx, item.PathDisplay);
                    }

                    items.Add(new FolderViewModel
                    {
                        Name = item.Name,
                        Thumbnail = 
                        thumbnail ?? new BitmapImage(new Uri("pack://application:,,,/Resources/Images/folder-icon.png", UriKind.RelativeOrAbsolute)),
                    });
                }
            }
            return items;
        }

        public async Task<BitmapImage> FetchThumbnailAsync(DropboxClient dbx, string filePath)
        {
            using (var response = await dbx.Files.GetThumbnailAsync(filePath))
            {
                if (response != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await response.GetContentAsStreamAsync().Result.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = memoryStream;
                        bitmap.EndInit();
                        bitmap.Freeze(); // Important for use in UI thread

                        return bitmap;
                    }
                }
            }
            return null; // Return null or a default thumbnail if fetching fails
        }

        private void OnRename(object param)
        {
            var item = param as FolderViewModel;
            if (item != null)
            {
                // Implement your renaming logic here
                MessageBox.Show($"Rename: {item.Name}");
            }
        }

        private void OnDelete(object param)
        {
            var item = param as FolderViewModel;

            if (item != null)
            {
                // Implement your deletion logic here
                MessageBox.Show($"Delete: {item.Name}");
            }
        }

    }
}
