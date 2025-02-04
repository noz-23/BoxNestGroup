﻿using DocumentFormat.OpenXml.EMMA;
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
    public class FolderGroupTreeView : BaseView
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private FolderGroupTreeView()
        {
            _info = null;
        }

        private DirectoryInfo? _info =null;
        /// <summary>
        /// グループ名(フォルダ名)
        /// </summary>
        public string GroupName
        {
            get => _info?.Name ?? string.Empty; 
            set
            {
                string newPath = _info?.Parent?.FullName + @"\" + value;
                _info?.MoveTo(newPath);

                string tmp=string.Empty;
                _SetValue(ref tmp,newPath);
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

        public FolderGroupTreeView(DirectoryInfo info_, FolderGroupTreeView parent_) :this()
        {
            _info = info_;
            Parent = parent_;

            info_.GetDirectories().ToList().ForEach(info => ListChild.Add(new FolderGroupTreeView(info, this)));
        }
        /// <summary>
        /// グループ名が含まれているか
        /// </summary>
        /// <param name="groupName_"></param>
        /// <returns></returns>
        public bool Contains(string groupName_)
        {
            if (GroupName == groupName_)
            {
                return true;
            }

            return ListChild?.ToList()?.Find(view=> view.Contains(groupName_))!=null;

        }

        public IList<FolderGroupTreeView> Find(string groupName_)
        {
            var list = new List<FolderGroupTreeView>();
            if (GroupName == groupName_)
            {
                list.Add(this);
            }
            foreach (var view in ListChild)
            {
                list.AddRange(view.Find(groupName_));
            }

            return list;
        }
    }
}
