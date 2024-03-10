using BoxNestGroup.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Manager
{
    class FolderManager
    {
        static public FolderManager Instance { get; } = new FolderManager();

        string _commonGroupFolder = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupFolder;


        private FolderManager()
        {
        }

        public void CreateGroup(string name_)
        {
            Directory.CreateDirectory(_commonGroupFolder + @"\" + name_);
        }

        public async Task<ObservableCollection<FolderGroupTreeView>> ListFolder() 
        {
            return getListFolder(_commonGroupFolder);
        }

        private ObservableCollection<FolderGroupTreeView> getListFolder(string path_)
        {
            Console.WriteLine("getListFolder:" + path_);
            var rtn = new ObservableCollection<FolderGroupTreeView>();

            foreach (var folderName in Directory.GetDirectories(path_))
            {
                var addData = new FolderGroupTreeView();

                addData.Name = System.IO.Path.GetFileName(folderName);

                addData.ListNest = getListFolder(folderName);

                rtn.Add(addData);

            }
            return rtn;
        }
    }
}
