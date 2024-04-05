using System.Collections.ObjectModel;
using System.ComponentModel;


namespace loginproto
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private string apiKey = string.Empty;
        private string userInput = string.Empty;

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        public string UserInput
        {
            get { return userInput; }
            set { userInput = value; }
        }

        public AdminViewModel()
        {
            Folders = new ObservableCollection<FolderViewModel>();
        }

        public ObservableCollection<FolderViewModel> Folders { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
