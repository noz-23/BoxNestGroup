using DocumentFormat.OpenXml.Drawing.Charts;
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

        private string _fileName =Directory.GetCurrentDirectory()+ @"\XmlGroupTree.xml";

        public bool ContainsName(string groupName_)
        {
            bool rtn = false;

            this.ToList().ForEach(x => rtn |= x.ContainsName(groupName_));

            return rtn;
        }

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

        public bool ContainsView(XmlGroupTreeView group_)
        {
            bool rtn = false;

            this.ToList().ForEach(x => rtn |= x.ContainsView(group_));

            return rtn;
        }


        public void UpdateGroupName(string oldName_, string newName_)
        {
            Debug.WriteLine("■UpdateGroupName old [{0}] new [{1}]", oldName_, newName_);
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
        public int MaxNestCount(string groupName_ ,int nest_)
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

        public int NameCount(string groupName_, int count_)
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
        public IList<string> ListMinimumGroup(ICollection<string> listGroupName_)
        {
            var listNest = new HashSet<string>();
            var rtn = new HashSet<string>(listGroupName_);

            // ネストしているフォルダの削除
            var listView = new List<XmlGroupTreeView>();
            foreach (var groupName in listGroupName_)
            {
                _findAllGroupName(listView, groupName);
            }

            var listParent = new HashSet<string>();
            foreach (var view in listView)
            {
                view.ListAllParentGroupName(listParent);
            }

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
        public List<string> ListUniqueGroup(ICollection<string> listGroupName_)
        {

            var listView =new List<XmlGroupTreeView>();
            foreach (var group in listGroupName_)
            {
                _findAllGroupName(listView, group);
            }

            var rtn = new HashSet<string>(listGroupName_);
            foreach (var view in listView)
            {
                view.ListAllParentGroupName(rtn);
            }

            rtn.Remove(string.Empty);
            return rtn.ToList();
        }

        private void _findAllGroupName(ICollection<XmlGroupTreeView> rtn_, string groupName_)
        {
            foreach (var view in this)
            {
                if (view.GroupName == groupName_)
                {
                    rtn_.Add(view);
                }
                view.ListChild._findAllGroupName(rtn_, groupName_);
            }
        }


        public IList<XmlGroupTreeView> FindAllGroupId(string groupId_)
        {
            var rtn = new List<XmlGroupTreeView>();
            _findAllGroupId(rtn, groupId_);
            return rtn;
        }

        private void _findAllGroupId(ICollection<XmlGroupTreeView> rtn_, string groupId_)
        {
            if (string.IsNullOrEmpty(groupId_) == true)
            {
                return;
            }
            foreach (var view in this)
            {
                if (view.GroupId == groupId_)
                {
                    rtn_.Add(view);
                }
                view.ListChild._findAllGroupId(rtn_, groupId_);
            }
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