using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// SelectGroupWindows.xaml の相互作用ロジック
    /// グループ選択画面
    /// </summary>
    public partial class SelectGroupWindows : Window
    {
        private HashSet<string> _listSelect  = null;
        /// <summary>
        /// 選択されたグループリスト
        /// </summary>
        public string ListSelectGroup { get => string.Join("\n", FolderManager.Instance.ListUniqueGroup(_listSelect)); }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="groups"></param>
        public SelectGroupWindows( string groups)
        {
            InitializeComponent();

            _listSelect = new HashSet<string>( groups.Split("\n"));
            _setCheckedItem(_treeBoxGroup.ItemsSource as ObservableCollection<FolderGroupTreeView>);
        }

        /// <summary>
        /// チェックボックスの状態を設定
        /// </summary>
        /// <param name="list_"></param>
        private void _setCheckedItem(ObservableCollection<FolderGroupTreeView> list_)
        {
            if (list_ != null)
            {
                foreach (FolderGroupTreeView item in list_)
                {
                    item.Checked = _listSelect?.Contains(item.GroupName) ??false;
                    _setCheckedItem(item.ListChild);
                }
            }
        }

        /// <summary>
        /// チェックされたアイテムを取得
        /// </summary>
        /// <param name="list_"></param>
        private void _getCheckItem(ObservableCollection<FolderGroupTreeView> list_)
        {
            if (list_ != null)
            {
                foreach (FolderGroupTreeView item in list_)
                {
                    if (item.Checked == true)
                    {
                        _listSelect.Add(item.GroupName);
                    }
                    _getCheckItem(item.ListChild);
                }
            }
        }

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canselButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButtonClick(object sender, RoutedEventArgs e)
        {
            _listSelect.Clear();
            _getCheckItem(_treeBoxGroup.ItemsSource as ObservableCollection<FolderGroupTreeView>);

            DialogResult = true;
            Close();
        }
    }
}
