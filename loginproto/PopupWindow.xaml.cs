using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Input;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private string dropboxPath = string.Empty;
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
            string searchPattern = $"{firstName.ToLower()}.{lastName.ToLower()}";

            MessageBox.Show(searchPattern);

            // Define the Dropbox path
            DropboxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Robot Revolution Dropbox", "code");

            string[] dirs = Directory.GetDirectories(DropboxPath, $"{searchPattern}");

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            if (dirs.Length > 0)
            {   
                try
                {
                    var mapPath = @"\\" + Environment.MachineName + "\\Users\\" + 
                        Environment.UserName + "\\Robot Revolution Dropbox\\code\\" + 
                        searchPattern;

                    DriveSettings.MapNetworkDrive("R", mapPath);

                    MessageBoxResult result = MessageBox.Show("Login successful.", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (result == MessageBoxResult.OK)
                    {
                        Valid = 1;

                        Close();
                    }
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Some Error " + ex, "Error", MessageBoxButton.OK, 
                        MessageBoxImage.Stop);
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
                        string mDir = dir.Replace(DropboxPath, "").Replace('.', ' ').Replace('\\', ' ').Trim();

                        //MessageBox.Show(mDir);

                        string[] splitName = textInfo.ToTitleCase(mDir.ToLower()).Split(' ');

                        //if (splitName.Length >= 3)
                            viewModel.AddStudent(splitName[0], splitName[1]);
                        /*else
                            viewModel.AddStudent(splitName[0].Replace('\\', ' ').Trim(), splitName[1], null);*/
                    }

                    Show();
                }
                else
                {
                    MessageBox.Show("Student could not be found.", 
                        "Not found", MessageBoxButton.OK, MessageBoxImage.Warning);

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
                    ((MainWindow)Application.Current.MainWindow).fTxtB.Text = 
                        selectedStudent.FirstName;

                    ((MainWindow)Application.Current.MainWindow).lTxtB.Text =
                        selectedStudent.LastName;

                    Close();

                } 
                catch (InvalidCastException ex)
                {
                    MessageBox.Show("Error: Unable to cast selected item to Student.\n" + 
                        ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, 
                        MessageBoxImage.Stop);
                }
            }
        }
    }
}
