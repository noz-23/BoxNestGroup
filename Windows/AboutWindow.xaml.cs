using System.Windows;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// AboutWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutWindow : System.Windows.Window
    {
        /// <summary>
        ///  About画面
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「OK」ボタン操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
