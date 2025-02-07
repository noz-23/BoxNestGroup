using System.Windows;
using System.Reflection;

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

        public string ApplicationVersion 
        {
            get
            {
                var asmName =Assembly.GetExecutingAssembly().GetName();

                return $"{asmName.Name} Ver.{asmName?.Version?.ToString()??string.Empty} \nLisence: MIT license\n";
            }
        }
    }
}
