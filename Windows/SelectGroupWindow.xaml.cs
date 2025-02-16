using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// SelectGroupWindows.xaml の相互作用ロジック
    /// グループ選択画面
    /// </summary>
    public partial class SelectGroupWindows : Window
    {
        private readonly HashSet<string> _listSelect  = new HashSet<string>();
        /// <summary>
        /// 選択されたグループリスト
        /// </summary>
        public ObservableCollection<string> ListSelectGroup { get=> new ObservableCollection<string>( SettingManager.Instance.ListXmlGroupTreeView.ListUniqueGroup(_listSelect)); }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="groups"></param>
        public SelectGroupWindows(IList<string> listGroup)
        {
            InitializeComponent();

            var item =_treeBoxGroup.ItemContainerGenerator;

            _listSelect.UnionWith(listGroup);
            _setCheckedItem(_treeBoxGroup.ItemsSource as XmlGroupTreeModel);
        }

        /// <summary>
        /// チェックボックスの状態を設定
        /// </summary>
        /// <param name="list_"></param>
        private void _setCheckedItem(XmlGroupTreeModel? list_)
        {
            list_?.ToList().ForEach(item =>
            {
                item.Checked = _listSelect?.Contains(item.GroupName) ?? false;
                _setCheckedItem(item.ListChild);
            });
        }

        /// <summary>
        /// チェックされたアイテムを取得
        /// </summary>
        /// <param name="list_"></param>
        private void _getCheckItem(XmlGroupTreeModel? list_)
        {
            list_?.ToList().ForEach(item =>
            {
                if (item.Checked == true)
                {
                    _listSelect.Add(item.GroupName);
                }
                _getCheckItem(item.ListChild);
            });
        }

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _canselButtonClick(object sender_, RoutedEventArgs e_)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _okButtonClick(object sender_, RoutedEventArgs e_)
        {
            _listSelect.Clear();
            _getCheckItem(_treeBoxGroup.ItemsSource as XmlGroupTreeModel);

            DialogResult = true;
            Close();
        }
    }
}
