using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.View
{
    /// <summary>
    /// フォルダーツリーのデータ構造
    /// </summary>
    public class FolderGroupTreeView : INotifyPropertyChanged
    {
        private DirectoryInfo? _info =null;
        /// <summary>
        /// グループ名(フォルダ名)
        /// </summary>
        public string GroupName
        {
            get { return _info?.Name ?? string.Empty; }
            set
            {
                string newPath = _info?.Parent?.FullName + @"\" + value;
                _info?.MoveTo(newPath);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// ツリービュー表示用
        /// </summary>
        //public List<FolderGroupTreeView> ListViewGroup { get { return ListNestGroup.ToList(); } }

        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        public ObservableCollection<FolderGroupTreeView> ListNestGroup { get; private set; }= new ObservableCollection<FolderGroupTreeView>();


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        public FolderGroupTreeView()
        {
            _info = null;
        }

        public FolderGroupTreeView(DirectoryInfo info_) :this()
        {
            _info = info_;

            foreach (var info in info_.GetDirectories())
            {
                ListNestGroup.Add(new FolderGroupTreeView(info));
            }
        }

        public bool Contains(string name_)
        {
            if (GroupName == name_)
            {
                return true;
            }

            foreach (var view in ListNestGroup)
            {
                if (view.Contains(name_) == true)
                {
                    return true;
                }
            }
            return false;
        }

        public IList<FolderGroupTreeView> Find(string name_)
        {
            var list = new List<FolderGroupTreeView>();
            if (GroupName == name_)
            {
                list.Add(this);
            }
            foreach (var view in ListNestGroup)
            {
                list.AddRange(view.Find(name_));
            }

            return list;
        }
    }
}
