using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class MembershipGroupNameMailView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// ユーザーメールアドレス
        /// </summary>
        public string UserAddress { get; private set; } = string.Empty;
        public MembershipGroupNameMailView(string groupName_, string userAddress_)
        {
            GroupName = groupName_;
            UserAddress = userAddress_;
        }

    }
}
