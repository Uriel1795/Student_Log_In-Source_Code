using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using loginproto.Models;

namespace loginproto
{
    /// <summary>
    /// Interaction logic for StudentListWindow.xaml
    /// </summary>
    public partial class StudentListWindow : Window
    {
        private string dropboxPath = string.Empty;
        private bool found = false; 

        public StudentListWindow(string firstName, string lastName)
        {
            InitializeComponent();
            ViewModel vM = new ViewModel();

            Task populateNamesTask = PopulateNamesListAsync(firstName, lastName, vM);

            DataContext = vM;
        }      

        public string DropboxPath
        {
            get { return dropboxPath; }
            set { dropboxPath = value; }
        }

        public bool Found
        {
            get { return found; }
            set { found = value; }
        }

        public async Task PopulateNamesListAsync(string firstName, string lastName, ViewModel viewModel)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            string searchPattern = $"{firstName.ToLower()}.{lastName.ToLower()}";

            // Dropbox path
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

                   var result = MessageBox.Show("Log in successful", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    if(result == MessageBoxResult.OK)
                    {
                        Found = true;
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Some Error: " + ex.Message, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Stop);
                    });
                }
            }
            else
            {
                bool dataLoaded = await Task.Run(() =>
                {

                    // Show loading text and hide list box on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        loadingText.Visibility = Visibility.Visible;
                        listBoxNames.Visibility = Visibility.Hidden;

                        Show();
                    });

                    searchPattern = $"{firstName[0]}*.{lastName[0]}*";

                    dirs = Directory.GetDirectories(DropboxPath, $"{searchPattern}");

                    if (dirs.Length > 0)
                    {
                        foreach (var dir in dirs)
                        {
                            string mDir = dir.Replace(DropboxPath, "").Replace('.', ' ').Replace('\\', ' ').Trim();

                            string[] splitName = textInfo.ToTitleCase(mDir.ToLower()).Split(' ');

                            viewModel.AddStudent(splitName[0], splitName[1]); // Populate the view model
                        }

                        Found = true; // Data was found
                    }
                    return Found;

                },token);

                // Perform UI updates based on whether data was found
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (dataLoaded)
                    {
                        listBoxNames.Visibility = Visibility.Visible;
                        loadingText.Visibility = Visibility.Collapsed;

                        Show(); // Ensure this is on the UI thread
                    }
                    else
                    {
                        MessageBox.Show("Student could not be found.",
                            "Not found", MessageBoxButton.OK, MessageBoxImage.Warning);

                        Close();
                    }
                });

            }

            // Cancel the token to prevent further processing
            cts.Cancel();
        }


        private void listBoxNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listBoxNames.SelectedItem != null) 
            {
                try
                {
                    StudentModel selectedStudent = (StudentModel)listBoxNames.SelectedItem;

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
