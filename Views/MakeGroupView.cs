using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class MakeGroupView: BaseView
    {

        public MakeGroupView(GroupDataGridView groupView_) : base()
        {
            LogFile.Instance.WriteLine($"{groupView_.GroupName} {groupView_.GroupId}");
            //

            IsChecked = false;
            GroupName = groupView_.GroupName;
            GroupId = groupView_.GroupId;
        }

        public bool IsChecked { get; set; } = false; 

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// グループID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

    }
}
