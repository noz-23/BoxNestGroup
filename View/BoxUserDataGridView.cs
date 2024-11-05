using Box.V2.Models;
using BoxNestGroup.Manager;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BoxNestGroup.View
{
    internal class BoxUserDataGridView : INotifyPropertyChanged
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
        public string ListNowGroup
        {
            get
            {
                //return string.Join("\n",FolderManager.Instance.ListMinimumGroup(_listAllGroup));
                return string.Join("\n", FolderManager.Instance.ListMinimumGroup(UserId));
            }

        }

        /// <summary>
        /// 現在所属の全所属
        /// </summary>
        //private List<string> _listAllGroup =new List<string>();
        public string ListNowAllGroup
        {
            get
            {
                //return string.Join("\n", _listAllGroup);
                return string.Join("\n", SettingManager.Instance.ListBoxGroupMembership.ListGroupNameInUser(UserId));
            }
            //set 
            //{
            //    _listAllGroup.Clear();
            //    _listAllGroup.AddRange(value.Split("\n"));
            //}               
        }

        /// <summary>
        /// 変更後の追加
        /// </summary>
        private List<string> _listModGroup = new List<string>();
        public string ListModGroup
        {
            get
            {
                return string.Join("\n", _listModGroup);
            }
            set
            {
                _listModGroup.Clear();
                _listModGroup.AddRange(value.Split("\n"));
                NotifyPropertyChanged();
                NotifyPropertyChanged("ListModAllGroup");
            }
        }
        /// <summary>
        /// 変更後の前所属(ネスト後)
        /// </summary>
        //private List<string> _listNestGroup = new List<string>();

        public string ListModAllGroup
        {
            //get
            //{
            //    return string.Join("\n", _listNestGroup);
            //}
            //set
            //{
            //    _listNestGroup.Clear();
            //    _listNestGroup.AddRange(value.Split("\n"));
            //}
            get
            {
                return string.Join("\n", FolderManager.Instance.ListUniqueGroup(_listModGroup)); 
            }
        }
        /// <summary>
        /// ユーザーの領域制限
        /// </summary>
        public string UserSpaceUsed { get; set; } = string.Empty;
        /// <summary>
        /// 外部コラボ制限
        /// </summary>
        public string UserExternalCollaborate { get; set; } = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }


        /// <summary>
        /// Boxのユーザー(オンライン時)
        /// </summary>
        private BoxUser? _userBox = null;


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
            _userBox = user_;

            UserName = user_.Name;
            UserMailAddress = user_.Login;
            UserId = user_.Id;

            //_listAllGroup.Clear();
            //_listAllGroup.AddRange(SettingManager.Instance.ListBoxGroupMembership.ListGroupNameInUser(UserId));

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
            Debug.WriteLine("■BoxUserDataGridView name_[{0}] mail_[{1}] listGroup_[{2}] strage_[{3}] colabo_[{4}]", name_, mail_,string.Join(",", listGroup_), strage_,colabo_);
            UserName = name_;
            UserMailAddress = mail_;

            UserSpaceUsed = (strage_.Contains(BOX_UNLIMITED) ==true) ? APP_UNLIMITED : strage_;
            UserExternalCollaborate = (colabo_.Contains( "disabled") ==true) ? "許　可" : "不許可";

            //_listAllGroup.AddRange ( listGroup_);
        }
    }
}
