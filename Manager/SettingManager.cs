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

namespace BoxNestGroup.Manager
{
    internal class SettingManager
    {
        // シングルトン
        static public SettingManager Instance { get; } = new SettingManager();

        public ObservableCollection<BoxGroupDataGridView> ListGroupDataGridRow { get; private set; } = new ObservableCollection<BoxGroupDataGridView>();
        public ObservableCollection<BoxUserDataGridView> ListUserDataGridRow { get; private set; } = new ObservableCollection<BoxUserDataGridView>();


        // 基本グループ設定 フォルダ
        private string _commonGroupSettingPath = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupSetting;

        // グループ設定とIDの紐づけ
        private List<GroupSettingData> _listGroupData = new List<GroupSettingData>();



        private const string _settingFile = "_Setting.txt";
        private string _pathGroupSetting(string id_)
        {
            return _commonGroupSettingPath + @"\" + id_ + _settingFile;
        }

        private SettingManager()
        {
        }

        public void LoadFile()
        {
            _listGroupData.Clear();
            foreach (var path in Directory.GetFiles(_commonGroupSettingPath, "*" + _settingFile))
            {
                var id = path.Replace(_commonGroupSettingPath, string.Empty).Replace(_settingFile, string.Empty).Replace(@"\", string.Empty);
                _listGroupData.Add(new GroupSettingData(id, path));
            }
            Console.WriteLine("■SettingManager.LoadFile.Count:{0}", _listGroupData.Count);
        }
        /*public void Check(ObservableCollection<BoxGroupDataGridView> listGroup_)
        {
            foreach (var boxGroup in listGroup_)
            {
                if (_listGroupData.ContainsKey(boxGroup.GroupId) == true)
                {
                    var data = _listGroupData[boxGroup.GroupId];
                    if (data.GroupName != boxGroup.GroupName)
                    {
                    }
                }
            }
        }
        */

        //public void CreateSetting(BoxGroup boxGroup_)
        //{
        //    _listGroupData.Add(new GroupSettingData(boxGroup_.Id, _pathGroupSetting(boxGroup_.Id), boxGroup_.Name));
        //}

        // 設定ファイルでのBoxとフォルダ比較
        //public void CheckSettingBoxAndFolder(string name_, string id_)
        public void CheckSettingBoxAndFolder(BoxGroup boxGroup_)
        {
            if (boxGroup_ == null)
            {
                return;
            }
            if (boxGroup_.Id == string.Empty)
            {
                return;
            }
            Console.WriteLine("■CheckSettingBoxAndFolder  box:name[{0}] id[{1}]", boxGroup_.Name, boxGroup_.Id);

            var find = _listGroupData.Find(d => d.GroupId == boxGroup_.Id);

            if (find == null)
            {
                // 設定ファイルがない場合作る
                _listGroupData.Add(new GroupSettingData(boxGroup_, _pathGroupSetting(boxGroup_.Id)));
            }
            else
            {
                Console.WriteLine("　CheckSettingBoxAndFolder find:name[{0}] id_[{1}]", find.GroupName, find.GroupId);
                // 設定がある場合は、フォルダ名をチェック
                if (find.GroupName != boxGroup_.Name)
                {
                    // 保存しているフォルダと実際が違う場合
                    //if (FolderManager.Instance.IsHaveGroup(find.GroupName) == true)
                    //{
                    // 古いフォルダがあった場合、フォルダ名を変更
                    FolderManager.Instance.UpdateFolder(find.GroupName, boxGroup_.Name);
                    // 削除してから新規作成
                    _listGroupData.Remove(find);
                    _listGroupData.Add(new GroupSettingData(boxGroup_, _pathGroupSetting(boxGroup_.Id)));
                    //}
                }
                else if (FolderManager.Instance.IsHaveGroupForlder(boxGroup_.Name) == false)
                {
                    // 現状なかったら作る
                    FolderManager.Instance.CreateFolder(boxGroup_.Name);
                }
            }

            //if (_listGroupData.ContainsKey(boxGroup_.Id) == false)
            //{
            //    // ない場合は作る
            //    FolderManager.Instance.CreateFolder(boxGroup_);
            //    CreateSetting(boxGroup_);
            //    return;
            //}

            //var data = _listGroupData[boxGroup_.Id];
            //if (data.GroupName == boxGroup_.Name)
            //{
            //    // 同じ場合は処理抜け
            //    return;
            //}

            //// 違う場合は更新
            //FolderManager.Instance.UpdateFolder(data.GroupName ,boxGroup_.Name);
            //_listGroupData[boxGroup_.Id] = new GroupSettingData(_pathGroupSetting(boxGroup_.Id), boxGroup_.Name, data.ListMemberId);
            Console.WriteLine("　CheckSettingBoxAndFolder count[{0}]", _listGroupData.Count);
        }

        public async Task<int> LoadSettingBoxGroupMemberId(string groupId__)
        {
            var find = _listGroupData.Find(d => d.GroupId == groupId__);
            if (find == null)
            {
                // 本来ない
                return 0;
            }
            return await find.LoadGroupMemberId();

        }

        // ユーザーが所属するグループの取得
        public List<string> GetListGroupNameInUser(string userId__)
        {
            Console.WriteLine("■GetListGroupNameInUser id  [{0}]", userId__);
            var rtn = new List<string>();

            foreach (var group in _listGroupData)
            {
                //if (group._loadMemberId == false)
                //{
                //    continue;
                //}
                //if (group.ListMemberId.Contains(userId__) == true)
                //{
                //    rtn.Add(group.GroupName);
                //    Console.WriteLine("　GetListGroupNameInUser add [{0}]", group.GroupName);
                //}
                if (group.ListMember == null)
                {
                    continue;
                }
                //if (group.ListMember.Contains(userId__) == true)
                if (group.ListMemberContains(userId__) == true)
                {
                    rtn.Add(group.GroupName);
                    Console.WriteLine("　GetListGroupNameInUser add [{0}]", group.GroupName);
                }
            }


            return rtn;
        }

        public string GetGroupName(string groupId_)
        {
            var group = _listGroupData.Find((g) => g.GroupId == groupId_);

            return group.GroupName;

        }
        public string GetGroupId(string groupName_)
        {
            var group = _listGroupData.Find((g) => g.GroupName == groupName_);

            return group.GroupId;

        }

        public List<BoxGroupMembership> ListGroupMembership( string userId)
        {
            var rtn = new List<BoxGroupMembership>();

            foreach (var group in _listGroupData)
            {
                foreach (var member in group.ListMember) 
                {
                    if (member.User.Id == userId)
                    {
                        rtn.Add(member);
                    }
                }
            }

            return rtn;
        }
    }
}
