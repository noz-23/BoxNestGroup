using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// UserGridViewControl.xaml の相互作用ロジック
    /// </summary>

    public partial class UserGridViewControl : System.Windows.Controls.UserControl
    {
        public ReNewDelegate Renew = null;

        public UserGridViewControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 「ユーザー作成」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonMakeUserButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■makeUserButtonClick : {0} {1}", sender, e);
        }

        /// <summary>
        /// 「更新」ボタン操作
        ///  設定されているグループに更新
        /// </summary>
        private async void _buttonUserGroupChangeButtonClick(object sender, RoutedEventArgs e)
        {
            /*
                        Debug.WriteLine("■renewUserButtonClick : {0} {1}", sender, e);
                        //var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
                        var listUser = _dataGridUser.ItemsSource as BoxUserDataGridModel;
                        if (listUser == null)
                        {
                            return;
                        }
                        //var listUser = view.SourceCollection;

                        //if (listUser == null)
                        //{
                        //	return;
                        //}

                        var wait = new WaitWindow() { Owner = this };
                        wait.Show();

                        //_buttonRenewUser.IsEnabled = false;

                        foreach (BoxUserDataGridView user in listUser)
                        {
                            if (user == null)
                            {
                                continue;
                            }
                            if (user.ListModGroup == string.Empty)
                            {
                                continue;
                            }
                            var listNow = new List<string>(user.ListNowAllGroup.Split("\n"));
                            var listMod = new List<string>(user.ListModAllGroup.Split("\n"));

                            // 削除するユーザとグループの管理IDリストを取得し削除
                            var listDel = new List<string>(listNow);
                            foreach (var name in listMod)
                            {
                                // 更新後にあるフォルダはそのまま
                                listDel.Remove(name);
                            }
                            Debug.WriteLine(string.Format("Name[{0}] /ID[{1}]", user.UserName, user.UserId));

                            Debug.WriteLine(string.Format("\trenewUserButtonClick listDel: {0}", string.Join(",", listDel)));
                            var listDelMem = SettingManager.Instance.ListBoxGroupMembership.ListGroupMembershipFromUserId(user.UserId, listDel);
                            await BoxManager.Instance.DeleteGroupUser(listDelMem);

                            // 更新するリストのグループIDを取得後、ユーザーとグループを紐づける(更新)
                            var listAdd = new List<string>(listMod);
                            foreach (var name in listNow)
                            {
                                // 更新前と更新後がそのままのの場合は削除
                                listAdd.Remove(name);
                            }
                            Debug.WriteLine(string.Format("\trenewUserButtonClick listAdd: {0}", string.Join(",", listAdd)));
                            var listAddGroupId = SettingManager.Instance.ListGroupDataGridView.ListNestGroupId(listAdd);
                            await BoxManager.Instance.AddGroupUser(user.UserId, listAddGroupId);
                        }

                        //SettingManager.Instance.LoadFile();

                        //var box = await renewBox();
                        //await renewBox();
                        if (Renew != null)
                        {
                            await Renew();
                        }
                        //Debug.WriteLine("　renewUserButtonClick box:{0}", box);
                        //await setView();
                        //await checkFolderToDataGridViewGroupData();

                        wait.Close();
                        //_buttonRenewUser.IsEnabled = true;
            */
        }


        /// <summary>
        /// 「追加」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonAddUserGroupClick(object sender, RoutedEventArgs e)
        {
            /*
                        Debug.WriteLine("■addUserGroupClick : {0} {1}", sender, e);
                        //
                        //var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
                        //var listUser = SettingManager.Instance.ListUserDataGridRow;
                        var listUser = SettingManager.Instance.ListUserDataGridView;
                        var selectGroup = _treeViewFolder.SelectedItem as FolderGroupTreeView;

                        if (listUser == null)
                        {
                            return;
                        }
                        if (selectGroup == null)
                        {
                            return;
                        }

                        foreach (var user in listUser)
                        {
                            //if (user.Selected == false)
                            //{
                            //	// チェックがない場合は処理しない
                            //	continue;
                            //}
                            if (selectGroup.GroupName == Settings.Default.ClearGroupName)
                            {
                                // クリア設定の場合はクリア
                                //user.ListNestGroup = string.Empty;
                                user.ListModGroup = selectGroup.GroupName;
                                continue;
                            }

                            var addList = new List<string>();

                            var nowList = user.ListModGroup;
                            if (nowList != string.Empty)
                            {
                                addList.AddRange(nowList.Split("\n"));
                            }

                            addList.Add(selectGroup.GroupName);
                            user.ListModGroup = string.Join("\n", new HashSet<string>(addList));

                            // 重複はHashSetで消えるから、そのまま追加
                            addList.AddRange(FolderManager.Instance.ListUniqueGroup(addList));

                            //user.ListNestGroup = string.Join("\n", new HashSet<string>(addList));
                        }

                        //dataGridUser.ItemsSource = new List<BoxUserDataGridView>(listUser);
                        //dataGridUser.UpdateLayout();
                        //await setView();
            */
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
            if (elem != null)
            {
                DataGridCell cell = elem.Parent as DataGridCell;
                if (cell == null)
                {
                    // ParentでDataGridCellが拾えなかった時はTemplatedParentを参照
                    // （Borderをダブルクリックした時）
                    cell = elem.TemplatedParent as DataGridCell;
                }
                if (cell != null)
                {
                    // ここでcellの内容を処理
                    // （cell.DataContextにバインドされたものが入っているかと思います）

                    if (cell.Column.Header.ToString() == MainWindow.MENU_USER_NOW)
                    {
                        var data = cell.DataContext as BoxUserDataGridView;

                        Debug.WriteLine("■_dataGridUserMouseDoubleClick Cell : {0} {1}", data.ListNowGroup, data.ListNowGroup);

                    }
                }
            }
        }
    }
}
