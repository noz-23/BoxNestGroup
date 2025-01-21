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
    public class XmlGroupTreeModel : ObservableCollection<XmlGroupTreeView>
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public XmlGroupTreeModel()
        {
            if (File.Exists(_fileName) ==true)
            {
                Open(_fileName);
            }
        }
        ~XmlGroupTreeModel()
        {

            Save(_fileName);
        }

        private string _fileName = "XmlGroupTree.xml";

        public bool Contains(string groupName_)
        {
            bool rtn = false;

            this.ToList().ForEach(x => rtn |= x.Contains(groupName_));

            return rtn;
        }

        public bool ContainsView(XmlGroupTreeView group_)
        {
            bool rtn = false;

            this.ToList().ForEach(x => rtn |= x.Contains(group_));

            return rtn;
        }


        public void UpdateGroupName(string oldName_, string newName_)
        {
            Debug.WriteLine("��UpdateGroupName old [{0}] new [{1}]", oldName_, newName_);
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
        /// �t�H���_(�O���[�v)�����܂񂾃p�X���X�g(�l�X�g)�ꗗ
        /// </summary>
        /// <param name="groupName_">�t�H���_(�O���[�v)��</param>
        /// <returns>�p�X���X�g</returns>
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
        /// �S�t�H���_(�O���[�v)���ꗗ����l�X�g���폜�����O���[�v�̎擾
        /// </summary>
        /// <param name="listGroupName_">�S�t�H���_(�O���[�v)���ꗗ</param>
        /// <returns>�ŏ��̃t�H���_(�O���[�v)���ꗗ</returns>
        public IList<string> ListMinimumGroup(ICollection<string> listGroupName_)
        {
            var listNest = new HashSet<string>();
            var rtn = new HashSet<string>(listGroupName_);

            // �l�X�g���Ă���t�H���_�̍폜
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
        /// �l�X�g�Ŋ܂܂��t�H���_(�O���[�v)���̎擾
        /// </summary>
        /// <param name="listGroupName_">�l�X�g�O�̃O���[�v���ꗗ</param>
        /// <returns>�S�t�H���_(�O���[�v)���ꗗ</returns>
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

        public void Open(string path_)
        {
            var serializer = new XmlSerializer(typeof(XmlGroupTreeModel));
            using (var sr = new StreamReader(path_, Encoding.UTF8))
            {
                var xml = serializer.Deserialize(sr) as XmlGroupTreeModel;

                var listPropert = typeof(XmlGroupTreeModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var p in listPropert)
                {
                    typeof(XmlGroupTreeModel)?.GetProperty(p.Name)?.SetValue(this, p.GetValue(xml));
                }
            }
        }

        public void Save(string path_)
        {
            var serializer = new XmlSerializer(typeof(XmlGroupTreeModel));
            using (var sw = new StreamWriter(path_, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, this);
            }
        }

    }
}