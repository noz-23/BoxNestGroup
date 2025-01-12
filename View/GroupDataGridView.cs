using Box.V2.Models;
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
            GroupName = groupName_;
            OldGroupName = groupName_;
            GroupId = Resource.OfflineName;

            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            //_inital();
            //
            string name = string.Empty;

            // 設定がない場合は作る
            if (FolderManager.Instance.Contains(GroupName) == false)
            {
                FolderManager.Instance.CreateFolder(GroupName);
            }
            _flgInital = true;
        }

        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public GroupDataGridView(BoxGroup group_)
        {
            //_groupBox = group_;
            GroupName = group_.Name;
            OldGroupName = group_.Name;
            GroupId = group_.Id;

            // Boxから取得
            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            // グループ名の変更された場合の処理
            string name = string.Empty;
            if (SettingManager.Instance.ListGroupIdName.TryGetValue(GroupId, out name) == true)
            {
                // 設定と現状の名前が違う場合
                if (GroupName != name)
                {
                    if (FolderManager.Instance.Contains(name) == true)
                    {
                        FolderManager.Instance.UpdateGroupName(name, GroupName);
                    }
                }
            }
            // 設定がない場合は作る
            if (FolderManager.Instance.Contains(GroupName) == false)
            {
                FolderManager.Instance.CreateFolder(GroupName);
            }
            SettingManager.Instance.ListGroupIdName[GroupId] = GroupName;
            _flgInital = true;
        }



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
        public int MaxNestCount { get => maxNestCount(FolderManager.Instance.ListPathFindFolderName(GroupName)); }

        /// <summary>
        /// フォルダの数
        /// </summary>
        public int FolderCount { get => FolderManager.Instance.ListPathFindFolderName(GroupName).Count; }

        /// <summary>
        /// ユーザーの数
        /// </summary>
        public int UserCount { get; private set; } = 0;


        /// <summary>
        /// 最大のネスト数の取得
        /// </summary>
        /// <param name="list_">パス一覧</param>
        /// <returns>最大のネスト数</returns>

        private int maxNestCount(IList<string> list_)
        {
            return list_.Max(path => (path.Length - path.Replace(@"\", "").Length));
        }
    }
}
