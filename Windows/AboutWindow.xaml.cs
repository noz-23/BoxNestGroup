using System.Reflection;
using System.Windows;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// AboutWindow.xaml の相互作用ロジック
    ///  About画面
    /// </summary>
    public partial class AboutWindow : System.Windows.Window
    {
        /// <summary>
        /// コンストラクタ
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

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
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
