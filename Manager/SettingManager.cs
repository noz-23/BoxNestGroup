﻿using Box.V2.Models;
using BoxNestGroup.Manager.Data;
using BoxNestGroup.View;
using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BoxNestGroup.Manager
{
    internal class SettingManager: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// シングルトン
        /// </summary>
        static public SettingManager Instance { get; } = new SettingManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SettingManager()
        {
        }

        /// <summary>
        /// ログインネーム
        /// </summary>
        private string _loginName = "認証前";
        public string LoginName 
        {
            get { return _loginName; }
            set
            { 
                _loginName = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("LoginName");
            }
                
        } 
        
        /// <summary>
        /// グループビュー
        /// </summary>
        //public ObservableCollection<BoxGroupDataGridView> ListGroupDataGridRow { get; private set; } = new ObservableCollection<BoxGroupDataGridView>();
        public BoxGroupDataGridModel ListGroupDataGridView { get; private set; } = new BoxGroupDataGridModel();

        /// <summary>
        /// ユーザービュー
        /// </summary>
        public BoxUserDataGridModel ListUserDataGridView { get; private set; } = new BoxUserDataGridModel();

        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理
        ///  グループとユーザーの両方でカウントありのためBoxGroupMembershipを登録
        /// </summary>
        public ListBoxMembership ListBoxGroupMembership { get; private set; }= new ListBoxMembership();

        /// <summary>
        /// オフラインでのグループとユーザーの紐づけ管理
        /// </summary>
        public List<KeyValuePair<string,string>> _listOffGroupMembership =new List<KeyValuePair<string,string>>(); // ユーザ名 - グループ名

        /// <summary>
        /// Boxグループとフォルダのチェック処理
        /// 　新しいフォルダ名への更新
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public void CheckFolderName(BoxGroup? group_)
        {
            if (group_ == null)
            {
                return;
            }

            //Debug.WriteLine(string.Format("■CheckFolderName Id[{0}] Name[{1}]", group_.Id, group_.Name));
            string path = GroupSettingData.PathGroupSetting(group_.Id);
            //Debug.WriteLine(string.Format("\tCheckFolderName path[{0}]", path));


            if (File.Exists(path) == true)
            {
                var name =GroupSettingData.ReadGroupName(path);

                if (name == group_.Name) 
                {
                    return;
                }
                FolderManager.Instance.UpdateFolder(name, group_.Name);

            }
            GroupSettingData.WriteGroupName(path, group_.Name);
            FolderManager.Instance.CreateFolder(group_.Name);
        }

        /// <summary>
        /// 設定データからのグループID取得
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>グループID</returns>
        public string GetGroupIdFromSttingData(string groupName_)
        {
            var listFile = GroupSettingData.ListAllGroupSettingData();
            foreach (var path in listFile)
            {
                var name = GroupSettingData.ReadGroupName(path);

                if (name == groupName_)
                {
                    return GroupSettingData.GetGroupId(path);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 設定データからのグループ所属人数を取得
        /// </summary>
        /// <param name="groupName_">所属人数</param>
        /// <returns>グループの所属数</returns>
        public int CountGroupMemberShipFromSettingData(string groupName_)
        {
            var list = _listOffGroupMembership.FindAll((d) => (d.Value == groupName_));
            return list.Count;
        }

        /// <summary>
        /// エクセルのシート読み込み処理
        /// </summary>
        /// <param name="sheet_">エクセルのシート</param>
        public void LoadExcelSheet(IXLWorksheet sheet_)
        {
            var listGroup =new HashSet<string>();

            // 1行目はヘッダーのため飛ばし
            for (var row = 2; row < sheet_.RowCount()+1; row++)
            {
                var userName = sheet_.Cell(row, 1).Value.ToString();
                var userMail = sheet_.Cell(row, 2).Value.ToString();
                //
                var group = new List<string>(sheet_.Cell(row, 3).Value.ToString().Split(";"));
                group.Remove(string.Empty);
                listGroup.UnionWith(group);
                //
                var strage = sheet_.Cell(row, 4).Value.ToString();
                var colabo = sheet_.Cell(row, 5).Value.ToString();
                //
                var add = new BoxUserDataGridView(userName, userMail, group, strage, colabo);

                if (userName == string.Empty)
                {
                    break;
                }
                //listDataGridRow.Add(add);
                
                //ListUserDataGridRow.Add(add);
                ListUserDataGridView.Add(add);

                foreach (var groupName in listGroup) 
                {
                    _listOffGroupMembership.Add( new KeyValuePair<string, string>(userName, groupName));
                }
            }

            foreach (var groupName in listGroup)
            {
                //var add =new BoxGroupDataGridView(groupName);
                //await add.Inital();
                //ListGroupDataGridRow.Add(add);
                ListGroupDataGridView.Add(new BoxGroupDataGridView(groupName));
            }
        }

        /// <summary>
        /// エクセルのシートの保存処理
        /// </summary>
        /// <param name="path_"></param>
        public void SaveExcelFile(string path_)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(1);

                worksheet.Cell(1, 1).Value = "名前";
                worksheet.Cell(1, 2).Value = "メール";
                worksheet.Cell(1, 3).Value = "グループ";
                worksheet.Cell(1, 4).Value = "ストレージ";
                worksheet.Cell(1, 5).Value = "外部コラボレーション制限";

                int row = 2;
                //foreach (var user in SettingManager.Instance.ListUserDataGridRow)
                foreach (var user in SettingManager.Instance.ListUserDataGridView)
                {
                    if (user.ListModAllGroup == string.Empty)
                    {
                        continue;
                    }

                    worksheet.Cell(row, 1).Value = user.UserName;
                    worksheet.Cell(row, 2).Value = user.UserMailAddress;
                    worksheet.Cell(row, 3).Value = user.ListModAllGroup.Replace("\n", ";");
                    worksheet.Cell(row, 4).Value = (user.UserSpaceUsed == BoxUserDataGridView.APP_UNLIMITED) ? BoxUserDataGridView.BOX_UNLIMITED : user.UserSpaceUsed;
                    worksheet.Cell(row, 5).Value = (user.UserExternalCollaborate == BoxUserDataGridView.APP_ENABLED) ? BoxUserDataGridView.BOX_ENABLED : BoxUserDataGridView.BOX_DISABLED;
                    row++;

                }

                workbook.SaveAs(path_);

            }
        }
    }
}
