using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Check if the registry key exists
            RegistryHelper.CreateRegistry(Properties.Settings.Default.registryPath);
        }

        //Click on the login button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //If textbox is empty show message box
            if (fTxtB.Text.Length == 0 || lTxtB.Text.Length == 0)
            {
                MessageBox.Show("Please type your full name",
                "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            //Assign variable to a new instance of popupwindow passing the textboxes with the first name and last name
            var popup = new PopupWindow(fTxtB.Text.ToString(), lTxtB.Text.ToString());

            if (popup.Valid == 1)
            {
                var logout = new LogoutWindow();

                Close();
            }
        }

        //Click on the Admin Mode button
        private void AdminMode_Click(object sender, RoutedEventArgs e)
        {
            InputWindow inputWindow = new InputWindow(InputType.Validation);

            if (inputWindow.ShowDialog() == true)
            {
                string userInput = inputWindow.UserInput;

                //Check if hash is the same as registry
                var adminResult =
                RegistryHelper.AdminValidation(Properties.Settings.Default.registryPath, "Admin", userInput);

                if (adminResult == 1)
                {
                    MessageBox.Show("Access Granted", "Verified", 
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    //Open Admin window
                    var adminWindow = new AdminModeWindow();
                    Close();
                    adminWindow.Show();
                }
                else
                {
                    MessageBox.Show("Please type the right password", "Wrong password",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Operation cancelled.", "Cancelled", MessageBoxButton.OK, 
                    MessageBoxImage.Exclamation);

            }
        }
    }
}