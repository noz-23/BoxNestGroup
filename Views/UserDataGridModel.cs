using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class UserDataGridModel: ObservableCollection<UserDataGridView>
    {
        public void UpdateGroupName(string oldName_, string newName_)
        {
            this.ToList().ForEach(m => m.UpdateGroupName(oldName_, newName_));
        }
    }
}
