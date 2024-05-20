using Dropbox.Api;
using Dropbox.Api.Common;
using loginproto.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;

namespace loginproto.Helpers
{
    public static class DropboxHelper
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<DropboxClient> Connector()
        {
            var dbx = new DropboxClient(App.AccessToken);

            var accountInfo = await dbx.Users.GetCurrentAccountAsync();

            var db = dbx.WithPathRoot(new PathRoot.NamespaceId(accountInfo.RootInfo.RootNamespaceId));

            return db;
        }

        public static async Task<TokenResponseModel> GetAccessToken(string code)
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

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenResponseModel>(responseContent);
        }

        public static async Task<string> PollForAuthCode()
        {
            for (int i = 0; i < 30; i++)
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
                await Task.Delay(1000);
            }
            return null;
        }
    }
}
