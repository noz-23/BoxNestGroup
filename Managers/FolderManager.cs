using Box.V2.Models;
using BoxNestGroup.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace BoxNestGroup.Managers
{
    /// <summary>
    /// フォルダの管理
    /// </summary>
    public class FolderManager
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        static public FolderManager Instance { get; } = new FolderManager();

        /// <summary>
        /// フォルダ管理パス
        /// </summary>
        public string CommonGroupFolderPath { get; private set; } = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupFolder;


        private FileSystemWatcher _watcher = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private FolderManager()
        {
            ////ListFolderTree = new FolderGroupTreeView(new DirectoryInfo(CommonGroupFolderPath));
            ////Debug.WriteLine("■ListCommonFolder:" + CommonGroupFolderPath);
            ////ListFolderTree.Clear();
            //var commonDirInfo = new DirectoryInfo(CommonGroupFolderPath);

            //var list =new List<FolderGroupTreeView>();
            //foreach (var info in commonDirInfo.GetDirectories())
            //{
            //    list.Add(new FolderGroupTreeView(info));
            //    //ListFolderTree.Add(new FolderGroupTreeView(info));
            //}
            ////ListFolderTree =ListFolderTree.OrderBy(x => x.GroupName);
            //list.Sort((a, b) => string.Compare(a.GroupName,b.GroupName));
            //ListFolderTree = new ObservableCollection<FolderGroupTreeView>(list);
            _watcher = new FileSystemWatcher(CommonGroupFolderPath)
            {
                NotifyFilter = System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.CreationTime | System.IO.NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };
            _watcher.Changed += new System.IO.FileSystemEventHandler((source_, e_) =>  Reload());
            _watcher.Created += new System.IO.FileSystemEventHandler((source_, e_) =>  Reload());
            _watcher.Renamed += new System.IO.RenamedEventHandler((source_, e_) =>  Reload());

            Reload();
        }

        public void Reload()
        {
            _watcher.EnableRaisingEvents = false;
            var commonDirInfo = new DirectoryInfo(CommonGroupFolderPath);

            var list = new List<FolderGroupTreeView>();
            foreach (var info in commonDirInfo.GetDirectories())
            {
                list.Add(new FolderGroupTreeView(info));
            }
            list.Sort((a, b) => string.Compare(a.GroupName, b.GroupName));
            ListFolderTree = new ObservableCollection<FolderGroupTreeView>(list);

            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// フォルダ(グループ名)の一覧
        /// 　あるなしの判断
        /// </summary>
        //private HashSet<string> _listFolderName = new HashSet<string>();
        //public HashSet<string> ListFolderName
        //{
        //    get
        //    {
        //        if (_listFolderName.Count ==0) 
        //        {
        //            foreach( var path in ListPathFindFolderName("*")) 
        //            {
        //                var list = path.Split("\\");
        //                _listFolderName.UnionWith(list);
        //            }
        //            _listFolderName.Remove(string.Empty);
        //        }
        //        return _listFolderName; 
        //    }
        //}

        /// <summary>
        /// フォルダのツリービュー管理
        /// </summary>
        //private ObservableCollection<FolderGroupTreeView> _listFolderTree = new ObservableCollection<FolderGroupTreeView>();
        public ObservableCollection<FolderGroupTreeView> ListFolderTree { get; private set; } = new ObservableCollection<FolderGroupTreeView> ();
        //{
        //    get
        //    {
        //        Debug.WriteLine("■ListCommonFolder:" + CommonGroupFolderPath);
        //        _listFolderTree.Clear();

        //        //ListFolderName.Clear();
        //        // このフォルダリスト
        //        var list = new ObservableCollection<FolderGroupTreeView>();
        //        list.Add(new FolderGroupTreeView() { GroupName = Settings.Default.ClearGroupName });

        //        _listFolderTree = listFolder(CommonGroupFolderPath, list);
        //        return _listFolderTree;
        //    }
        //}



        private bool _contains(string name_)
        {
            foreach (var view in ListFolderTree)
            {
                if ( view.Contains(name_) ==true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// フォルダ(グループ)作成
        /// </summary>
        /// <param name="boxGroup_">Boxグループ</param>
        //public void CreateFolder(BoxGroup boxGroup_)
        //{
        //    CreateFolder(boxGroup_.Name);
        //}

        /// <summary>
        /// フォルダ(グループ)作成
        /// </summary>
        /// <param name="boxGroup_">グループ名</param>
        public void CreateFolder(string name_) 
        {
            if (_contains(name_) == true)
            {
                // すでにある場合は作らない
                return;
            }

            var pathName = CommonGroupFolderPath + @"\" + name_;

            if (Directory.Exists(pathName) == false)
            {
                // フォルダがない場合は作る
                Directory.CreateDirectory(pathName);
            }
        }

        /// <summary>
        /// Box側でフォルダ名が変わった場合の更新処理
        /// </summary>
        /// <param name="oldName_">古いフォルダ(グループ)名</param>
        /// <param name="newName_">新しいフォルダ(グループ)名</param>
        public void UpdateFolder(string oldName_, string newName_)
        {
            Debug.WriteLine("■UpdateFolder old [{0}] new [{1}]", oldName_, newName_);
            //var list = ListPathFindFolderName(oldName_);

            //foreach (var path in list)
            //{
            //    var oldPath = CommonGroupFolderPath + path;
            //    var newPath = CommonGroupFolderPath + path.Substring(0,path.LastIndexOf(@"\"))+@"\"+ newName_;

            //    Directory.Move(oldPath,newPath);
            //    Debug.WriteLine("　UpdateFolder old [{0}] -> new [{1}]", oldPath, newPath);
            //}

            var list =new List<FolderGroupTreeView>();
            foreach (var view in ListFolderTree)
            {
                list.AddRange(view.Find(oldName_));
            }

            foreach (var view in list)
            {
                view.GroupName = newName_;
            }
        }

        /// <summary>
        /// フォルダ管理リストの作成
        /// </summary>
        /// <param name="path_">パス</param>
        /// <param name="list_">サブフォルダ(クリア用の表示をするため、関数内で作らない)</param>
        /// <returns>フォルダ管理リスト</returns>
        //private ObservableCollection<FolderGroupTreeView> listFolder(string path_, ObservableCollection<FolderGroupTreeView> list_)
        //{
        //    //Debug.WriteLine("■ listFolder :" + path_.Replace(_commonGroupFolderPath,string.Empty));

        //    // このフォルダリスト
        //    foreach (var folderPath in Directory.GetDirectories(path_))
        //    {
        //        var list = new ObservableCollection<FolderGroupTreeView>();
        //        var addData = new FolderGroupTreeView();

        //        addData.GroupName = System.IO.Path.GetFileName(folderPath);
        //        addData.ListNestGroup = listFolder(folderPath, list);

        //        list_.Add(addData);

        //    }
        //    return list_;
        //}

        /// <summary>
        /// フォルダ(グループ)名を含んだパスリスト(ネスト)一覧
        /// </summary>
        /// <param name="groupName_">フォルダ(グループ)名</param>
        /// <returns>パスリスト</returns>
        public IList<string> ListPathFindFolderName(string groupName_) 
        {
            //Debug.WriteLine("■ListPathFindFolderName[{0}]" , groupName_);

            var rtn = new List<string>();
            foreach (var folderName in Directory.GetDirectories(CommonGroupFolderPath, groupName_, System.IO.SearchOption.AllDirectories))
            {
                //Debug.WriteLine("　find:" + folderName);
                rtn.Add(folderName.Replace(CommonGroupFolderPath, string.Empty));
            }

            return rtn;
        }

        /// <summary>
        /// ネストで含まれるフォルダ(グループ)名の取得
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>全フォルダ(グループ)名一覧</returns>
        public IList<string> ListUniqueGroup(IList<string> listGroupName_) 
        {
            var rtn = new HashSet<string>();

            foreach (var name in listGroupName_)
            {
                // パスの「\」を変換することで全フォルダになる
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

        /// <summary>
        /// 全フォルダ(グループ)名一覧からネストを削除したグループの取得
        /// </summary>
        /// <param name="listGroupName_">全フォルダ(グループ)名一覧</param>
        /// <returns>最小のフォルダ(グループ)名一覧</returns>
        //public IList<string> ListMinimumGroup(IList<string> listGroupName_)
        //{
        //    var listNest = new HashSet<string>();
        //    var rtn =new HashSet<string>(listGroupName_);

        //    // ネストしているフォルダの削除
        //    foreach (var name in listGroupName_)
        //    {
        //        // パスの「\」を変換することで全フォルダになる
        //        var listPath = ListPathFindFolderName(name);
        //        foreach (var path in listPath)
        //        {
        //            var nest = path.Substring(0,path.LastIndexOf(@"\"));
        //            var list = nest.Split( @"\");
        //            listNest.UnionWith(list);
        //        }
        //    }
        //    // ネストしているフォルダを全グループから削除
        //    foreach(var nest in listNest)
        //    {
        //        rtn.Remove(nest);
        //    }

        //    rtn.Remove(string.Empty);
        //    return new List<string>(rtn);
        //}
        //public IList<string> ListMinimumGroup(string userId_ )
        public IList<string> ListMinimumGroup(IList<string> listGroupName_)
        {
            //var listGroupName = SettingManager.Instance.ListBoxGroupMembership.ListGroupNameInUser(userId_);
            var listNest = new HashSet<string>();
            var rtn = new HashSet<string>(listGroupName_);

            // ネストしているフォルダの削除
            foreach (var name in listGroupName_)
            {
                // パスの「\」を変換することで全フォルダになる
                var listPath = ListPathFindFolderName(name);
                foreach (var path in listPath)
                {
                    var nest = path.Substring(0, path.LastIndexOf(@"\"));
                    var list = nest.Split(@"\");
                    listNest.UnionWith(list);
                }
            }
            // ネストしているフォルダを全グループから削除
            foreach (var nest in listNest)
            {
                rtn.Remove(nest);
            }

            rtn.Remove(string.Empty);
            return new List<string>(rtn);
        }

    }
}
