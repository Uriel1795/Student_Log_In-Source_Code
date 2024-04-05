using System.ComponentModel;
using System.Windows.Media.Imaging;

public class FolderViewModel : INotifyPropertyChanged
{
    private string _name;
    private BitmapImage _thumbnail;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
    public BitmapImage Thumbnail
    {
        get => _thumbnail;
        set
        {
            _thumbnail = value;
            OnPropertyChanged(nameof(Thumbnail));
        }
    }

    // The PropertyChanged event and OnPropertyChanged method implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
