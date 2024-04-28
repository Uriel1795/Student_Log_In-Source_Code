using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace loginproto
{
    public class BooleanToIconConverter : IValueConverter
    {
        // Dictionary mapping extensions to icon paths
        private static readonly Dictionary<string, string> ExtensionToIconMap = new Dictionary<string, string>
        {
            { ".iqblocks", "pack://application:,,,/Resources/Images/iqblocks_file_icon.png" },
            // Add more extensions and icons as needed
        };
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fileName = value as string;
            var fileExtension = Path.GetExtension(fileName).ToLower();

            // Return the icon for the corresponding extension, or a default icon if not found
            if (ExtensionToIconMap.ContainsKey(fileExtension))
            {
                return new BitmapImage(new Uri(ExtensionToIconMap[fileExtension]));
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/default_file_icon.png"));  // Default icon
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();  // ConvertBack is not needed in this scenario
        }
    }
}
