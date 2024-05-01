using Box.V2.Models;
using BoxNestGroup.GridView;
using BoxNestGroup.Manager.Data;
using BoxNestGroup.View;
using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.IO;

namespace BoxNestGroup.Manager
{
    internal class SettingManager
    {
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
        /// グループビュー
        /// </summary>
        public ObservableCollection<BoxGroupDataGridView> ListGroupDataGridRow { get; private set; } = new ObservableCollection<BoxGroupDataGridView>();
        /// <summary>
        /// ユーザービュー
        /// </summary>
        public ObservableCollection<BoxUserDataGridView> ListUserDataGridRow { get; private set; } = new ObservableCollection<BoxUserDataGridView>();

        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理
        /// </summary>
        public List<BoxGroupMembership> _listBoxGroupMembership = new List<BoxGroupMembership>();
        /// <summary>
        /// オフラインでのグループとユーザーの紐づけ管理
        /// </summary>
        public List<KeyValuePair<string,string>> _listOffGroupMembership =new List<KeyValuePair<string,string>>(); // ユーザ名 - グループ名

        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理にデータ追加
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        public async Task AddBoxGroupMemberShip(string groupId_)
        {
            var list = await BoxManager.Instance.ListMemberIdFromGroup(groupId_);
            _listBoxGroupMembership.AddRange(list.Entries);
        }

        /// <summary>
        ///  グループに所属する人数の取得
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        /// <returns>所属人数</returns>
        public int CountBoxGroupMemberShip(string groupId_)
        {
            var list = _listBoxGroupMembership.FindAll((d) => (d.Group.Id == groupId_));
            return list.Count;
        }

        /// <summary>
        /// ユーザーが所属するグループ名の一覧取得
        /// </summary>
        /// <param name="userId__">BoxユーザーID</param>
        /// <returns>グループ名の一覧</returns>
        public IList<string> ListGroupNameInUser(string userId__)
        {
            Console.WriteLine("■ListGroupNameInUser id  [{0}]", userId__);
            var rtn = new List<string>();

            var list = _listBoxGroupMembership.FindAll((d)=>(d.User.Id==userId__));
            foreach ( var member in list)
            {
                rtn.Add(member.Group.Name);
            }
            return rtn;
        }

        /// <summary>
        /// Boxグループとフォルダのチェック処理
        /// 　新しいフォルダ名への更新
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public void CheckFolderName(BoxGroup group_)
        {
            Console.WriteLine("■CheckFolderName Id[{0}] Name[{1}]", group_.Id, group_.Name);
            string path = GroupSettingData.PathGroupSetting(group_.Id);
            Console.WriteLine("　CheckFolderName path[{0}]", path);


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
        /// グループIDからグループ名の取得
        /// </summary>
        /// <param name="groupId_">グループID</param>
        /// <returns>グループ名</returns>
        public string GetBoxGroupName(string groupId_)
        {
            var list = ListGroupDataGridRow.ToList();
            var group = list.Find((d)=>(d.GroupId ==groupId_));

            return (group == null) ? string.Empty:group.GroupName;
        }
        /// <summary>
        /// グループ名からグループIDの取得
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>グループID</returns>
        public string GetBoxGroupId(string groupName_)
        {
            var list = ListGroupDataGridRow.ToList();
            var group = list.Find((d) => (d.GroupName == groupName_));
            return (group == null) ? string.Empty : group.GroupId;

        }
        /// <summary>
        /// ユーザーの所属するグループリストの取得
        /// </summary>
        /// <param name="userId_">ユーザーID</param>
        /// <param name="listAdd_">追加したいリストを指定する場合のグループ名一覧</param>
        /// <returns>ユーザーとグループの紐づけ一覧</returns>
        public IList<BoxGroupMembership> ListGroupMembershipFromUserId( string userId_, IList<string> ? listAdd_ =null)
        {
            Console.WriteLine("■ListGroupMembershipFromUserId id  [{0}]", userId_);

            var rtn = _listBoxGroupMembership.FindAll((d) => (d.User.Id == userId_));
            if (listAdd_ != null) 
            {
                return rtn.FindAll((d)=>(listAdd_.Contains(d.Group.Name))==true);
            }

            return rtn;
        }
        /// <summary>
        /// ネストをして増やしたグループ名一覧
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>グループ名一覧</returns>
        public IList<string> ListNestGroupId(IList<string> listGroupName_) 
        {
            var rtn = new List<string>();

            var listGroup = ListGroupDataGridRow.ToList();
            foreach (var name in listGroupName_)
            {
                var find = listGroup.Find( ( g) =>( g.GroupName == name) );

                if (find == null)
                {
                    continue;
                }
                rtn.Add(find.GroupId);
            }
            return rtn;
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
        public async void LoadExcelSheet(IXLWorksheet sheet_)
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
                
                ListUserDataGridRow.Add(add);

                foreach (var groupName in listGroup) 
                {
                    _listOffGroupMembership.Add( new KeyValuePair<string, string>(userName, groupName));
                }
            }

            foreach (var groupName in listGroup)
            {
                var add =new BoxGroupDataGridView(groupName);
                await add.Inital();
                ListGroupDataGridRow.Add(add);
            }
        }

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
                foreach (var user in SettingManager.Instance.ListUserDataGridRow)
                {
                    if (user.ListNestGroup == string.Empty)
                    {
                        continue;
                    }

                    worksheet.Cell(row, 1).Value = user.UserName;
                    worksheet.Cell(row, 2).Value = user.UserMailAddress;
                    worksheet.Cell(row, 3).Value = user.ListNestGroup.Replace("\n", ";");
                    worksheet.Cell(row, 4).Value = (user.UserSpaceUsed == BoxUserDataGridView.APP_UNLIMITED) ? BoxUserDataGridView.BOX_UNLIMITED : user.UserSpaceUsed;
                    worksheet.Cell(row, 5).Value = (user.UserExternalCollaborate == BoxUserDataGridView.APP_ENABLED) ? BoxUserDataGridView.BOX_ENABLED : BoxUserDataGridView.BOX_DISABLED;
                    row++;

                }

                workbook.SaveAs(path_);

            }
        }
    }
}
