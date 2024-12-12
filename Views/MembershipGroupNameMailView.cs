using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class MembershipGroupNameMailView
    {
        public string GroupName { get; set; } = string.Empty;
        public string UserAddress { get; private set; } = string.Empty;
        public MembershipGroupNameMailView(string groupName_, string userAddress_)
        {
            GroupName = groupName_;
            UserAddress = userAddress_;
        }
    }
}
