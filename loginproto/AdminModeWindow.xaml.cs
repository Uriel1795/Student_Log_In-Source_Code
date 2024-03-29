using System.Windows;
using Dropbox.Api;
using Dropbox.Api.Files;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for AdminModeWindow.xaml
    /// </summary>
    public partial class AdminModeWindow : Window
    {
        public AdminModeWindow()
        {
            InitializeComponent();
        }

        static async Task Main(string[] args)
        {
            // Initialize Dropbox app credentials
            var apiKey = "nws9r63uqopnz41";
            var apiSecret = "92rld2e09b22f0p";
            var redirectUri = "admin://oauth";

            var tokenResult = await DropboxOAuth2Helper.ProcessCodeFlowAsync(redirectUri, apiKey, apiSecret, redirectUri.ToString());

            var accessToken = tokenResult.AccessToken;

            using (var dropboxClient = new DropboxClient(accessToken))
            {
                var list = await ListFilesAsync(dropboxClient, "");

                foreach (var item in list)
                {
                    Console.WriteLine(item.Name);
                }
            }
        }

        static async Task<List<Metadata>> ListFilesAsync(DropboxClient client, string folderPath)
        {
            var list = new List<Metadata>();

            try
            {
                var listFolder = await client.Files.ListFolderAsync(folderPath);

                foreach (var item in listFolder.Entries)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (ApiException<ListFolderError> ex)
            {
                Console.WriteLine($"Error listing folder: {ex.Message}");
                return null;
            }
        }
    }
}
