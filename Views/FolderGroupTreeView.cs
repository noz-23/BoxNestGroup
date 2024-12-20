using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// フォルダーツリーのデータ構造
    /// </summary>
    public class FolderGroupTreeView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

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
        public bool Checked { get; set; } = false;
        public FolderGroupTreeView Parent { get; private set; } = null;

        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        public ObservableCollection<FolderGroupTreeView> ListChild { get; private set; }= new ObservableCollection<FolderGroupTreeView>();

        public FolderGroupTreeView()
        {
            _info = null;
        }

        public FolderGroupTreeView(DirectoryInfo info_, FolderGroupTreeView parent_) :this()
        {
            _info = info_;
            Parent = parent_;

            info_.GetDirectories().ToList().ForEach(info => ListChild.Add(new FolderGroupTreeView(info, this)));
        }

        public bool Contains(string name_)
        {
            if (GroupName == name_)
            {
                return true;
            }

            return ListChild?.ToList()?.Find(view=> view.Contains(name_))!=null;

        }

        public IList<FolderGroupTreeView> Find(string name_)
        {
            var list = new List<FolderGroupTreeView>();
            if (GroupName == name_)
            {
                list.Add(this);
            }
            foreach (var view in ListChild)
            {
                list.AddRange(view.Find(name_));
            }

            return list;
        }
    }
}
