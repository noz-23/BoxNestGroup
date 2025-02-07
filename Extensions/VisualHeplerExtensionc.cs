using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BoxNestGroup.Extensions
{
    public static class VisualHeplerExtensionc
    {
        public static T? GetParentOfType<T>( this DependencyObject element_) where T : DependencyObject
        {
            while(element_!=null)
            {
                if(element_ is T rtn)
                {
                    return rtn;
                }
                element_ = VisualTreeHelper.GetParent(element_);
            }

            return null;
        }
    }
}
