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
using Windows.Devices.Geolocation;

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
        private HashSet<string> _listFolderName = new HashSet<string>();
        public HashSet<string> ListFolderName
        {
            get
            {
                if (_listFolderName.Count ==0) 
                {
                    foreach( var path in ListPathFindFolderName("*")) 
                    {
                        var list = path.Split("\\");
                        _listFolderName.UnionWith(list);
                    }
                    _listFolderName.Remove(string.Empty);
                    Console.WriteLine("■ListFolderName:"+ string.Join(",", _listFolderName));

                }
                return _listFolderName; 
            }
        }


        private ObservableCollection<FolderGroupTreeView> _listFolderTree = new ObservableCollection<FolderGroupTreeView>();
        public ObservableCollection<FolderGroupTreeView> ListFolderTree
        {
            get
            {
                Console.WriteLine("■ListCommonFolder:" + _commonGroupFolderPath);
                _listFolderTree.Clear();

                //ListFolderName.Clear();
                // このフォルダリスト
                var list = new ObservableCollection<FolderGroupTreeView>();
                list.Add(new FolderGroupTreeView() { GroupName = Settings.Default.ClearGroupName });

                _listFolderTree = listFolder(_commonGroupFolderPath, list);
                return _listFolderTree;
            }
        }


        private FolderManager()
        {
        }

        // 
        //public bool IsHaveGroup(string name_)
        //{
        //    return ListFolderName.Contains(name_);

            
        //}

        public bool IsHaveGroupForlder(string name_)
        {
            return ListPathFindFolderName(name_).Count > 0;
        }

        /*
         * フォルダ作成
         *  引数　：name_ フォルダ名()
         *  戻り値：なし
         */

        public void CreateFolder(BoxGroup boxGroup_)
        {
            CreateFolder(boxGroup_.Name);
        }

        public void CreateFolder(string name_) 
        {
            var pathName = _commonGroupFolderPath + @"\" + name_;

            if (Directory.Exists(pathName) == false)
            {
                // フォルダがない場合は作る
                Directory.CreateDirectory(pathName);
            }
        }

        public void UpdateFolder(string oldName_, string newName_)
        {
            Console.WriteLine("■UpdateFolder old [{0}] new [{1}]", oldName_, newName_);
            var list = ListPathFindFolderName(oldName_);

            foreach (var path in list)
            {
                var oldPath = _commonGroupFolderPath + path;
                var newPath = _commonGroupFolderPath + path.Substring(0,path.LastIndexOf(@"\"))+@"\"+ newName_;

                Directory.Move(oldPath,newPath);
                Console.WriteLine("　UpdateFolder old [{0}] -> new [{1}]", oldPath, newPath);
            }

        }
        /*
         * リストフォルダ
         *  引数　：なし
         *  戻り値：なし
         */
        //public ObservableCollection<FolderGroupTreeView> ListCommonFolder() 
        //{
        //    Console.WriteLine("■ListCommonFolder:" + _commonGroupFolderPath);

        //    _listFolderTree.Clear();

        //    //ListFolderName.Clear();
        //    // このフォルダリスト
        //    var list = new ObservableCollection<FolderGroupTreeView>();
        //    list.Add(new FolderGroupTreeView() { GroupName = Settings.Default.ClearGroupName });

        //    _listFolderTree = listFolder(_commonGroupFolderPath, list);
        //    return _listFolderTree;
        //}

        private ObservableCollection<FolderGroupTreeView> listFolder(string path_, ObservableCollection<FolderGroupTreeView> list_)
        {
            Console.WriteLine("■ listFolder :" + path_.Replace(_commonGroupFolderPath,string.Empty));

            // このフォルダリスト
            foreach (var folderPath in Directory.GetDirectories(path_))
            {
                var list = new ObservableCollection<FolderGroupTreeView>();
                var addData = new FolderGroupTreeView();

                addData.GroupName = System.IO.Path.GetFileName(folderPath);
                //ListFolderName.Add(addData.GroupName);

                addData.ListNest = listFolder(folderPath, list);

                list_.Add(addData);

            }
            return list_;
        }

        public List<string> ListPathFindFolderName(string name_) 
        {
            Console.WriteLine("■ListPathFindFolderName:" + name_);

            var rtn = new List<string>();
            foreach (var folderName in Directory.GetDirectories(_commonGroupFolderPath, name_, System.IO.SearchOption.AllDirectories))
            {
                Console.WriteLine("　find:" + folderName);
                rtn.Add(folderName.Replace(_commonGroupFolderPath, string.Empty));
            }

            //if (rtn.Count ==0)
            //{
            //    // フォルダがない場合は作って、再検索
            //    CreateFolder(name_);
            //    return ListPathFindFolderName(name_);
            //}
            return rtn;
        }

        // 
        public List<string> ListUniqueGroup(List<string> listPath_) 
        {
            var rtn = new HashSet<string>();

            //Dictionary<string, int> listCount = new Dictionary<string, int>();

            foreach (var name in listPath_)
            {
                var listPath = ListPathFindFolderName(name);

                foreach (var path in listPath)
                {
                    var list =path.Split(new char[] { '\\' });
                    rtn.UnionWith(list);
                }
            }

            rtn.Remove(string.Empty);

            return new List<string>(rtn);
        }

    }
}
