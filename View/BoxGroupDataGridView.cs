using Box.V2.Models;
using BoxNestGroup.Manager;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.View
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    internal class BoxGroupDataGridView : INotifyPropertyChanged
    {
        /// <summary>
        /// グループ名
        /// </summary>
        public string NowGroupName { get; set; } = string.Empty;


        public string ModGroupName { get; set; } = string.Empty;

        /// <summary>
        /// グループID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;
        /// <summary>
        /// 最大のネスト最大数(基本並び替え用)
        /// </summary>
        public int MaxNestCount { get; private set; } =0;

        /// <summary>
        /// フォルダの数
        /// </summary>
        public int FolderCount { get; private set; } = 0;

        /// <summary>
        /// ユーザーの数
        /// </summary>
        public int UserCount { get; private set; } = 0;

        /// <summary>
        /// Boxのグループ(オンライン時)
        /// </summary>
        private BoxGroup? _groupBox =null;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoxGroupDataGridView()
        {
        }

        /// <summary>
        /// オフラインや新規作成時のコンストラクタ
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        public BoxGroupDataGridView(string groupName_)
        {
            NowGroupName = groupName_;
            _inital();
        }

        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public BoxGroupDataGridView(BoxGroup group_)
        {
            _groupBox = group_;

            NowGroupName = group_.Name;
            GroupId = group_.Id;

            // Boxから取得
            UserCount = SettingManager.Instance.ListBoxGroupMembership.CountBoxGroupMemberShip(GroupId);
            _inital();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        //public async Task Inital()
        private void _inital()
        {
            // 設定関係を優先
            if (GroupId != string.Empty)
            {
                SettingManager.Instance.CheckFolderName(_groupBox);
            }
            else 
            {
                // データ(ファイル)から取得
                GroupId = SettingManager.Instance.GetGroupIdFromSttingData(NowGroupName);

                UserCount = SettingManager.Instance.CountGroupMemberShipFromSettingData(NowGroupName);

                FolderManager.Instance.CreateFolder(NowGroupName);
                //if (FolderManager.Instance.Contains(GroupName) == false)
                //{
                //    // フォルダがない場合は作成
                //    FolderManager.Instance.CreateFolder(GroupName);
                //}
            }

            var list = FolderManager.Instance.ListPathFindFolderName(NowGroupName);
            FolderCount = list.Count;

            MaxNestCount = maxNestCount(list);
        }

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
