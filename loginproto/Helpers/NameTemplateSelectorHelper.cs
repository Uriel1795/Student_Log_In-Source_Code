using System.Windows.Controls;
using System.Windows;

namespace loginproto
{
    public class NameTemplateSelectorHelper : DataTemplateSelector
    {
        public required DataTemplate FullNameTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return FullNameTemplate;
        }
    }
}
