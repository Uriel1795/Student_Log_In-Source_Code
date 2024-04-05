using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Newtonsoft.Json;

namespace loginproto
{
    public class AuthenticationService
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        public AuthenticationService(string clientId, string clientSecret, string redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public void RequestAuthorization()
        {
            string clientId = RegistryHelper.GetRegistryValue("ApiKey");
            string redirectUri = Uri.EscapeDataString(RegistryHelper.GetRegistryValue("RedirectUri")); // Your actual redirect URI goes here
            string authUri = $"https://www.dropbox.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}";

            // Launches the system default web browser to the authorization URL
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = authUri,
                UseShellExecute = true,
            };

            Process.Start(psi);
        }


        public async Task<TokenResponse> ExchangeCodeForToken(string code)
        {
            using var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
            });

            var response = await client.PostAsync("https://api.dropbox.com/oauth2/token", requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);
                return tokenResponse;
            }
            else
            {
                throw new Exception($"Failed to exchange code for tokens: {responseString}");
            }
        }

    }

}
