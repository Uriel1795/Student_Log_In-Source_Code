using System.Collections.ObjectModel;

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
            Names.Add(new Student { FirstName = firstName, LastName = lastName});
        }
    }
}
