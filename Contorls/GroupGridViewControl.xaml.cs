﻿using Box.V2.Models;
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

        }
    }
 }
