using Box.V2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxNestGroup.Manager.Data;
using BoxNestGroup.GridView;
using System.Collections.ObjectModel;
using BoxNestGroup.View;
using DocumentFormat.OpenXml.InkML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Box.V2.Models.Request;

namespace BoxNestGroup.Manager
{
    internal class SettingManager
    {
        // シングルトン
        static public SettingManager Instance { get; } = new SettingManager();

        //
        
        public ObservableCollection<BoxGroupDataGridView> ListGroupDataGridRow { get; private set; } = new ObservableCollection<BoxGroupDataGridView>();
        public ObservableCollection<BoxUserDataGridView> ListUserDataGridRow { get; private set; } = new ObservableCollection<BoxUserDataGridView>();

        // Box でのグループとユーザーの紐づけ管理
        public List<BoxGroupMembership> _listBoxGroupMembership = new List<BoxGroupMembership>();
        // オフラインでのグループとユーザーの紐づけ管理
        public List<KeyValuePair<string,string>> _listOffGroupMembership =new List<KeyValuePair<string,string>>(); // ユーザ名 - グループ名

        // グループ設定とIDの紐づけ
        //private List<GroupSettingData> _listGroupData = new List<GroupSettingData>();
        private SettingManager()
        {
        }

        //public void LoadFile()
        //{
        //    _listGroupData.Clear();
        //    foreach (var path in Directory.GetFiles(_commonGroupSettingPath, "*" + _settingFile))
        //    {
        //        var id = path.Replace(_commonGroupSettingPath, string.Empty).Replace(_settingFile, string.Empty).Replace(@"\", string.Empty);
        //        _listGroupData.Add(new GroupSettingData( path, id));
        //    }
        //    Console.WriteLine("■SettingManager.LoadFile.Count:{0}", _listGroupData.Count);
        //}

        //public void AddBoxGroup(BoxGroup boxGroup_)
        //{
        //    if (boxGroup_ == null)
        //    {
        //        return;
        //    }
        //    if (boxGroup_.Id == string.Empty)
        //    {
        //        return;
        //    }
        //    Console.WriteLine("■CheckSettingBoxAndFolder  box:name[{0}] id[{1}]", boxGroup_.Name, boxGroup_.Id);

        //    var find = _listGroupData.Find(d => d.GroupId == boxGroup_.Id);

        //    if (find == null)
        //    {
        //        // 設定ファイルがない場合作る
        //        _listGroupData.Add(new GroupSettingData(_pathGroupSetting(boxGroup_.Id), boxGroup_));
        //    }
        //    else
        //    {
        //        Console.WriteLine("　CheckSettingBoxAndFolder find:name[{0}] id_[{1}]", find.GroupName, find.GroupId);
        //        // 設定がある場合は、フォルダ名をチェック
        //        if (find.GroupName != boxGroup_.Name)
        //        {
        //            // 保存しているフォルダと実際が違う場合
        //            //if (FolderManager.Instance.IsHaveGroup(find.GroupName) == true)
        //            //{
        //            // 古いフォルダがあった場合、フォルダ名を変更
        //            FolderManager.Instance.UpdateFolder(find.GroupName, boxGroup_.Name);
        //            // 削除してから新規作成
        //            _listGroupData.Remove(find);
        //            _listGroupData.Add(new GroupSettingData(_pathGroupSetting(boxGroup_.Id), boxGroup_));
        //            //}
        //        }
        //        else if (FolderManager.Instance.IsHaveGroupForlder(boxGroup_.Name) == false)
        //        {
        //            // 現状なかったら作る
        //            FolderManager.Instance.CreateFolder(boxGroup_.Name);
        //        }
        //    }
        //    Console.WriteLine("　CheckSettingBoxAndFolder count[{0}]", _listGroupData.Count);
        //}

        public async Task AddBoxGroupMemberShip(string groupId_)
        {
            //var find = _listGroupData.Find(d => d.GroupId == groupId_);
            //if (find == null)
            //{
            //    return;
            //}

            //await find.LoadBoxGroupMemberShip();
            var list = await BoxManager.Instance.ListMemberIdFromGroup(groupId_);
            _listBoxGroupMembership.AddRange(list.Entries);
        }

