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

namespace BoxNestGroup.Manager
{
    internal class SettingManager
    {
        static public SettingManager Instance { get; } = new SettingManager();

        private string _commonGroupSettingPath = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupSetting;


        private Dictionary<string, GroupSettingData> _listGroupData = new Dictionary<string, GroupSettingData>();

        private const string _settingFile = "_Setting.txt";
        private string _pathGroupSetting(string id_)
        {
            return _commonGroupSettingPath + @"\" + id_ + _settingFile;
        }

        private SettingManager()
        {
        }

        public void Load()
        {
            _listGroupData.Clear();
            foreach (var path in Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + _settingFile))
            {
                var id = path.Replace(Directory.GetCurrentDirectory(), string.Empty).Replace(_settingFile, string.Empty).Replace(@"\", string.Empty);
                _listGroupData[id] = new GroupSettingData(path);
            }
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

        public void CreateSetting(BoxGroup boxGroup_)
        {
            _listGroupData.Add(boxGroup_.Id, new GroupSettingData(_pathGroupSetting(boxGroup_.Id), boxGroup_.Name));
        }

        // 設定ファイルでのBoxとフォルダ比較
        public void CheckSettingBoxAndFolder(BoxGroup boxGroup_) 
        {
            if (_listGroupData.ContainsKey(boxGroup_.Id) == false)
            {
                // ない場合は作る
                FolderManager.Instance.CreateFolder(boxGroup_);
                CreateSetting(boxGroup_);
                return;
            }

            var data = _listGroupData[boxGroup_.Id];
            if (data.GroupName == boxGroup_.Name)
            {
                // 同じ場合は処理抜け
                return;
            }

            // 違う場合は更新
            FolderManager.Instance.UpdateFolder(data.GroupName ,boxGroup_.Name);
            _listGroupData[boxGroup_.Id] = new GroupSettingData(_pathGroupSetting(boxGroup_.Id), boxGroup_.Name, data.ListMemberId);

        }

    }
}
