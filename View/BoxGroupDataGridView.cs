using Box.V2.Models;
using BoxNestGroup.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.GridView
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    internal class BoxGroupDataGridView
    {
        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;
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
        private BoxGroup _groupBox =null;

        public BoxGroupDataGridView()
        {
        }

        /// <summary>
        /// オフラインや新規作成時のコンストラクタ
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        public BoxGroupDataGridView(string groupName_)
        {
            GroupName = groupName_;
        }

        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public BoxGroupDataGridView(BoxGroup group_)
        {
            _groupBox = group_;

            GroupName = group_.Name;
            GroupId = group_.Id;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public async Task Inital()
        {
            // 設定関係を優先
            if (GroupId != string.Empty)
            {
                // Boxから取得
                await SettingManager.Instance.AddBoxGroupMemberShip(GroupId);
                UserCount = SettingManager.Instance.CountBoxGroupMemberShip(GroupId);

                SettingManager.Instance.CheckFolderName(_groupBox);
            }
            else 
            {
                // データ(ファイル)から取得
                GroupId = SettingManager.Instance.GetGroupIdFromSttingData(GroupName);

                UserCount = SettingManager.Instance.CountGroupMemberShipFromSettingData(GroupName);

                //
                SettingManager.Instance.CheckFolderName(GroupName);
            }

            var list = FolderManager.Instance.ListPathFindFolderName(GroupName);
            FolderCount = list.Count;

            MaxNestCount = maxNestCount(list);           
        }
        private int maxNestCount(List<string> list_)
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
