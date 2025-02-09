using BoxNestGroup.Files;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;


namespace BoxNestGroup.Views
{

    [XmlRoot]
    public class XmlGroupTreeModel : ObservableCollection<XmlGroupTreeView>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public XmlGroupTreeModel()
        {
        }

        ~XmlGroupTreeModel()
        {
            //Save(_fileName);
        }
        /// <summary>
        /// 保存ファイル処理
        /// </summary>
        private string _fileName =Directory.GetCurrentDirectory()+ @"\XmlGroupTree.xml";

        /// <summary>
        /// 子も含めたグループ名の検索(持っているか)
        /// </summary>
        /// <param name="groupName_"></param>
        /// <returns></returns>
        public bool ContainsName(string groupName_)
        {
            bool rtn = false;

            this?.ToList().ForEach(x => rtn |= x.ContainsName(groupName_));

            return rtn;
        }

        /// <summary>
        /// 子も含めたグループIDの検索(持っているか)
        /// </summary>
        /// <param name="groupId_"></param>
        /// <returns></returns>
        public bool ContainsId(string groupId_)
        {
            if (string.IsNullOrEmpty(groupId_) == true)
            {
                return true;
            }

            bool rtn = false;

            this?.ToList().ForEach(x => rtn |= x.ContainsId(groupId_));

            return rtn;
        }

        /// <summary>
        /// 子も含めたViewの検索(持っているか)
        /// </summary>
        /// <param name="group_"></param>
        /// <returns></returns>
        public bool ContainsView(XmlGroupTreeView group_)
        {
            bool rtn = false;

            this?.ToList().ForEach(x => rtn |= x.ContainsView(group_));

            return rtn;
        }

        /// <summary>
        /// 名前の更新
        /// </summary>
        /// <param name="oldName_"></param>
        /// <param name="newName_"></param>
        public void UpdateGroupName(string oldName_, string newName_)
        {
            LogFile.Instance.WriteLine($"[{oldName_}] -> [{newName_}]");
            var list = new List<XmlGroupTreeView>();

            this?.ToList().ForEach(view_ => 
            {
                if (view_.GroupName == oldName_)
                {
                    view_.GroupName = newName_;
                }

                view_.ListChild.UpdateGroupName(oldName_, newName_);
            });
        }

        /// <summary>
        /// フォルダ(グループ)名を含んだパスリスト(ネスト)一覧
        /// </summary>
        /// <param name="groupName_">フォルダ(グループ)名</param>
        /// <returns>パスリスト</returns>
        public int MaxNestCount(string groupName_ ,int nest_=0)
        {
            int rtn = 0;
            this?.ToList().ForEach(view_ =>
            {
                if (view_.GroupName == groupName_)
                {
                    rtn= Math.Max(rtn, nest_ + 1);
                }

                rtn = Math.Max(rtn, view_.ListChild.MaxNestCount(groupName_, nest_ + 1));
            });


            return rtn;
        }

        /// <summary>
        /// 名前のカウント
        /// </summary>
        /// <param name="groupName_"></param>
        /// <param name="count_"></param>
        /// <returns></returns>
        public int NameCount(string groupName_, int count_ =0)
        {
            this?.ToList().ForEach(view_ =>
            {
                if (view_.GroupName == groupName_)
                {
                    count_++;
                }
                count_ = view_.ListChild.NameCount(groupName_, count_);
            });

            return count_;
        }

        /// <summary>
        /// 全フォルダ(グループ)名一覧からネストを削除したグループの取得
        /// </summary>
        /// <param name="listGroupName_">全フォルダ(グループ)名一覧</param>
        /// <returns>最小のフォルダ(グループ)名一覧</returns>
        public IList<string> ListMinimumGroup(List<string> listGroupName_)
        {
            var listNest = new HashSet<string>();
            var rtn = new HashSet<string>(listGroupName_);

            // ネストしているフォルダの削除
            var listView = new List<XmlGroupTreeView>();
            listGroupName_.ForEach(groupName_ => listView.AddRange(FindAllGroupName(groupName_)));

            // すべての親の名前を取得
            var listParent = new HashSet<string>();
            listView.ForEach(view => listParent.UnionWith(view.ListAllParentGroupName()));

            // 親の名前を削除
            listParent?.ToList().ForEach(parent_ => rtn.Remove(parent_));

            rtn.Remove(string.Empty);

            return rtn.ToList();
        }

        /// <summary>
        /// ネストで含まれるフォルダ(グループ)名の取得
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>全フォルダ(グループ)名一覧</returns>
        public List<string> ListUniqueGroup(List<string> listGroupName_)
        {
            var listView =new List<XmlGroupTreeView>();

            listGroupName_.ForEach(groupName=> listView.AddRange(FindAllGroupName(groupName)));

            var rtn = new HashSet<string>(listGroupName_);
            listView.ForEach(view_ => rtn.UnionWith(view_.ListAllParentGroupName()));

            rtn.Remove(string.Empty);
            return rtn.ToList();
        }

        /// <summary>
        /// グループ名を持ってるViewのすべての取得
        /// </summary>
        /// <param name="rtn_"></param>
        /// <param name="groupName_"></param>
        public IList<XmlGroupTreeView> FindAllGroupName( string groupName_)
        {
            var rtn = new List<XmlGroupTreeView>();

            this?.ToList().ForEach(view =>
            {
                if (view.GroupName == groupName_)
                {
                    rtn.Add(view);
                }
                rtn.AddRange(view.ListChild.FindAllGroupName(groupName_));
            });
            return rtn;
        }

        /// <summary>
        /// グループIDを持ってるViewのすべての取得
        /// </summary>
        /// <param name="groupId_"></param>
        /// <returns></returns>
        public IList<XmlGroupTreeView> FindAllGroupId(string groupId_)
        {
            var rtn =new List<XmlGroupTreeView>();
            if (string.IsNullOrEmpty(groupId_) == true)
            {
                return rtn;
            }

            this?.ToList().ForEach(view_ =>
            {
                if (view_.GroupId == groupId_)
                {
                    rtn.Add(view_);
                }
                rtn.AddRange(view_.ListChild.FindAllGroupId(groupId_));
            });
            return rtn;
        }

        /// <summary>
        /// グループ名からグループIDを取得
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns></returns>
        public string FindGroupId(string groupName_)
        {
            var rtn =FindAllGroupName(groupName_);

            return (rtn.Count > 0) ? (rtn[0].GroupId ): string.Empty;
        }


        /// <summary>
        /// Xml ファイルの読み込み
        /// </summary>
        public void Open()
        {
            Open(_fileName);
        }

        /// <summary>
        /// Xml ファイルの読み込み
        /// </summary>
        /// <param name="path_">ファイルパス</param>
        public void Open(string path_)
        {
            if (File.Exists(path_) == false)
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(XmlGroupTreeModel));
            using (var sr = new StreamReader(path_, Encoding.UTF8))
            {
                var xml = serializer.Deserialize(sr) as XmlGroupTreeModel;
                LogFile.Instance.WriteLine($"[{xml?.Count}]");

                xml?.ToList().ForEach(x_ => this.Add(x_));
                sr.Close();
            }
        }

        /// <summary>
        /// Xml ファイルの保存
        /// </summary>
        public void Save()
        {
            Save(_fileName);
        }

        /// <summary>
        /// Xml ファイルの保存
        /// </summary>
        /// <param name="path_">ファイルパス</param>
        public void Save(string path_)
        {
            var serializer = new XmlSerializer(typeof(XmlGroupTreeModel));
            using (var sw = new StreamWriter(path_, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, this);
                sw.Close();
            }
        }

    }
}