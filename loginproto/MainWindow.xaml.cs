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
        }

        //Click on the login button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //If textbox is empty show message box
            if (fTxtB.Text.Length == 0 || lTxtB.Text.Length == 0)
            {
                MessageBox.Show("Please type your full name", "Incomplete Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Assign variable to a new instance of popupwindow passing the textboxes with the first name and last name
            var popup = new PopupWindow(fTxtB.Text.ToString(), lTxtB.Text.ToString());
        }
    }
}