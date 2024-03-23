using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using Path = System.IO.Path;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {

        private string dropboxPath = "";

        public PopupWindow(string firstName, string lastName)
        {
            InitializeComponent();
            ViewModel vM = new ViewModel();
            PopulateNamesList(firstName, lastName, vM);
            DataContext = vM;
        }      

        public string DropboxPath
        {
            get { return dropboxPath; }
            set { dropboxPath = value; }
        }

        public void PopulateNamesList(string firstName, string lastName, ViewModel viewModel)
        {
            List<string> foundNames = new List<string>();

            string searchPattern = $"{firstName}.{lastName}";

            DropboxPath = @"\\" + Environment.MachineName + "\\Users\\" + Environment.UserName + "\\Robot Revolution Dropbox\\";

            string[] dirs = Directory.GetDirectories(DropboxPath, $"{searchPattern}");

            string mDir = "";

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            if (dirs.Length > 0)
            {   
                try
                {
                    var mapPath = DropboxPath + searchPattern;

                    MessageBox.Show(DropboxPath);

                    NetworkDriveAPI.MapNetworkDrive("R",mapPath, Environment.UserName, "");
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Some Error " + ex);
                }
            }
            else
            {
                searchPattern = $"{firstName[0]}*.{lastName[0]}*";

                dirs = Directory.GetDirectories(DropboxPath, $"{searchPattern}");

                if (dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        mDir = dir.Trim().Replace(DropboxPath, "").Replace('.', ' ');

                        mDir = textInfo.ToTitleCase(mDir.ToLower());

                        string[] splitName = mDir.Split(' ');

                        viewModel.AddStudent(splitName[0], splitName[1]);
                    }

                    Show();
                }
            }

            //return false;
        }
        private void listBoxNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listBoxNames.SelectedItem != null) 
            {
                try
                {
                    Student selectedStudent = (Student)listBoxNames.SelectedItem;

                    // Pass selected student information to the existing MainWindow instance
                    ((MainWindow)Application.Current.MainWindow).fTxtB.Text = selectedStudent.FirstName;
                    ((MainWindow)Application.Current.MainWindow).lTxtB.Text = selectedStudent.LastName;

                    Close();

                } 
                catch (InvalidCastException ex)
                {
                    MessageBox.Show("Error: Unable to cast selected item to Student.\n" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
