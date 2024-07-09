using System.Timers;
using System.Windows;
using loginproto.Helpers;
using Timer = System.Timers.Timer;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for LogoutWindow.xaml
    /// </summary>
    public partial class LogoutWindow : Window
    {
        private Timer logoutTimer;
        private DateTime timerStartTime;
        private int initialTimerDurationInSeconds;
        private int remainingTimeInSeconds;


        public LogoutWindow()
        {
            InitializeComponent();
            timerStartTime = DateTime.Now;

            initialTimerDurationInSeconds = CurrentSessionTimer.IsSummerCamp(timerStartTime);

            // Start the logout timer
            StartLogoutTimer();
            UpdateCountdownText(); // Update the countdown text when the window is created

            Closing += LogoutWindow_Closing;

            //Show LogoutWindow

            Show();

            WindowState = WindowState.Minimized;
        }

        private void StartLogoutTimer()
        {
            logoutTimer = new Timer(1000); // Timer Ticks every second
            logoutTimer.Elapsed += Timer_Tick;
            logoutTimer.AutoReset = true;
            logoutTimer.Enabled = true;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - timerStartTime;

            remainingTimeInSeconds = initialTimerDurationInSeconds - (int)elapsed.TotalSeconds;

            if (remainingTimeInSeconds <= 0)
            {
                logoutTimer.Stop();
                DisconnectMappedDrive();
                Application.Current.Dispatcher.Invoke(ShutdownApplication);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(UpdateCountdownText);
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

            if(DriveSettings.IsDriveMapped("R") != true)
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                System.Media.SystemSounds.Question.Play();
            }
        }

        private void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        private void LogoutWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Handle logout button click event
            DisconnectMappedDrive();

            //Shutdown applicaton
            ShutdownApplication();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;
            Left = screenWidth - Width;
            Top = screenHeight - Height;
        }
    }
}
