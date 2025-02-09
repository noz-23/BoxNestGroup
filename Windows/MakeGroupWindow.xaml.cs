using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// MakeGroupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MakeGroupWindow : Window
    {
        public MakeGroupWindow()
        {
            InitializeComponent();

            SettingManager.Instance.ListGroupDataGridView?.ToList().ForEach(view_ => ListGroup.Add(new MakeGroupView(view_)));
        }

        /// <summary>
        /// 作成グループリスト
        /// </summary>
        public ObservableCollection<MakeGroupView> ListGroup{get;private set;} =new ObservableCollection<MakeGroupView>();

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canselButtonClick(object sender_, RoutedEventArgs e_)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButtonClick(object sender_, RoutedEventArgs e_)
        {
            DialogResult = true;
            Close();
        }
    }
}
