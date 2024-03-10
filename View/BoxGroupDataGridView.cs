using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.GridView
{
    internal class BoxGroupDataGridView
    {
        public string GroupName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;

        public int MaxNestCount { get; set; } =0;

        public int FolderCount { get; set; } = 0;
    }
}
