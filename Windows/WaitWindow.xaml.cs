using System.Windows;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// WaitWindow.xaml の相互作用ロジック
    /// Please wait window
    /// </summary>
    public partial class WaitWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WaitWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 実行処理デリゲート
        /// </summary>
        /// <param name="win_"></param>
        /// <returns></returns>
        public delegate  Task TaskRun(WaitWindow? win_);

        /// <summary>
        /// 実行処理
        /// </summary>
        public TaskRun? Run = null;

        /// <summary>
        /// 表示メッセージクリア
        /// </summary>
        /// <param name="message_"></param>
        public void MessageClear(string message_ ="")
        {
            App.Current.Dispatcher.Invoke(() => {this._textBox.Text = message_; });
        }

        /// <summary>
        /// 表示メッセージ追加
        /// </summary>
        /// <param name="message_"></param>
        public void MessageAdd(string message_)
        {
            App.Current.Dispatcher.Invoke(() => { this._textBox.Text += message_; this._textBox.ScrollToEnd(); });
        }

        /// <summary>
        /// 表示後処理実行
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private async void _loaded(object sender_, RoutedEventArgs e_)
        {
            await Task.Run( () => {  Run?.Invoke(this); });
            this.Close();
        }
    }
}
