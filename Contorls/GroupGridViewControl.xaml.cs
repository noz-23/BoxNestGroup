using Box.V2.Models;
using BoxNestGroup;
using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// GroupGridViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupGridViewControl : System.Windows.Controls.UserControl
    {
        public ReNewDelegate Renew = null;


        public GroupGridViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「グループ作成」ボタン操作
        /// </summary>
        private async void _buttonMakeAndRenewGroupButtonClick(object sender, RoutedEventArgs e)
        {
            var delList = new List< GroupDataGridView >();
            var addList = new List<GroupDataGridView>();
            foreach (var group in SettingManager.Instance.ListGroupDataGridView)
            {
                if (string.IsNullOrEmpty(group.GroupId) == true)
                {
                    // 新規
                    if(BoxManager.Instance.IsOnlne)
                    {
                        var boxGroup = await BoxManager.Instance.CreateGroup(group.GroupName);
                        delList.Add(group);
                        addList.Add(new GroupDataGridView(boxGroup));
                    }

                    if (FolderManager.Instance.Contains(group.GroupName) == false)
                    {
                        FolderManager.Instance.CreateFolder(group.GroupName);
                    }
                    continue;
                }
                if (group.GroupId == GroupDataGridView.OFFLINE_GROUP_ID)
                {
                    if( group.IsSameOldGroupName==false)
                    {
                        FolderManager.Instance.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListMembershipGroupNameMail.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListUserDataGridView.UpdateGroupName(group.OldGroupName, group.GroupName);

                        delList.Add(group);
                        addList.Add(new GroupDataGridView(group.GroupName));
                    }
                }
            }

            delList.ForEach(del => SettingManager.Instance.ListGroupDataGridView.Remove(del));
            addList.ForEach(add => SettingManager.Instance.ListGroupDataGridView.Add(add));

/*
            Debug.WriteLine("■makeGroupButtonClick : {0} {1}", sender, e);

            var list = _dataGridGroup.ItemsSource as BoxGroupDataGridModel;
            if (list == null)
            {
                return;
            }
            //var listGroup = view.SourceCollection;
            //
            foreach (BoxGroupDataGridView group in list)
            {
                if (group == null)
                {
                    continue;
                }

                if (group.GroupId != string.Empty)
                {
                    // IDが入っている場合は何もしない
                    continue;
                }

                var createName = group.ModGroupName.Trim();

                if (createName == string.Empty)
                {
                    // IDが入っている場合は何もしない
                    continue;
                }

                var groupCreate = await BoxManager.Instance.CreateGroup(createName);

                if (groupCreate == null)
                {
                    continue;
                }
                FolderManager.Instance.CreateFolder(createName);

                //if (FolderManager.Instance.Contains(createName) == false)
                //{
                //	// フォルダがない場合は作成
                //	FolderManager.Instance.CreateFolder(groupCreate);
                //}
            }
            //var box = await renewBox();
            //await renewBox();
            if (Renew != null)
            {
                await Renew();
            }
            //Debug.WriteLine("　makeGroupButtonClick box:{0}", box);
            //await setView();
*/
        }
/*
        /// <summary>
        /// グループ名の編集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _dataGridGroupCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Debug.WriteLine("■_dataGridGroupCellEditEnding : {0} {1}", sender, e);

            var editBox = e.EditingElement as System.Windows.Controls.TextBox;
            if (editBox == null)
            {
                return;
            }

            var newGroupName = editBox?.Text ?? string.Empty;
            if (string.IsNullOrEmpty(newGroupName) == true)
            {
                return;
            }
            var oldGroup = (e.Row.Item as BoxGroupDataGridView);

            if (oldGroup == null)
            {
                return;
            }
            if (oldGroup.GroupId == string.Empty)
            {
                return;
            }

            var result = System.Windows.MessageBox.Show("グループ名を変更しますか\n[" + oldGroup.NowGroupName + "]→[" + newGroupName + "]", "確認", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
            {
                editBox.Text = oldGroup.NowGroupName;
                return;
            }

            Debug.WriteLine("_dataGridGroupCellEditEnding  Id[{0}] old[{1}] -> new[{2}]", oldGroup.GroupId, oldGroup.NowGroupName, newGroupName);

            var rtn = BoxManager.Instance.UpdateGroupName(oldGroup.GroupId, newGroupName);
            FolderManager.Instance.UpdateFolder(oldGroup.NowGroupName, newGroupName);

            //await BoxManager.Instance.ListGroupData();

            _dataGridUser.ItemsSource = _dataGridUser.ItemsSource;
            //await setView();
        }
 */

    }
 }
