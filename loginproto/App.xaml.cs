using System.Configuration;
using System.Data;
using System.Windows;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        //private static Mutex mutex = null;

        public static string? AccessToken
        {
            get { return (string?)Current.Properties["AccessToken"]; }
            set { Current.Properties["AccessToken"] = value; }
        }

        public static string? ClientID
        {
            get { return (string?)Current.Properties["ClientID"]; }
            set { Current.Properties["ClientID"] = value; }
        }

        public static string? ClientSecret
        {
            get { return (string?)Current.Properties["ClientSecret"]; }
            set { Current.Properties["ClientSecret"] = value; }
        }

        public static string? RedirectUri
        {
            get { return (string?)Current.Properties["RedirectUri"]; }
            set { Current.Properties["RedirectUri"] = value; }
        }

        public static string? CodePollUri
        {
            get { return (string?)Current.Properties["CodePollUri"]; }
            set { Current.Properties["CodePollUri"] = value; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (DriveSettingsHelper.IsDriveMapped("R"))
            {
                DriveSettingsHelper.DisconnectNetworkDrive("R", true);
            }

            AccessToken = "";
            ClientID = "l52k5711oq6mdqp";
            ClientSecret = "y8q41ic4wy8qtpi";
            RedirectUri = "https://uriel1795.github.io";
            CodePollUri = "https://dropbox-oauth-server-5d0b4b7ded68.herokuapp.com/get_code";
        }
    }
}
