using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System;

namespace loginproto
{
    public class ViewModel
    {
        public ViewModel()
        {
            Names = new ObservableCollection<Student>();
        }
        public ObservableCollection<Student> Names { get; }

        public void AddStudent(string firstName, string lastName)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Names.Add(new Student { FirstName = firstName, LastName = lastName });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding student: " + ex.Message);

                }
            });
        }
    }
}
