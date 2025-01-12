using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System.Windows;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// GroupGridViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupGridViewControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupGridViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「グループ作成」ボタン操作
        /// </summary>
        private async void _buttonMakeAndRenewGroupButtonClick(object sender_, RoutedEventArgs e_)
        {
            // 更新は削除と追加で行う
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
                        if (boxGroup != null)
                        {
                            delList.Add(group);
                            addList.Add(new GroupDataGridView(boxGroup));
                        }
                    }

                    if (FolderManager.Instance.Contains(group.GroupName) == false)
                    {
                        FolderManager.Instance.CreateFolder(group.GroupName);
                    }
                    continue;
                }
                // オフライン
                if (group.GroupId == Resource.OfflineName)
                {
                    if( group.IsSameOldGroupName==false)
                    {
                        FolderManager.Instance.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListMembershipGroupNameLogin.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListUserDataGridView.UpdateGroupName(group.OldGroupName, group.GroupName);

                        delList.Add(group);
                        addList.Add(new GroupDataGridView(group.GroupName));
                    }
                }
            }
            // その場で処理するとエラーになるため
            delList.ForEach(del => SettingManager.Instance.ListGroupDataGridView.Remove(del));
            addList.ForEach(add => SettingManager.Instance.ListGroupDataGridView.Add(add));
        }
    }
 }
