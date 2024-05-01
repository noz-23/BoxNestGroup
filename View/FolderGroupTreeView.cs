using System.Collections.ObjectModel;

namespace BoxNestGroup.View
{
    /// <summary>
    /// フォルダーツリーのデータ構造
    /// </summary>
    public class FolderGroupTreeView
    {
        /// <summary>
        /// グループ名(フォルダ名)
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// ツリービュー表示用
        /// </summary>
        public List<FolderGroupTreeView> ListViewGroup { get { return ListNestGroup.ToList(); } }

        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        public ObservableCollection<FolderGroupTreeView> ListNestGroup = new ObservableCollection<FolderGroupTreeView>();

    }
}
