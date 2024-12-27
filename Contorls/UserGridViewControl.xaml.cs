using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// UserGridViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class UserGridViewControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserGridViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「追加/更新」ボタン操作
        ///  About画面の表示
        /// </summary>
        private async void _buttonMakeAndRenewUserGroupClick(object sender, RoutedEventArgs e)
        {
            // 更新は削除と追加で行う
            var delList = new List<UserDataGridView>();
            var addList = new List<UserDataGridView>();
            foreach (var user in SettingManager.Instance.ListUserDataGridView)
            {
                if (BoxManager.Instance.IsOnlne == false)
                {
                    continue;
                }

                //if (string.IsNullOrEmpty(user.UserId) == true)
                if(user.StatudData == UserDataGridView.Status.NEW)
                {
                    // 新規
                    var boxUser = await BoxManager.Instance.CreateUser(user.UserName, user.UserLogin);
                    if (boxUser != null)
                    {
                        delList.Add(user);
                        addList.Add(new UserDataGridView(boxUser));

                        await BoxManager.Instance.AddGroupUserFromName(boxUser, user.ListModAllGroup.Split('\n'));
                    }
                    continue;
                }
                if (user.StatudData == UserDataGridView.Status.MOD)
                {
                    var boxUser = await BoxManager.Instance.UpdateUser(user.UserId, user.UserName, user.UserLogin);
                    if (boxUser != null)
                    {
                        delList.Add(user);
                        addList.Add(new UserDataGridView(boxUser));

                        await BoxManager.Instance.UpDateGroupUserFromName(boxUser, user.ListModAllGroup.Split('\n'));
                    }
                    continue;
                }
                // オフライン

            }
            // その場で処理するとエラーになるため
            delList.ForEach(del => SettingManager.Instance.ListUserDataGridView.Remove(del));
            addList.ForEach(add => SettingManager.Instance.ListUserDataGridView.Add(add));
        }

        /// <summary>
        /// ｢保存｣ボタン操作
        /// 　Excelに出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveExcelButtonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();

            dlg.Filter = "EXCEL ファイル|*.xlsx";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var path = dlg.FileName;
            // ファイルを保存処理
            SettingManager.Instance.SaveExcelFile(path);
        }

        private void _dataGridUserMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("■_dataGridUserMouseDoubleClick : {0} {1}", sender, e);
            // https://qiita.com/kabosu/items/2e905a532632c1512e65
            var elem = e.MouseDevice.DirectlyOver as FrameworkElement;
            if (elem == null)
            {
                return;
            }
            // ParentでDataGridCellが拾えなかった時はTemplatedParentを参照
            // （Borderをダブルクリックした時）
            var cell = (elem.Parent as DataGridCell)?? elem.TemplatedParent as DataGridCell;
            if (cell == null)
            {
                return;
            }
            // ここでcellの内容を処理
            // （cell.DataContextにバインドされたものが入っているかと思います）
            if (cell.Column.Header.ToString() != MainWindow.MENU_USER_NOW)
            {
                return;
            }
            var data = cell.DataContext as UserDataGridView;
            if (data == null)
            {
                return;
            }
            Debug.WriteLine("■_dataGridUserMouseDoubleClick Cell : {0} {1}", data.ListNowGroup, data.ListNowAllGroup);

            var selectGroupWin = new SelectGroupWindows(data.ListNowGroup);

            if (selectGroupWin.ShowDialog() == true)
            {
                data.ListNowAllGroup = selectGroupWin.ListSelectGroup;
            }
        }
    }
}
