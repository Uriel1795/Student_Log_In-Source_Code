using System.Collections.ObjectModel;
using System.Windows;

namespace loginproto.Models
{
    public class ViewModel
    {
        public ViewModel()
        {
            Names = new ObservableCollection<StudentModel>();
        }
        public ObservableCollection<StudentModel> Names { get; }

        public void AddStudent(string firstName, string lastName)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Names.Add(new StudentModel { FirstName = firstName, LastName = lastName });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding student: " + ex.Message);

                }
            });
        }
    }
}
