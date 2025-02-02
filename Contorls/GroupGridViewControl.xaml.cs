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
                    if(BoxManager.Instance.IsOnlne ==true)
                    {
                        LogFile.Instance.WriteLine($"OffLine {group.GroupName} {group.GroupId}");

                        var boxGroup = await BoxManager.Instance.CreateGroup(group.GroupName);
                        if (boxGroup != null)
                        {
                            delList.Add(group);
                            addList.Add(new GroupDataGridView(boxGroup));
                        }
                    }

                    // オンラインのグループID検索
                    foreach (var view in SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupId(group.GroupId))
                    {
                        // グループIDが同じで名前が違う場合変更
                        if (view.GroupName != group.GroupName)
                        {
                            LogFile.Instance.WriteLine($"ID[{group.GroupId}] [{view.GroupName}] -> [{group.GroupName}]");
                            // グループIDが同じで名前が違う場合変更
                            view.GroupName = group.GroupName;
                        }
                    }
                    foreach (var view in SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupName(group.GroupName))
                    {
                        // 名前が同じでグループIDがない場合
                        if (string.IsNullOrEmpty(view.GroupId)==true)
                        {
                            LogFile.Instance.WriteLine($"Name[{group.GroupName}] [{view.GroupId}] -> [{group.GroupId}]");
                            // 名前が同じでグループIDがない場合
                            view.GroupId = group.GroupId;
                        }
                    }

                    if (SettingManager.Instance.ListXmlGroupTreeView.ContainsName(group.GroupName) == false)
                    {
                        LogFile.Instance.WriteLine($"Update {group.OldGroupName} -> {group.GroupName}");
                        // 新しいグループ名がない場合
                        SettingManager.Instance.ListXmlGroupTreeView.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListMembershipGroupNameLogin.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListUserDataGridView.UpdateGroupName(group.OldGroupName, group.GroupName);
                    }
                    continue;
                }
                // オフライン
                if (group.GroupId == Resource.OfflineName)
                {
                    if( group.IsSameOldGroupName==false)
                    {
                        //FolderManager.Instance.UpdateGroupName(group.OldGroupName, group.GroupName);
                        SettingManager.Instance.ListXmlGroupTreeView.UpdateGroupName(group.OldGroupName, group.GroupName);
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
