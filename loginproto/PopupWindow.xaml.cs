using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private int valid = 0;

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

        public int Valid
        {
            get { return valid; }
            set { valid = value; }
        }

        public void PopulateNamesList(string firstName, string lastName, ViewModel viewModel)
        {
            List<string> foundNames = new List<string>();

            string searchPattern = $"{firstName.ToLower()}.{lastName.ToLower()}";

            DropboxPath = @Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Robot Revolution Dropbox\\students\\summit\\";

            string[] dirs = Directory.GetDirectories(DropboxPath, $"{searchPattern}");

            string mDir = "";

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            if (dirs.Length > 0)
            {   
                try
                {
                    var mapPath = @"\\" + Environment.MachineName + "\\Users\\" + Environment.UserName + 
                        "\\Robot Revolution Dropbox\\students\\summit\\" + searchPattern;

                    DriveSettings.MapNetworkDrive("R", mapPath);

                    MessageBoxResult result = MessageBox.Show("Login successful.", "Success", MessageBoxButton.OK);

                    if (result == MessageBoxResult.OK)
                    {
                        Valid = 1;
                        Close();
                    }
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

                        string[] splitName = textInfo.ToTitleCase(mDir.ToLower()).Split(' ');

                        viewModel.AddStudent(splitName[0], splitName[1]);
                    }

                    Show();
                }
                else
                {
                    MessageBox.Show("Student could not be found.");
                    Close();
                }
            }
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
