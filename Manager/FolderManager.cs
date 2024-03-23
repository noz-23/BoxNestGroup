using Box.V2.Models;
using BoxNestGroup.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BoxNestGroup.Manager
{
    class FolderManager
    {
        static public FolderManager Instance { get; } = new FolderManager();

        private string _commonGroupFolderPath = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupFolder;

        //private HashSet<string> _listFolder =new HashSet<string>();
        /*
         * リストフォルダ
         *  引数　：なし
         *  戻り値：なし
         */
        public HashSet<string> ListFolderName { get; private set; } =new HashSet<string>();

        private FolderManager()
        {
        }

        public bool IsHaveGroup(string name_)
        {
            return ListFolderName.Contains(name_);
        }

        /*
         * フォルダ作成
         *  引数　：name_ フォルダ名
         *  戻り値：なし
         */

        public void CreateFolder(BoxGroup boxGroup_)
        {
            var pathName = _commonGroupFolderPath + @"\" + boxGroup_.Name;

            if (Directory.Exists(pathName) == false)
            {
                Directory.CreateDirectory(pathName);
            }
        }

        public void UpdateFolder(string oldName_, string newName_)
        {
            var list = ListPathFindFolderName(oldName_);

            foreach (var path in list)
            {
                var oldPath = _commonGroupFolderPath + path;
                var newPath = _commonGroupFolderPath + path.Substring(0,path.LastIndexOf(@"\"))+@"\"+ newName_;

                Directory.Move(oldPath,newPath);
                Console.WriteLine("UpdateFolder old [{0}] -> new [{1}]", oldPath, newPath);
            }

        }
        /*
         * リストフォルダ
         *  引数　：なし
         *  戻り値：なし
         */
        public ObservableCollection<FolderGroupTreeView> ListCommonFolder() 
        {
            Console.WriteLine("ListCommonFolder:" + _commonGroupFolderPath);

            ListFolderName.Clear();
            return listFolder(_commonGroupFolderPath);
        }

        private ObservableCollection<FolderGroupTreeView> listFolder(string path_)
        {
            Console.WriteLine(" List :" + path_.Replace(_commonGroupFolderPath,string.Empty));
            var rtn = new ObservableCollection<FolderGroupTreeView>();

            foreach (var folderName in Directory.GetDirectories(path_))
            {
                var addData = new FolderGroupTreeView();

                addData.GroupName = System.IO.Path.GetFileName(folderName);
                ListFolderName.Add(addData.GroupName);

                addData.ListNest = listFolder(folderName);

                rtn.Add(addData);

            }
            return rtn;
        }

        public List<string> ListPathFindFolderName(string name_) 
        {
            Console.WriteLine("ListPathFindFolderName:" + name_);

            var rtn = new List<string>();
            foreach (var folderName in Directory.GetDirectories(_commonGroupFolderPath, name_, System.IO.SearchOption.AllDirectories))
            {
                Console.WriteLine("\t:" + folderName);
                rtn.Add(folderName.Replace(_commonGroupFolderPath, string.Empty));
            }

            return rtn;
        }

    }
}
