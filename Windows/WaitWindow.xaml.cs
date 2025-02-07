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

        public delegate  Task TaskRun(WaitWindow? win_);

        public TaskRun? Run = null;


        void MessageClear(string message_ ="")
        {
            App.Current.Dispatcher.Invoke(() => {this._textBox.Text = message_; });
        }
        void MessageAdd(string message_ = "")
        {
            App.Current.Dispatcher.Invoke(() => { this._textBox.Text += message_; });
        }


        private async void _loaded(object sender_, RoutedEventArgs e_)
        {
            if (Run != null)
            {
                await Task.Run(async () => { await Run.Invoke(this); });
            }
            this.Close();
        }
    }
}
