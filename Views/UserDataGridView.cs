using Box.V2.Models;
using BoxNestGroup.Managers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BoxNestGroup.Views
{
    public class UserDataGridView : INotifyPropertyChanged
    {
        public const string BOX_UNLIMITED = "unlimited";
        public const string APP_UNLIMITED = "無制限";

        public const string BOX_ENABLED = "enabled";
        public const string BOX_DISABLED = "disabled";

        public const string APP_ENABLED = "制限あり";
        public const string APP_DISABLED = "制限なし";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// ユーザー名
        /// </summary>
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }

            set
            {
                _userName = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// メールアドレス
        /// </summary>
        private string _userMailAddress = string.Empty;
        public string UserMailAddress
        {
            get { return _userMailAddress; }

            set
            {
                _userMailAddress = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 現在所属の最小表示(ネスト分引く)
        /// </summary>
        public string ListNowGroup{get=>string.Join("\n", FolderManager.Instance.ListMinimumGroup(_listNowAllGroup));}

        /// <summary>
        /// 現在所属の全所属
        /// </summary>
        private List<string> _listNowAllGroup =new List<string>();
        public string ListNowAllGroup
        {
            get=> string.Join("\n", _listNowAllGroup);
            set 
            {
                _listNowAllGroup.Clear();
                _listNowAllGroup.AddRange(value.Split("\n"));
                _listNowAllGroup.Sort();
                NotifyPropertyChanged();
                NotifyPropertyChanged("ListNowGroup");
            }
        }

        /// <summary>
        /// 変更後の追加
        /// </summary>
        private List<string> _listModGroup = new List<string>();
        public string ListModGroup
        {
            get=> string.Join("\n", _listModGroup);
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
        public string ListModAllGroup{get=> string.Join("\n", FolderManager.Instance.ListUniqueGroup(_listModGroup));}
        /// <summary>
        /// ユーザーの領域制限
        /// </summary>
        public string UserSpaceUsed { get; private set; } = string.Empty;
        /// <summary>
        /// 外部コラボ制限
        /// </summary>
        public string UserExternalCollaborate { get; private set; } = string.Empty;

        /// <summary>
        /// Boxのユーザー(オンライン時)
        /// </summary>
        private BoxUser? _userBox = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        private UserDataGridView()
        {
        }
        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="user_">Boxユーザー</param>
        public UserDataGridView(BoxUser user_)
        {
            _userBox = user_;

            UserName = user_.Name;
            UserMailAddress = user_.Login;
            UserId = user_.Id;

            _listNowAllGroup.Clear();
            _listNowAllGroup.AddRange(SettingManager.Instance.ListMembershipGroupNameMail.ListGroupNameInUser(user_.Login));

            UserSpaceUsed = (user_.SpaceUsed == -1) ? APP_UNLIMITED : user_.SpaceUsed.ToString();
            UserExternalCollaborate = (user_.IsExternalCollabRestricted ==true) ? APP_ENABLED : APP_DISABLED;

        }

        /// <summary>
        /// オフライン時のコンストラクタ
        /// </summary>
        /// <param name="name_">ユーザー名</param>
        /// <param name="mail_">メールアドレス</param>
        /// <param name="listGroup_">所属グループ</param>
        /// <param name="strage_">容量制限</param>
        /// <param name="colabo_">外部コラボ制限</param>
        public UserDataGridView(string name_, string mail_, IList<string> listGroup_, string strage_, string colabo_)
        {
            Debug.WriteLine("■BoxUserDataGridView name_[{0}] mail_[{1}] listGroup_[{2}] strage_[{3}] colabo_[{4}]", name_, mail_,string.Join(",", listGroup_), strage_,colabo_);
            UserName = name_;
            UserMailAddress = mail_;

            _listNowAllGroup.Clear();
            _listNowAllGroup.AddRange(listGroup_);

            UserSpaceUsed = (strage_.Contains(BOX_UNLIMITED) ==true) ? APP_UNLIMITED : strage_;
            UserExternalCollaborate = (colabo_.Contains(BOX_DISABLED) ==true) ? APP_ENABLED : APP_DISABLED;
        }

        public void UpdateGroupName(string oldName_, string newName_)
        {

            if (_listNowAllGroup.Contains(oldName_) == true) 
            {
                _listNowAllGroup.Remove(oldName_);
                _listNowAllGroup.Add(newName_);
            }
            _listNowAllGroup.Sort();
        }
    }
}
