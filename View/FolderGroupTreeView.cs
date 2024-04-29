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

        public override string ToString()
        {
            return GroupName;
        }


        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        public ObservableCollection<FolderGroupTreeView> ListNestGroup { get; set; } = new ObservableCollection<FolderGroupTreeView>();

    }
}
