using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BoxNestGroup.Views
{
    [XmlRoot]
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

        public XmlGroupTreeView(string groupName_, string groupId_, XmlGroupTreeView parent_) : this()
        {
            GroupName = groupName_;
            GroupId = groupId_;
            Parent = parent_;
        }

        /// <summary>
        /// グループ名(フォルダ名)
        /// </summary>
        [XmlElement]
        public string GroupName {get;set;}=string.Empty;

        [XmlAttribute]
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// グループ選択チェック状態
        /// </summary>
        [XmlIgnore]
        public bool Checked { get; set; } = false;

        /// <summary>
        /// ツリービュー表示用
        /// </summary>
        [XmlIgnore]
        public XmlGroupTreeView Parent { get; set; } = null;

        /// <summary>
        /// サブフォルダリスト
        /// </summary>
        [XmlArray]
        public XmlGroupTreeModel ListChild { get; private set; }= new XmlGroupTreeModel();

        /// <summary>
        /// グループ名が含まれているか
        /// </summary>
        /// <param name="groupName_"></param>
        /// <returns></returns>
        public bool ContainsName(string groupName_)
        {
            if (GroupName == groupName_)
            {
                return true;
            }

            return ListChild?.ToList()?.Find(view=> view.ContainsName(groupName_))!=null;
        }


        public bool ContainsId(string groupId_)
        {
            if (GroupId == groupId_)
            {
                return true;
            }

            return ListChild?.ToList()?.Find(view => view.ContainsId(groupId_)) != null;
        }

        public bool ContainsView(XmlGroupTreeView groupView_)
        {
            if (this == groupView_)
            {
                return true;
            }
            return ListChild?.ToList()?.Find(view => view.ContainsView(groupView_)) != null;
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

        public IList<string> ListAllParentGroupName()
        {
            var rtn = new List<string>();
            if (Parent != null)
            {
                rtn.Add(Parent.GroupName);
                rtn.AddRange(Parent.ListAllParentGroupName());
            }
            return rtn;
        }
    }
}
