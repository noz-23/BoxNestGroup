using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Windows;
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
        /// 「StringResouce.xaml」からの読み出し
        /// </summary>
        private const string USER_GROUP_NOW = "UserGroupNow";

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
        private void _buttonMakeAndRenewUserGroupClick(object sender, RoutedEventArgs e)
        {
            if (BoxManager.Instance.IsOnlne == false)
            {
                // オフラインの場合は必要がない
                return;
            }

            // 更新は削除と追加で行う
            var delList = new List<UserDataGridView>();
            var addList = new List<UserDataGridView>();
            SettingManager.Instance.ListUserDataGridView?.ToList().ForEach(async (view_) =>
            {

                if (view_.StatudData == UserDataGridView.Status.NEW)
                {
                    LogFile.Instance.WriteLine($"新規 [{view_.UserName}] [{view_.UserLogin}]");
                    // 新規
                    var boxUser = await BoxManager.Instance.CreateUser(view_.UserName, view_.UserLogin);
                    if (boxUser != null)
                    {
                        delList.Add(view_);
                        addList.Add(new UserDataGridView(boxUser));

                        await BoxManager.Instance.AddGroupUserFromName(boxUser, view_.ListModAllGroup.Split('\n'));
                    }
                    return;
                }
                if (view_.StatudData == UserDataGridView.Status.MOD)
                {
                    LogFile.Instance.WriteLine($"変更 [{view_.UserName}] [{view_.UserLogin}]");
                    // 変更
                    var boxUser = await BoxManager.Instance.UpdateUser(view_.UserId, view_.UserName, view_.UserLogin);
                    if (boxUser != null)
                    {
                        delList.Add(view_);
                        addList.Add(new UserDataGridView(boxUser));

                        await BoxManager.Instance.UpDateGroupUserFromName(boxUser, view_.ListModAllGroup.Split('\n'));
                    }
                    return;
                }
                // オフライン

            });
            // その場で処理するとエラーになるため
            delList.ForEach(del => SettingManager.Instance.ListUserDataGridView?.Remove(del));
            addList.ForEach(add => SettingManager.Instance.ListUserDataGridView?.Add(add));
        }

        /// <summary>
        /// ｢保存｣ボタン操作
        /// 　Excelに出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveExcelButtonClick(object sender_, RoutedEventArgs e_)
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

        /// <summary>
        /// ダブルクリック処理(グループ追加)
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _dataGridUserMouseDoubleClick(object sender_, MouseButtonEventArgs e_)
        {
            // https://qiita.com/kabosu/items/2e905a532632c1512e65
            var elem = e_.MouseDevice.DirectlyOver as FrameworkElement;
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
            if (cell.Column.Header==null)
            {
                return;
            }
            // https://gist.github.com/kikuchy/2559432
            if (cell.Column.Header.ToString() != App.Current.Resources[USER_GROUP_NOW].ToString())
            {
                return;
            }
            var data = cell.DataContext as UserDataGridView;
            if (data == null)
            {
                return;
            }
            LogFile.Instance.WriteLine($"変更 [{data.ListNowGroup}] [{data.ListNowAllGroup}]");

            var selectGroupWin = new SelectGroupWindows(data.ListNowGroup);

            if (selectGroupWin.ShowDialog() == true)
            {
                data.ListNowAllGroup = selectGroupWin.ListSelectGroup;
            }
        }
    }
}
