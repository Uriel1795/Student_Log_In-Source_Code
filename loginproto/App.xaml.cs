using System.Configuration;
using System.Data;
using System.Windows;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*private static Mutex mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string appName = "StudentLogInApp";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // If the mutex exists, then the application is already running
                MessageBox.Show("An instance of the application is already running.");

                Current.Shutdown();

                return;
            }
        }*/
    }
}
