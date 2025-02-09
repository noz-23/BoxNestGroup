using BoxNestGroup.Files;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループ作成用ビュー
    /// </summary>
    public class MakeGroupView: BaseView
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="groupView_">元のグループビュー</param>
        public MakeGroupView(GroupDataGridView groupView_) : base()
        {
            LogFile.Instance.WriteLine($"{groupView_.GroupName} {groupView_.GroupId}");

            IsChecked = false;
            GroupName = groupView_.GroupName;
            GroupId = groupView_.GroupId;
        }

        /// <summary>
        /// 選択したか
        /// </summary>
        public bool IsChecked { get; set; } = false; 

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// グループID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

    }
}
