using Box.V2.Models;
using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using BoxNestGroup.Extensions;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// ユーザー情報の表示用クラス
    /// </summary>
    public class UserDataGridView : BaseView
    {
        public const string BOX_UNLIMITED = "unlimited";
        public const string BOX_ENABLED = "enabled";
        public const string BOX_DISABLED = "disabled";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserDataGridView():base()
        {
            _flgInital = true;
        }
        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="user_">Boxユーザー</param>
        public UserDataGridView(BoxUser? user_)
        {
            LogFile.Instance.WriteLine($"[{user_?.Id}] [{user_?.Name}] [{user_?.Login}]");

            _userBox = user_;

            UserName = user_?.Name ??string.Empty;
            UserLogin = user_?.Login ?? string.Empty;
            UserId = user_?.Id ?? string.Empty;

            //_listNowAllGroup.Clear();
            //_listNowAllGroup.AddRange(SettingManager.Instance.ListMembershipGroupNameLogin.ListGroupNameInUser(user_?.Login??string.Empty));
            ListNowAllGroup.Clear();
            ListNowAllGroup.AddRange(SettingManager.Instance.ListMembershipGroupNameLogin.ListGroupNameInUser(user_?.Login ?? string.Empty));

            UserSpaceUsed = ((user_?.SpaceUsed ?? -1) != -1) ? user_?.SpaceUsed?.ToString()??string.Empty:Properties.Resource.BOX_USER_DISK_UNLIMITED;
            UserExternalCollaborate = (user_?.IsExternalCollabRestricted == true) ? Properties.Resource.BOX_USER_LIMIT_ENABLED : Properties.Resource.BOX_USER_LIMIT_DISABELD;
            _flgInital = true;
        }

        /// <summary>
        /// オフライン時のコンストラクタ
        /// </summary>
        /// <param name="name_">ユーザー名</param>
        /// <param name="mail_">メールアドレス</param>
        /// <param name="listGroup_">所属グループ</param>
        /// <param name="strage_">容量制限</param>
        /// <param name="colabo_">外部コラボ制限</param>
        public UserDataGridView(string name_, string login_, IList<string> listGroup_, string strage_, string colabo_)
        {
            LogFile.Instance.WriteLine($"[{name_}] [{login_}] [{string.Join(",", listGroup_)}]");

            UserName = name_;
            UserLogin = login_;
            UserId = Properties.Resource.ID_NAME_OFFLINE;

            //_listNowAllGroup.Clear();
            //_listNowAllGroup.AddRange(listGroup_);
            ListNowAllGroup.Clear();
            ListNowAllGroup.AddRange(listGroup_);

            UserSpaceUsed = (strage_.Contains(BOX_UNLIMITED) == true) ? Properties.Resource.BOX_USER_DISK_UNLIMITED : strage_;
            UserExternalCollaborate = (colabo_.Contains(BOX_DISABLED) == true) ? Properties.Resource.BOX_USER_LIMIT_ENABLED : Properties.Resource.BOX_USER_LIMIT_DISABELD;
            _flgInital = true;
        }

        /// <summary>
        /// 初期読み込みフラグ
        /// </summary>
        private bool _flgInital = false;

        /// <summary>
        /// ステータス表示
        /// </summary>
        public Status StatudData
        {
            get => _statudData;
            private set => _statudData = value;
        }
        private Status _statudData = Status.NONE;

        public string StatusName
        {
            get => StatusString(_statudData);
        }


        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => _SetValue(ref _userName, value);
        }
        private string _userName = string.Empty;

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string UserLogin
        {
            get => _userLogin;
            set => _SetValue(ref _userLogin, value);
        }
        private string _userLogin = string.Empty;

        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; private set; } = string.Empty;

        /// <summary>
        /// 現在所属の最小表示(ネスト分引く)
        /// </summary>
        //public string ListNowGroup { get => string.Join("\n", SettingManager.Instance.ListXmlGroupTreeView.ListMinimumGroup(_listNowAllGroup)); }
        public ObservableCollection<string> ListNowGroup{get =>new ObservableCollection<string>(SettingManager.Instance.ListXmlGroupTreeView.ListMinimumGroup(ListNowAllGroup));}

        /// <summary>
        /// 現在所属の全所属
        /// </summary>
        //public string ListNowAllGroup
        //{
        //    get => string.Join("\n", _listNowAllGroup);
        //    set
        //    {
        //        _listNowAllGroup.Clear();
        //        _listNowAllGroup.AddRange(value.Split("\n"));
        //        _listNowAllGroup.Sort();
        //        _NotifyPropertyChanged();
        //        _NotifyPropertyChanged("ListNowGroup");
        //    }
        //}
        public ObservableCollection<string> ListNowAllGroup
        { 
            get=> _listNowAllGroup;
            set
            {
                _listNowAllGroup.Clear();
                _listNowAllGroup.AddRange(value);
                _NotifyPropertyChanged();
                _NotifyPropertyChanged("ListNowGroup");
            }
        }
        private ObservableCollection<string> _listNowAllGroup = new ObservableCollection<string>();

        /// <summary>
        /// 変更後の追加
        /// </summary>
        public string ListModGroup
        {
            get => string.Join("\n", _listModGroup);
            set
            {
                _listModGroup.Clear();
                _listModGroup.AddRange(value.Split("\n"));
                _NotifyPropertyChanged();
                _NotifyPropertyChanged("ListModAllGroup");
            }
        }
        private List<string> _listModGroup = new List<string>();

        /// <summary>
        /// 変更後の前所属(ネスト後)
        /// </summary>
        public string ListModAllGroup { get => string.Join("\n", SettingManager.Instance.ListXmlGroupTreeView.ListUniqueGroup(_listModGroup)); }

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
        /// 通知の変更
        /// </summary>
        /// <param name="propertyName_"></param>
        protected override void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            if (_flgInital == true)
            {
                _statudData = (string.IsNullOrEmpty(UserId) ==true) ? Status.NEW : Status.MOD;
                base._NotifyPropertyChanged("StatusName");
            }

            base._NotifyPropertyChanged(propertyName_);
        }

        /// <summary>
        /// グループ名の更新
        /// </summary>
        /// <param name="oldName_">古いグループ</param>
        /// <param name="newName_">新しいグループ</param>
        public void UpdateGroupName(string oldName_, string newName_)
        {
            //if (_listNowAllGroup.Contains(oldName_) == true) 
            //{
            //    _listNowAllGroup.Remove(oldName_);
            //    _listNowAllGroup.Add(newName_);
            //}
            //_listNowAllGroup.Sort();
            if (ListNowAllGroup.Contains(oldName_) == true)
            {
                ListNowAllGroup.Remove(oldName_);
                ListNowAllGroup.Add(newName_);
            }
            ListNowAllGroup.Sort();

        }
    }
}
