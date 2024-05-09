using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;


namespace loginproto
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close the About window
        }

        // Event handler for hyperlink navigation
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); // Open email client
            e.Handled = true; // Mark the event as handled
        }
    }
}
