using System.Windows;
using System.Windows.Media;

namespace BoxNestGroup.Extensions
{
    /// <summary>
    /// ドラッグ&ドロップ で利用(表示位置からからアイテムの選択)
    /// </summary>
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
