using Box.V2.Models;
using BoxNestGroup.Manager;

namespace BoxNestGroup.View
{
    internal class BoxUserDataGridView 
    {
        public const string BOX_UNLIMITED = "unlimited";
        public const string APP_UNLIMITED = "無制限";

        public const string BOX_ENABLED = "enabled";
        public const string BOX_DISABLED = "disabled";

        public const string APP_ENABLED = "制　限";
        public const string APP_DISABLED = "しない";


        /// <summary>
        /// 選択
        /// </summary>
        public bool Selected { get; set; } = false;
        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// メールアドレス
        /// </summary>
        public string UserMailAddress { get; set; } = string.Empty;
        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 現在所属の最小表示(ネスト分引く)
        /// </summary>
        public string ListNowGroup { get; set; } = string.Empty;
        /// <summary>
        /// 現在所属の全所属
        /// </summary>
        public string ListAllGroup { get; set; } =string.Empty;
        /// <summary>
        /// 変更後の追加
        /// </summary>
        public string ListModGroup { get; set; } = string.Empty;
        /// <summary>
        /// 変更後の前所属(ネスト後)
        /// </summary>
        public string ListNestGroup { get; set; } = string.Empty;

        /// <summary>
        /// ユーザーの領域制限
        /// </summary>
        public string UserSpaceUsed { get; set; } = string.Empty;
        /// <summary>
        /// 外部コラボ制限
        /// </summary>
        public string UserExternalCollaborate { get; set; } = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoxUserDataGridView()
        {
        }
        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="user_">Boxユーザー</param>
        public BoxUserDataGridView(BoxUser user_)
        {
            UserName = user_.Name;
            UserMailAddress = user_.Login;
            UserId = user_.Id;
            var listGroupName = SettingManager.Instance.ListGroupNameInUser(UserId);
            ListAllGroup = string.Join("\n", listGroupName);

            var listMinGroup = FolderManager.Instance.ListMinimumGroup(listGroupName);
            ListNowGroup =string.Join("\n", listMinGroup);

            UserExternalCollaborate = (user_.IsExternalCollabRestricted ==true) ? APP_ENABLED : APP_DISABLED;
            UserSpaceUsed = (user_.SpaceUsed == -1) ? APP_UNLIMITED : user_.SpaceUsed.ToString();

        }

        /// <summary>
        /// オフライン時のコンストラクタ
        /// </summary>
        /// <param name="name_">ユーザー名</param>
        /// <param name="mail_">メールアドレス</param>
        /// <param name="listGroup_">所属グループ</param>
        /// <param name="strage_">容量制限</param>
        /// <param name="colabo_">外部コラボ制限</param>
        public BoxUserDataGridView(string name_, string mail_, IList<string> listGroup_, string strage_, string colabo_)
        {
            Console.WriteLine("■BoxUserDataGridView name_[{0}] mail_[{1}] listGroup_[{2}] strage_[{3}] colabo_[{4}]", name_, mail_,string.Join(",", listGroup_), strage_,colabo_);
            UserName = name_;
            UserMailAddress = mail_;

            UserSpaceUsed = (strage_.Contains(BOX_UNLIMITED) ==true) ? APP_UNLIMITED : strage_;
            UserExternalCollaborate = (colabo_.Contains( "disabled") ==true) ? "許　可" : "不許可";

            ListAllGroup = string.Join("\n",listGroup_);

            var listMinGroup = FolderManager.Instance.ListMinimumGroup(listGroup_);
            ListNowGroup = string.Join("\n", listMinGroup);

        }
    }
}
