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
    public enum InputType
    {
        Validation,
        ResponseUri
    }

    public partial class InputWindow : Window
    {
        private readonly InputType inputType;
        private string registryPath = Properties.Settings.Default.registryPath;
        public string UserInput
        {
            get; private set; 
        }

        public Window MainWindowReference
        {
            get;
            private set;
        }

        public InputWindow(InputType inputType)
        {
            InitializeComponent();
            this.inputType = inputType;

            if(inputType == InputType.Validation)
            {
                Title = "Admin Password";
                windowLabel.Content = "Admin Password?";
            }

            if(inputType == InputType.ResponseUri) 
            {
                Title = "Uri";
                windowLabel.Content = "Dropbox Uri?";
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            UserInput = inputTextBox.Text;
            DialogResult = true;

            if (inputType == InputType.Validation)
            {
                //Add the salt 4610 to the input
                UserInput += "4610";

                //Hash the input + salt to sha3_512
                byte[] bytes = Encoding.UTF8.GetBytes(UserInput);

                var sha3 = new Sha3Digest(512);

                sha3.BlockUpdate(bytes, 0, bytes.Length);
                byte[] hashBytes = new byte[sha3.GetDigestSize()];
                sha3.DoFinal(hashBytes, 0);

                string hexString = BitConverter.ToString(hashBytes).Replace("-", "");

                UserInput = hexString;

            }
            if(inputType == InputType.ResponseUri) 
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
