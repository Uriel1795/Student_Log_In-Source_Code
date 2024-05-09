using System.Windows.Controls;
using System.Windows;

namespace loginproto
{
    public class NameTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate FullNameTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // Both FirstName and LastName are present
            return FullNameTemplate;
        }
    }
}
