using Box.V2.Models;
using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    public class GroupDataGridView : BaseView
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupDataGridView():base()
        {
            _flgInital = true;
        }

        /// <summary>
        /// オフラインや新規作成時のコンストラクタ
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        public GroupDataGridView(string groupName_)
        {
            LogFile.Instance.WriteLine($"{groupName_}");
            //
            GroupName = groupName_;
            OldGroupName = groupName_;

            GroupId = SettingManager.Instance.ListXmlGroupTreeView.FindGroupId(groupName_);
            GroupId =(string.IsNullOrEmpty(GroupId) == true )?(Properties.Resource.ID_NAME_OFFLINE) :( GroupId);

            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            //_inital();
            //
            string name = string.Empty;

            // 設定がない場合は作る
            //if (FolderManager.Instance.Contains(GroupName) == false)
            //{
            //    FolderManager.Instance.CreateFolder(GroupName);
            //}
            if (SettingManager.Instance.ListXmlGroupTreeView.ContainsName(GroupName) == false)
            {
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, null));
            }
            _flgInital = true;
        }

        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public GroupDataGridView(BoxGroup group_)
        {
            LogFile.Instance.WriteLine($"{group_.Id} {group_.Name}");
            //_groupBox = group_;
            GroupName = group_.Name;
            OldGroupName = group_.Name;
            GroupId = group_.Id;

            // Boxから取得
            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            // グループ名の変更された場合の処理
            string name = string.Empty;
            //if (SettingManager.Instance.ListGroupIdName.TryGetValue(GroupId, out name) == true)
            //{
            //    // 設定と現状の名前が違う場合
            //    if (GroupName != name)
            //    {
            //        //if (FolderManager.Instance.Contains(name) == true)
            //        //{
            //        //    FolderManager.Instance.UpdateGroupName(name, GroupName);
            //        //}
            //        if (SettingManager.Instance.ListXmlGroupTreeView.ContainsName(GroupName) == false)
            //        {
            //            SettingManager.Instance.ListXmlGroupTreeView.UpdateGroupName(name, GroupName);
            //        }
            //    }
            //}
            var listTreeView = SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupId(GroupId);
            foreach (var view in listTreeView)
            {
                if (view.GroupName != GroupName)
                {
                    view.GroupName = GroupName;
                }
            }

            // 設定がない場合は作る
            //if (FolderManager.Instance.Contains(GroupName) == false)
            //{
            //    FolderManager.Instance.CreateFolder(GroupName);
            //}
            if (SettingManager.Instance.ListXmlGroupTreeView.ContainsId(GroupId) == false)
            {
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, GroupId, null));
            }

            var listAllGroupName = SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupName(GroupName);


            if (listAllGroupName.Count>0)
            {
                foreach (var view in listAllGroupName)
                {
                    if (string.IsNullOrEmpty(view.GroupId)==true)
                    {
                        view.GroupId = GroupId;
                    }
                }
            }
            else
            {
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, GroupId, null));
            }

            //SettingManager.Instance.ListGroupIdName[GroupId] = GroupName;
            _flgInital = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName_"></param>
        protected override void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            if (_flgInital == true )
            {
                _statudData = (string.IsNullOrEmpty(GroupId) == true) ? Status.NEW : Status.MOD;
                base._NotifyPropertyChanged("StatusName");
            }
            base._NotifyPropertyChanged(propertyName_);
        }

        /// <summary>
        /// コンストラクタの場合、状態更新しない
        /// </summary>
        private bool _flgInital = false;

        /// <summary>
        /// 状態
        /// </summary>

        private Status _statudData = Status.NONE;
        public Status StatudData
        {
            get => _statudData;
            private set => _statudData = value;
        }
        public string StatusName
        {
            get => StatusString(_statudData);
        }

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;
        public bool IsSameOldGroupName { get => GroupName == OldGroupName; }

        public string OldGroupName { get; private set; } = string.Empty;

        /// <summary>
        /// グループID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;
        /// <summary>
        /// 最大のネスト最大数(基本並び替え用)
        /// </summary>
        //public int MaxNestCount { get => maxNestCount(FolderManager.Instance.ListPathFindFolderName(GroupName)); }
        public int MaxNestCount { get => SettingManager.Instance.ListXmlGroupTreeView.MaxNestCount(GroupName); }


        /// <summary>
        /// フォルダの数
        /// </summary>
        //public int FolderCount { get => FolderManager.Instance.ListPathFindFolderName(GroupName).Count; }
        public int FolderCount { get => SettingManager.Instance.ListXmlGroupTreeView.NameCount(GroupName); }

        /// <summary>
        /// ユーザーの数
        /// </summary>
        public int UserCount { get; private set; } = 0;


        /// <summary>
        /// 最大のネスト数の取得
        /// </summary>
        /// <param name="list_">パス一覧</param>
        /// <returns>最大のネスト数</returns>

    }
}
