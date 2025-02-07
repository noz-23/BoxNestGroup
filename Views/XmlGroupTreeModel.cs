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
        /// �R���X�g���N�^
        /// </summary>
        public XmlGroupTreeModel()
        {
        }
        ~XmlGroupTreeModel()
        {
            //Save(_fileName);
        }
        /// <summary>
        /// �ۑ��t�@�C������
        /// </summary>
        private string _fileName =Directory.GetCurrentDirectory()+ @"\XmlGroupTree.xml";

        /// <summary>
        /// �q���܂߂��O���[�v���̌���(�����Ă��邩)
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
        /// �q���܂߂��O���[�vID�̌���(�����Ă��邩)
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
        /// �q���܂߂�View�̌���(�����Ă��邩)
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
        /// ���O�̍X�V
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
        /// �t�H���_(�O���[�v)�����܂񂾃p�X���X�g(�l�X�g)�ꗗ
        /// </summary>
        /// <param name="groupName_">�t�H���_(�O���[�v)��</param>
        /// <returns>�p�X���X�g</returns>
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
        /// ���O�̃J�E���g
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
        /// �S�t�H���_(�O���[�v)���ꗗ����l�X�g���폜�����O���[�v�̎擾
        /// </summary>
        /// <param name="listGroupName_">�S�t�H���_(�O���[�v)���ꗗ</param>
        /// <returns>�ŏ��̃t�H���_(�O���[�v)���ꗗ</returns>
        public IList<string> ListMinimumGroup(List<string> listGroupName_)
        {
            var listNest = new HashSet<string>();
            var rtn = new HashSet<string>(listGroupName_);

            // �l�X�g���Ă���t�H���_�̍폜
            var listView = new List<XmlGroupTreeView>();
            listGroupName_.ForEach(groupName => listView.AddRange(FindAllGroupName(groupName)));

            // ���ׂĂ̐e�̖��O���擾
            var listParent = new HashSet<string>();
            listView.ForEach(view => listParent.UnionWith(view.ListAllParentGroupName()));

            // �e�̖��O���폜
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
        /// �O���[�v���������Ă�View�̂��ׂĂ̎擾
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
        /// �O���[�vID�������Ă�View�̂��ׂĂ̎擾
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
                LogFile.Instance.WriteLine($"��xml {xml?.Count}");

                //var listPropert = typeof(XmlGroupTreeModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                //foreach (var p in listPropert)
                //{
                //    try
                //    {
                //        typeof(XmlGroupTreeModel)?.GetProperty(p.Name)?.SetValue(this, p.GetValue(xml));
                //    }
                //    catch (Exception ex)
                //    {
                //        LogFile.Instance.WriteLine("��Open Error: {0}", ex.Message);
                //    }
                //}
                xml?.ToList().ForEach(x_ => this.Add(x_));
                sr.Close();
            }
            LogFile.Instance.WriteLine($"��this {this?.Count}");

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