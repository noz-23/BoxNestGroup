using System.Windows;
using System.Windows.Media;

namespace BoxNestGroup.Extensions
{
    /// <summary>
    /// ドラッグ&ドロップ で利用(表示位置からからアイテムの選択)
    /// </summary>
    public static class VisualHeplerExtensionc
    {
        public static T? GetParentOfType<T>( this DependencyObject src_) where T : DependencyObject
        {
            while(src_ != null)
            {
                if(src_ is T rtn)
                {
                    return rtn;
                }
                src_ = VisualTreeHelper.GetParent(src_);
            }

            return null;
        }
    }
}