        public int CountBoxGroupMemberShip(string groupId_)
        {
            //var find = _listGroupData.Find(d => d.GroupId == groupId_);
            //if(find== null)
            //{ 
            //    return 0; 
            //}
            //return find.ListBoxMemberShipCount();
            var list = _listBoxGroupMembership.FindAll((d) => (d.Group.Id == groupId_));
            return list.Count;
        }

        // ユーザーが所属するグループの取得
        public List<string> ListGroupNameInUser(string userId__)
        {
            Console.WriteLine("■ListGroupNameInUser id  [{0}]", userId__);
            var rtn = new List<string>();

            //foreach (var group in _listGroupData)
            //{
            //    if (group.ListBoxMemberShip == null)
            //    {
            //        continue;
            //    }
            //    if (group.ListBoxMemberShipContains(userId__) == true)
            //    {
            //        rtn.Add(group.GroupName);
            //        Console.WriteLine("　ListGroupNameInUser add [{0}]", group.GroupName);
            //    }
            //}
            var list = _listBoxGroupMembership.FindAll((d)=>(d.User.Id==userId__));
            foreach ( var member in list)
            {
                rtn.Add(member.Group.Name);
            }
            return rtn;
        }

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

        public void CheckFolderName(string groupName_)
        {
            FolderManager.Instance.CreateFolder(groupName_);
        }

        public string GetBoxGroupName(string groupId_)
        {
            //var group = _listGroupData.Find((g) => g.GroupId == groupId_);

            //return group.GroupName;
            var list = ListGroupDataGridRow.ToList();
            var group = list.Find((d)=>(d.GroupId ==groupId_));

            return (group == null) ? string.Empty:group.GroupName;

        }
        public string GetBoxGroupId(string groupName_)
        {
            //var group = _listGroupData.Find((g) => g.GroupName == groupName_);

            //return group.GroupId;
            var list = ListGroupDataGridRow.ToList();
            var group = list.Find((d) => (d.GroupName == groupName_));
            return (group == null) ? string.Empty : group.GroupId;

        }

        public List<BoxGroupMembership> ListGroupMembershipFromUserId( string userId_, List<string> ? listDel_ =null)
        {
            Console.WriteLine("■ListGroupMembershipFromUserId id  [{0}]", userId_);
            //var rtn = new List<BoxGroupMembership>();

            //foreach (var group in ListGroupMembership)
            //{
            //    if(listDel_ !=null)
            //    {
            //        if (listDel_.Contains(group.GroupName) == false)
            //        {
            //            // 削除フォルダ候補にない場合はそのまま
            //            continue;
            //        }
            //    }

            //    foreach (var member in group.ListBoxMemberShip) 
            //    {
            //        if (member.User.Id == userId_)
            //        {
            //            rtn.Add(member);
            //        }
            //    }
            //}

            //return rtn;

            var rtn = _listBoxGroupMembership.FindAll((d) => (d.User.Id == userId_));
            if (listDel_ != null) 
            {
                return rtn.FindAll((d)=>(listDel_.Contains(d.Group.Name))==true);
            }

            return rtn;
        }
        public List<string> ListNestGroupId(List<string> listGroupName_) 
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

        public async void LoadExcelSheet(IXLWorksheet sheet_)
        {
            var listGroup =new HashSet<string>();

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
        public string GetGroupIdFromSttingData(string name_)
        {
            var listFile = GroupSettingData.ListAllGroupSettingData();
            foreach (var path in listFile)
            {
                var name = GroupSettingData.ReadGroupName(path);

                if (name == name_)
                {
                    return GroupSettingData.GetGroupId(path);
                }
            }

            return string.Empty;
        }

        public int CountGroupMemberShipFromSettingData(string groupName_)
        {
            var list = _listOffGroupMembership.FindAll((d) => (d.Value == groupName_));
            return list.Count;
        }
    }
}
