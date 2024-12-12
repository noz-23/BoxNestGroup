using Box.V2.Models;
using BoxNestGroup.Managers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    public class GroupDataGridView : INotifyPropertyChanged
    {
        public const string OFFLINE_GROUP_ID = "[OFFLINE]";

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
        /// Boxのグループ(オンライン時)
        /// </summary>
        //private BoxGroup? _groupBox =null;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupDataGridView()
        {
        }

        /// <summary>
        /// オフラインや新規作成時のコンストラクタ
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        public GroupDataGridView(string groupName_)
        {
            GroupName = groupName_;
            OldGroupName = groupName_;
            GroupId = OFFLINE_GROUP_ID;

            UserCount = SettingManager.Instance.ListMembershipGroupNameMail.CountBoxUserInGroupName(GroupName);

            //_inital();
            //
            string name = string.Empty;

            // 設定がない場合は作る
            if (FolderManager.Instance.Contains(GroupName) == false)
            {
                FolderManager.Instance.CreateFolder(GroupName);
            }

            //_inital();
            //var list = FolderManager.Instance.ListPathFindFolderName(GroupName);
            //FolderCount = list.Count;
            //MaxNestCount = maxNestCount(list);
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
            UserCount = SettingManager.Instance.ListMembershipGroupNameMail.CountBoxUserInGroupName(GroupName);

            //
            string name = string.Empty;
            if (SettingManager.Instance.ListGroupIdName.TryGetValue(GroupId, out name) == true)
            {
                // 設定と現状の名前が違う場合
                if (GroupName != name)
                {
                    if (FolderManager.Instance.Contains(name) == true)
                    {
                        FolderManager.Instance.UpdateGroupName(name,GroupName);
                    }
                }
            }
            // 設定がない場合は作る
            if (FolderManager.Instance.Contains(GroupName) == false)
            {
                FolderManager.Instance.CreateFolder(GroupName);
            }
            SettingManager.Instance.ListGroupIdName[GroupId] = GroupName;

            //_inital();
            //var list = FolderManager.Instance.ListPathFindFolderName(GroupName);
            //FolderCount = list.Count;
            //MaxNestCount = maxNestCount(list);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        //public async Task Inital()
        //private void _inital()
        //{
        //    // 設定関係を優先
        //    if (GroupId != string.Empty)
        //    {
        //        SettingManager.Instance.CheckFolderName(_groupBox);
        //    }
        //    else 
        //    {
        //        // データ(ファイル)から取得
        //        GroupId = SettingManager.Instance.GetGroupIdFromSttingData(NowGroupName);

        //        UserCount = SettingManager.Instance.CountGroupMemberShipFromSettingData(NowGroupName);

        //        FolderManager.Instance.CreateFolder(NowGroupName);
        //        //if (FolderManager.Instance.Contains(GroupName) == false)
        //        //{
        //        //    // フォルダがない場合は作成
        //        //    FolderManager.Instance.CreateFolder(GroupName);
        //        //}
        //    }

        //    var list = FolderManager.Instance.ListPathFindFolderName(NowGroupName);
        //    FolderCount = list.Count;

        //    MaxNestCount = maxNestCount(list);
        //}

        /// <summary>
        /// 最大のネスト数の取得
        /// </summary>
        /// <param name="list_">パス一覧</param>
        /// <returns>最大のネスト数</returns>

        private int maxNestCount(IList<string> list_)
        {
            int count = 1;
            foreach (var path in list_)
            {
                count = Math.Max(count, path.Length - path.Replace(@"\", "").Length);
            }
            return count;
        }
    }
}
