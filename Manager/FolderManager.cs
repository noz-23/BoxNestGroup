using Box.V2.Models;
using BoxNestGroup.View;
using System.Collections.ObjectModel;
using System.IO;

namespace BoxNestGroup.Manager
{
    /// <summary>
    /// フォルダの管理
    /// </summary>
    class FolderManager
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        static public FolderManager Instance { get; } = new FolderManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private FolderManager()
        {
        }

        /// <summary>
        /// フォルダ管理パス
        /// </summary>
        public string CommonGroupFolderPath { get; private set; } = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupFolder;

        /// <summary>
        /// フォルダ(グループ名)の一覧
        /// 　あるなしの判断
        /// </summary>
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
                }
                return _listFolderName; 
            }
        }

        /// <summary>
        /// フォルダのツリービュー管理
        /// </summary>
        private ObservableCollection<FolderGroupTreeView> _listFolderTree = new ObservableCollection<FolderGroupTreeView>();
        public ObservableCollection<FolderGroupTreeView> ListFolderTree
        {
            get
            {
                Console.WriteLine("■ListCommonFolder:" + CommonGroupFolderPath);
                _listFolderTree.Clear();

                //ListFolderName.Clear();
                // このフォルダリスト
                var list = new ObservableCollection<FolderGroupTreeView>();
                list.Add(new FolderGroupTreeView() { GroupName = Settings.Default.ClearGroupName });

                _listFolderTree = listFolder(CommonGroupFolderPath, list);
                return _listFolderTree;
            }
        }


        /// <summary>
        /// フォルダ(グループ)作成
        /// </summary>
        /// <param name="boxGroup_">Boxグループ</param>
        public void CreateFolder(BoxGroup boxGroup_)
        {
            CreateFolder(boxGroup_.Name);
        }

        /// <summary>
        /// フォルダ(グループ)作成
        /// </summary>
        /// <param name="boxGroup_">グループ名</param>
        public void CreateFolder(string name_) 
        {
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
            Console.WriteLine("■UpdateFolder old [{0}] new [{1}]", oldName_, newName_);
            var list = ListPathFindFolderName(oldName_);

            foreach (var path in list)
            {
                var oldPath = CommonGroupFolderPath + path;
                var newPath = CommonGroupFolderPath + path.Substring(0,path.LastIndexOf(@"\"))+@"\"+ newName_;

                Directory.Move(oldPath,newPath);
                Console.WriteLine("　UpdateFolder old [{0}] -> new [{1}]", oldPath, newPath);
            }
        }

        /// <summary>
        /// フォルダ管理リストの作成
        /// </summary>
        /// <param name="path_">パス</param>
        /// <param name="list_">サブフォルダ(クリア用の表示をするため、関数内で作らない)</param>
        /// <returns>フォルダ管理リスト</returns>
        private ObservableCollection<FolderGroupTreeView> listFolder(string path_, ObservableCollection<FolderGroupTreeView> list_)
        {
            //Console.WriteLine("■ listFolder :" + path_.Replace(_commonGroupFolderPath,string.Empty));

            // このフォルダリスト
            foreach (var folderPath in Directory.GetDirectories(path_))
            {
                var list = new ObservableCollection<FolderGroupTreeView>();
                var addData = new FolderGroupTreeView();

                addData.GroupName = System.IO.Path.GetFileName(folderPath);
                addData.ListNestGroup = listFolder(folderPath, list);

                list_.Add(addData);

            }
            return list_;
        }
        /// <summary>
        /// フォルダ(グループ)名を含んだパスリスト(ネスト)一覧
        /// </summary>
        /// <param name="groupName_">フォルダ(グループ)名</param>
        /// <returns>パスリスト</returns>
        public IList<string> ListPathFindFolderName(string groupName_) 
        {
            //Console.WriteLine("■ListPathFindFolderName[{0}]" , groupName_);

            var rtn = new List<string>();
            foreach (var folderName in Directory.GetDirectories(CommonGroupFolderPath, groupName_, System.IO.SearchOption.AllDirectories))
            {
                //Console.WriteLine("　find:" + folderName);
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
        public IList<string> ListMinimumGroup(IList<string> listGroupName_)
        {
            var listNest = new HashSet<string>();
            var rtn =new HashSet<string>(listGroupName_);

            // ネストしているフォルダの削除
            foreach (var name in listGroupName_)
            {
                // パスの「\」を変換することで全フォルダになる
                var listPath = ListPathFindFolderName(name);
                foreach (var path in listPath)
                {
                    var nest = path.Substring(0,path.LastIndexOf('\\'));
                    var list = nest.Split(new char[] { '\\' });
                    listNest.UnionWith(list);
                }
            }
            // ネストしているフォルダを全グループから削除
            foreach(var nest in listNest)
            {
                rtn.Remove(nest);
            }

            rtn.Remove(string.Empty);
            return new List<string>(rtn);
        }

    }
}
