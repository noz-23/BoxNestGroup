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
        static public FolderManager Instance { get; private set; } = new FolderManager();

        /// <summary>
        /// フォルダ管理パス
        /// </summary>
        public string CommonGroupFolderPath { get; private set; } = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupFolder;

        //private FileSystemWatcher _watcher = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private FolderManager()
        {
            Load();
        }

        /// <summary>
        /// フォルダの読み込み
        /// </summary>
        public void Load()
        {
            var commonDirInfo = new DirectoryInfo(CommonGroupFolderPath);

            var list = new List<FolderGroupTreeView>();
            foreach (var info in commonDirInfo.GetDirectories())
            {
                list.Add(new FolderGroupTreeView(info,null));
            }
            list.Sort((a, b) => string.Compare(a.GroupName, b.GroupName));
            ListFolderTree = new ObservableCollection<FolderGroupTreeView>(list);
        }

        /// <summary>
        /// フォルダのツリービュー管理
        /// </summary>
        public ObservableCollection<FolderGroupTreeView> ListFolderTree { get; private set; } = new ObservableCollection<FolderGroupTreeView> ();

        /// <summary>
        /// グループ名を含んでるか
        /// </summary>
        /// <param name="name_">グループ名</param>
        /// <returns></returns>
        public bool Contains(string name_)
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
        /// <param name="boxGroup_">グループ名</param>
        public void CreateFolder(string name_) 
        {
            if (Contains(name_) == true)
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
        public void UpdateGroupName(string oldName_, string newName_)
        {
            Debug.WriteLine("■UpdateFolder old [{0}] new [{1}]", oldName_, newName_);
            var list =new List<FolderGroupTreeView>();

            ListFolderTree.ToList().ForEach(view => list.AddRange(view.Find(oldName_)));
            list.ForEach(view => view.GroupName = newName_);
        }


        /// <summary>
        /// フォルダ(グループ)名を含んだパスリスト(ネスト)一覧
        /// </summary>
        /// <param name="groupName_">フォルダ(グループ)名</param>
        /// <returns>パスリスト</returns>
        public IList<string> ListPathFindFolderName(string groupName_) 
        {
            var rtn = new List<string>();
            foreach (var folderName in Directory.GetDirectories(CommonGroupFolderPath, groupName_, System.IO.SearchOption.AllDirectories))
            {
                rtn.Add(folderName.Replace(CommonGroupFolderPath, string.Empty));
            }

            return rtn;
        }

        /// <summary>
        /// ネストで含まれるフォルダ(グループ)名の取得
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>全フォルダ(グループ)名一覧</returns>
        public IList<string> ListUniqueGroup(ICollection<string> listGroupName_) 
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
        public IList<string> ListMinimumGroup(ICollection<string> listGroupName_)
        {
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
            return rtn.ToList();
        }

    }
}
