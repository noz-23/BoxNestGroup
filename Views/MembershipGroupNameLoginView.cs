using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// メンバシップ(グループ名とユーザーログイン情報)の紐付き情報
    /// </summary>
    public class MembershipGroupNameLoginView : BaseView
    {
        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// ユーザーログイン情報(メールアドレス)
        /// </summary>
        public string UserLogin { get; private set; } = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="groupName_"></param>
        /// <param name="userLogin_"></param>
        public MembershipGroupNameLoginView(string groupName_, string userLogin_)
        {
            GroupName = groupName_;
            UserLogin = userLogin_;
        }

    }
}
