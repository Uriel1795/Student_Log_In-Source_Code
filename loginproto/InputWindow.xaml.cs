using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Org.BouncyCastle.Crypto.Digests;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public string UserInput
        {
            get;
            private set;
        }

        public Window MainWindowReference
        {
            get;
            private set;
        }

        public InputWindow(Window mainWindow)
        {
            InitializeComponent();
            MainWindowReference = mainWindow;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            UserInput = inputTextBox.Text;
            DialogResult = true;

            //Add the salt 4610 to the input
            UserInput = UserInput + "4610";

            //Hash the input + salt to sha3_512
            byte[] bytes = Encoding.UTF8.GetBytes(UserInput);

            var sha3 = new Sha3Digest(512);

            sha3.BlockUpdate(bytes, 0, bytes.Length);
            byte[] hashBytes = new byte[sha3.GetDigestSize()];
            sha3.DoFinal(hashBytes, 0);

            string hexString = BitConverter.ToString(hashBytes).Replace("-", "");

            //Check if hash is the same as registry
            var adminResult = RegistryHelper.AdminValidation(Properties.Settings.Default.registryPath, "Admin", hexString);

            if (adminResult == 1)
            {
                MessageBox.Show("Verified");

                //Open Admin window
                var adminWindow = new AdminModeWindow();
                MainWindowReference.Close();
                adminWindow.Show();

            }
            else
            {
                MessageBox.Show("Please type the right password");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
