using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for LogoutWindow.xaml
    /// </summary>
    public partial class LogoutWindow : Window
    {
        private Timer logoutTimer;
        private int remainingTimeInSeconds;

        public LogoutWindow()
        {
            InitializeComponent();

            remainingTimeInSeconds = 105 * 60;

            // Start the logout timer
            StartLogoutTimer();

            UpdateCountdownText(); // Update the countdown text when the window is created

            Closing += LogoutWindow_Closing;

            //Show LogoutWindow
            Show();
        }

        private void StartLogoutTimer()
        {
            logoutTimer = new System.Timers.Timer(1000); // Timer Ticks every second
            logoutTimer.Elapsed += Timer_Tick;
            logoutTimer.AutoReset = true;
            logoutTimer.Enabled = true;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            remainingTimeInSeconds--;

            if (remainingTimeInSeconds <= 0)
            {
                logoutTimer.Stop();

                DisconnectMappedDrive();

                Dispatcher.Invoke(() =>
                {
                    ShutdownApplication();
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateCountdownText();
                });
            }
        }

        private void UpdateCountdownText()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTimeInSeconds);
            countdownText.Text = timeSpan.ToString(@"hh\:mm\:ss");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            logoutTimer.Stop();

            // Handle logout button click event
            DisconnectMappedDrive();

            //Shutdown applicaton
            ShutdownApplication();
        }

        private void DisconnectMappedDrive()
        {
            DriveSettings.DisconnectNetworkDrive("R", true);
        }

        private void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

         private void LogoutWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Handle logout button click event
            DisconnectMappedDrive();

            MessageBox.Show("You have been logged out. Please allow a few seconds for the drive to be disconnected.");

            //Shutdown applicaton
            ShutdownApplication();
        }
    }
}
