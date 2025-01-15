using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class XmlGroupTreeView
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private XmlGroupTreeView()
        {
        }
        
        public XmlGroupTreeView(string groupName_, XmlGroupTreeView parent_) :this()
        {
            GroupName = groupName_;
            Parent = parent_;
        }        

        /// <summary>
        /// グループ名(フォルダ名)
        /// </summary>
        public string GroupName {get;set;}=string.Empty;

        /// <summary>
        /// グループ選択チェック状態
        /// </summary>
        public bool Checked { get; set; } = false;

        /// <summary>
        /// ツリービュー表示用
        /// </summary>
        public XmlGroupTreeView Parent { get; private set; } = null;

        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        public XmlGroupTreeModel ListChild { get; private set; }= new XmlGroupTreeModel();

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

        //public IList<XmlGroupTreeView> Find(string groupName_)
        //{
        //    var list = new List<XmlGroupTreeView>();
        //    if (GroupName == groupName_)
        //    {
        //        list.Add(this);
        //    }
        //    foreach (var view in ListChild)
        //    {
        //        list.AddRange(view.Find(groupName_));
        //    }

        //    return list;
        //}

        public void ListAllParentGroupName(ICollection<string> rtn_)
        {
            if (Parent != null)
            {
                rtn_.Add(Parent.GroupName);
                Parent.ListAllParentGroupName(rtn_);
            }

        }
    }
}
