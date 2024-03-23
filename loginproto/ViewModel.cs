using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Names.Add(new Student { FirstName = firstName, LastName = lastName });
        }
    }
}
