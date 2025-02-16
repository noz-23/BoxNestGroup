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
            // XML で読み込み時に利用
        }
        
        public XmlGroupTreeView(string groupName_, XmlGroupTreeView? parent_) :this(groupName_,string.Empty, parent_)
        {
        }

        public XmlGroupTreeView(string groupName_, string groupId_, XmlGroupTreeView? parent_) : this()
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
        public XmlGroupTreeView? Parent { get; set; } = null;

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

            return ListChild?.ToList()?.Find(v_=> v_.ContainsName(groupName_))!=null;
        }

        /// <summary>
        /// グループIDが含まれているか
        /// </summary>
        /// <param name="groupId_"></param>
        /// <returns></returns>
        public bool ContainsId(string groupId_)
        {
            if (GroupId == groupId_)
            {
                return true;
            }

            return ListChild?.ToList()?.Find(view => view.ContainsId(groupId_)) != null;
        }

        /// <summary>
        /// グループViewが含まれているか
        /// </summary>
        /// <param name="groupView_"></param>
        /// <returns></returns>
        public bool ContainsView(XmlGroupTreeView? groupView_)
        {
            if (groupView_ ==null)
            {
                return false;
            }
            if (groupView_ == this)
            {
                return true;
            }
            return ListChild?.ToList()?.Find(view => view.ContainsView(groupView_)) != null;
        }

        /// <summary>
        /// 親リストのグループ名リスト
        /// </summary>
        /// <returns></returns>
        public ICollection<string> ListAllParentGroupName()
        {
            var rtn = new HashSet<string>();
            if (Parent != null)
            {
                rtn.Add(Parent.GroupName);
                rtn.UnionWith(Parent.ListAllParentGroupName());
            }
            return rtn;
        }

        /// <summary>
        /// XML利用時に親を設定するため
        /// </summary>
        /// <param name="parent_"></param>
        public void SetParent(XmlGroupTreeView parent_)
        {
            ListChild?.ToList()?.ForEach(view_ => 
            {
                view_.Parent = parent_ ;
              view_.SetParent(this);
          
            });
        }
    }
}
