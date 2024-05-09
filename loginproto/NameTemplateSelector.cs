using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static loginproto.PopupWindow;
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
