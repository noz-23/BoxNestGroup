using BoxNestGroup.GridView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.View
{
    class FolderGroupTreeView
    {
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }


        public ObservableCollection<FolderGroupTreeView> ListNest = new ObservableCollection<FolderGroupTreeView>();

    }
}
