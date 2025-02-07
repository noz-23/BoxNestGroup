using BoxNestGroup.Files;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            this.ToList().ForEach(x => rtn |= x.ContainsName(groupName_));

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

            this.ToList().ForEach(x => rtn |= x.ContainsId(groupId_));

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

            this.ToList().ForEach(x => rtn |= x.ContainsView(group_));

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

            foreach (var view in this)
            {
                if (view.GroupName == oldName_)
                {
                    view.GroupName = newName_;
                }

                view.ListChild.UpdateGroupName(oldName_, newName_);
            }
        }

        /// <summary>
        /// フォルダ(グループ)名を含んだパスリスト(ネスト)一覧
        /// </summary>
        /// <param name="groupName_">フォルダ(グループ)名</param>
        /// <returns>パスリスト</returns>
        public int MaxNestCount(string groupName_ ,int nest_=0)
        {
            int rtn = 0;
            foreach (var view in this)
            {
                if (view.GroupName == groupName_)
                {
                    return Math.Max(rtn,nest_ + 1);
                }

                rtn =Math.Max(rtn,view.ListChild.MaxNestCount(groupName_, nest_ + 1));
            }

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
            foreach (var view in this)
            {
                if (view.GroupName == groupName_)
                {
                    count_++;
                }
                count_ = view.ListChild.NameCount(groupName_, count_);
            }

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
            listGroupName_.ForEach(groupName => listView.AddRange(FindAllGroupName(groupName)));

            // すべての親の名前を取得
            var listParent = new HashSet<string>();
            listView.ForEach(view => listParent.UnionWith(view.ListAllParentGroupName()));

            // 親の名前を削除
            foreach (var del in listParent)
            {
                rtn.Remove(del);
            }

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
            listView.ForEach(view => rtn.UnionWith(view.ListAllParentGroupName()));

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
            foreach (var view in this)
            {
                if (view.GroupName == groupName_)
                {
                    rtn.Add(view);
                }
                rtn.AddRange(view.ListChild.FindAllGroupName(groupName_));
            }
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
            foreach (var view in this)
            {
                if (view.GroupId == groupId_)
                {
                    rtn.Add(view);
                }
                rtn.AddRange(view.ListChild.FindAllGroupId( groupId_));
            }
            return rtn;
        }

        public string FindGroupId(string groupName_)
        {
            var rtn =FindAllGroupName(groupName_);

            return (rtn.Count > 0) ? (rtn[0].GroupId ): string.Empty;
        }


        public void Open()
        {
            Open(_fileName);
        }   

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
                LogFile.Instance.WriteLine($"■xml {xml?.Count}");

                //var listPropert = typeof(XmlGroupTreeModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                //foreach (var p in listPropert)
                //{
                //    try
                //    {
                //        typeof(XmlGroupTreeModel)?.GetProperty(p.Name)?.SetValue(this, p.GetValue(xml));
                //    }
                //    catch (Exception ex)
                //    {
                //        LogFile.Instance.WriteLine("■Open Error: {0}", ex.Message);
                //    }
                //}
                xml?.ToList().ForEach(x_ => this.Add(x_));
                sr.Close();
            }
            LogFile.Instance.WriteLine($"■this {this?.Count}");

        }

        public void Save()
        {
            Save(_fileName);
        }

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