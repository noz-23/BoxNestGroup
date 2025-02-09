using System.Collections.ObjectModel;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    public class GroupDataGridModel : ObservableCollection<GroupDataGridView>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupDataGridModel()
        { 
        }

        /// <summary>
        /// グループIDからグループ名の取得
        /// </summary>
        /// <param name="groupId_">グループID</param>
        /// <returns>グループ名</returns>
        public string GetBoxGroupName(string groupId_)
        {
            return this.ToList().Find((d) => (d.GroupId == groupId_))?.GroupName??string.Empty;
        }

        /// <summary>
        /// グループ名からグループIDの取得
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>グループID</returns>
        public string GetBoxGroupId(string groupName_)
        {
            return this.ToList().Find((d) => (d.GroupName == groupName_))?.GroupId??string.Empty;
        }

        /// <summary>
        /// ネストをして増やしたグループ名一覧
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>グループ名一覧</returns>
        public IList<string> ListNestGroupId(IList<string> listGroupName_)
        {
            var rtn = new List<string>();

            listGroupName_?.ToList().ForEach(groupName_ =>
            {
                var find = this?.ToList().Find((g) => (g.GroupName == groupName_));
                if (find == null)
                {
                    return;
                }
                rtn.Add(find.GroupId);
            });

            return rtn;
        }

    }
}
