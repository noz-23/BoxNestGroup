using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Models;
using Box.V2.Models.Request;
using BoxNestGroup.Views;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/*
 * https://github.com/box/box-windows-sdk-v2
 */

namespace BoxNestGroup.Managers
{
    /// <summary>
    /// Boxの通信などの管理
    /// </summary>
    public class BoxManager
    {
        /// <summary>
        ///  シングルトン
        /// </summary>
        static public BoxManager Instance { get; private set; } = new BoxManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private BoxManager()
        {
            RenewConfig();
        }

        /// <summary>
        /// 設定
        /// </summary>
        private BoxConfig ?_config =null;
        /// <summary>
        /// 通信クライアント
        /// </summary>
        private BoxClient ?_client =null;

        /// <summary>
        /// Token取得のURL(BoxConfig)
        /// </summary>
        //private const string _authEnticationUrl = "https://api.box.com/oauth2/token";


        /// <summary>
        /// 承認の URL
        /// </summary>
        public string AuthorizationUrl{ get=> (_config !=null) ?_config.AuthCodeUri.ToString():string.Empty;}


        /// <summary>
        /// 設定変更時の更新処理
        /// </summary>

        public void RenewConfig()
        {
            var redirectUri = new Uri(string.Format(Settings.Default.RedirectUrl, Settings.Default.PortNumber));
            _config = new BoxConfig(Settings.Default.ClientID, Settings.Default.SecretID, redirectUri);
            _client = new BoxClient(_config);
        }

        /// <summary>
        /// ユーザーコードからアクセストークンの取得
        /// </summary>
        /// <param name="userCode_"></param>
        /// <returns></returns>
        public async Task CreateBoxClient(string userCode_)
        {
            LogFile.Instance.WriteLine($"UserCode [{userCode_}]");

            if (_client == null)
            {
                return ;
            }
            var session = await _client.Auth.AuthenticateAsync(userCode_);
            _client = new BoxClient(_config, session);
            //
            try
            {
                session = await _client.Auth.RefreshAccessTokenAsync(session.AccessToken);
                _client = new BoxClient(_config, session);
            }
            catch (Exception ex_)
            {
                LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
            }

            SetTokens(_client.Auth.Session.AccessToken, _client.Auth.Session.RefreshToken);
        }

        /// <summary>
        /// クライアントIDがあるか
        /// </summary>
        public bool IsHaveClientID { get => IsNoData(Settings.Default.ClientID?.Trim() ?? string.Empty); }
        /// <summary>
        /// シークレットがあるか
        /// </summary>
        public bool IsSecretID { get => IsNoData(Settings.Default.SecretID?.Trim() ?? string.Empty); }

        /// <summary>
        /// アクセストークンを取得済みか(true:取得済み)
        /// </summary>
        public bool IsHaveAccessToken { get => IsNoData(Settings.Default.AccessToken?.Trim() ?? string.Empty); }

        /// <summary>
        /// リフレッシュトークンを取得済みか(true:取得済み)
        /// </summary>
        public bool IsHaveRefreshToken { get=> IsNoData(Settings.Default.RefreshToken?.Trim() ?? string.Empty); }

        /// <summary>
        /// オンライン接続してるか
        /// </summary>
        public bool IsOnlne { get => _client != null; }

        /// <summary>
        /// 承認接続処理
        /// </summary>
        public void OAuthToken()
        {
            // トークンがあればそのトークンを使って再セッション
            var accessToken = Settings.Default.AccessToken;
            string refreshToken = (IsHaveRefreshToken == false) ? "" : Settings.Default.RefreshToken;
            LogFile.Instance.WriteLine($"OAuthToken AccessToken[{accessToken}] RefreshToken[{refreshToken}]");

            // リフレッシュトークン取得
            var session = new OAuthSession(accessToken, refreshToken, Constants.AccessTokenExpirationTime, Constants.BearerTokenType);
            _client = new BoxClient(_config, session);

            SetTokens(session.AccessToken, session.RefreshToken);

        }

        /// <summary>
        /// リフレッシュトークンの更新
        /// </summary>
        public async Task RefreshToken()
        {
            LogFile.Instance.WriteLine($"RefreshToken");
            if (_client == null)
            {
                return;
            }

            var session = await _client.Auth.RefreshAccessTokenAsync(Settings.Default.AccessToken);
            _client = new BoxClient(_config, session);

            SetTokens(session.AccessToken, session.RefreshToken);
        }


        /// <summary>
        /// トークンの保存処理
        /// </summary>
        /// <param name="accessToken_">アクセストークン</param>
        /// <param name="refreshToken_">リフレッシュトークン</param>
        public void SetTokens(string accessToken_, string refreshToken_)
        {
            LogFile.Instance.WriteLine($"Access[{accessToken_}] Refresh[{refreshToken_}]");


            Settings.Default.AccessToken = accessToken_;
            Settings.Default.RefreshToken = refreshToken_;

            Settings.Default.Save();
        }

        /// <summary>
        /// ログインユーザ名の取得
        /// </summary>
        /// <returns>ユーザ名</returns>
        //public async Task<string> LoginUserName()
        public async Task GetLoginUser()
        {
            //if (_client != null) 
            //{
                try
                {
                    var currentUser = await _client?.UsersManager.GetCurrentUserInformationAsync();
                    SettingManager.Instance.LoginName = currentUser?.Name?? "取得失敗";
                }
                catch (Exception ex_)
                {
                    SettingManager.Instance.LoginName = "取得失敗";
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
        }

        /// <summary>
        /// グループデータの取得
        /// </summary>
        public async Task GetListGroupData()
        {
            //if (_client != null)
            //{
                try
                {
                    LogFile.Instance.WriteLine($"GetListGroupData");

                    SettingManager.Instance.ListGroupDataGridView.Clear();
                    SettingManager.Instance.ListMembershipGroupNameLogin.Clear();

                    // Boxからグループデータ取得
                    var listGroup = await _client?.GroupsManager.GetAllGroupsAsync();
                    foreach (var group in listGroup.Entries)
                    {
                        SettingManager.Instance.ListGroupDataGridView.Add(new GroupDataGridView(group));

                        // Boxからメンバシップ取得
                        var listMembership = await BoxManager.Instance.GetListMemberIdFromGroup(group.Id);

                        listMembership.Entries?.ForEach(membership => SettingManager.Instance.ListMembershipGroupNameLogin.Add(new MembershipGroupNameLoginView(membership.Group.Name, membership.User.Login)));
                    }
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
        }
        /// <summary>
        /// Boxにグループを作成
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>作成したBoxグループ</returns>
        public async Task<BoxGroup> CreateGroup(string groupName_)
        {
            LogFile.Instance.WriteLine($"CreateGroup [{groupName_}]");

            //if (_client != null)
            //{
                try
                {
                    var request = new BoxGroupRequest() { Name = groupName_ };
                    return await _client?.GroupsManager.CreateAsync(request);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
            return null;
        }

        // グループの更新
        /// <summary>
        /// グループの更新
        /// </summary>
        /// <param name="groupId_">グループID</param>
        /// <param name="groupName_">グループ名</param>
        /// <returns></returns>
        public async Task<BoxGroup> UpdateGroupName(string groupId_, string groupName_)
        {
            LogFile.Instance.WriteLine($"UpdateGroup [{groupId_}] [{groupName_}]");

            //if (_client != null)
            //{
                try
                {
                    var request = new BoxGroupRequest() { Id = groupId_, Name = groupName_ };
                    return await _client?.GroupsManager.UpdateAsync(groupId_, request);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}

            return null;
        }

        /// <summary>
        /// グループのメンバー取得
        /// </summary>
        /// <param name="gorupId_">グループID</param>
        /// <returns>所属するメンバシップ(グループとユーザの紐付け)の取得</returns>
        public async Task<BoxCollection<BoxGroupMembership>> GetListMemberIdFromGroup(string gorupId_)
        {
            LogFile.Instance.WriteLine($"GetListMember [{gorupId_}]");

            //if (_client != null)
            //{
                try
                {
                    return await _client?.GroupsManager.GetAllGroupMembershipsForGroupAsync(gorupId_);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
            return null;
        }

        /// <summary>
        /// ユーザーデータの取得
        /// </summary>
        /// <returns>全ユーザーの取得(データービュー)</returns>
        public async Task GetListUserData()
        {
            LogFile.Instance.WriteLine($"GetListUserData");
            //if (_client != null)
            //{
                try
                {
                    var listUser = await _client?.UsersManager.GetEnterpriseUsersAsync();

                    SettingManager.Instance.ListUserDataGridView.Clear();

                    listUser.Entries?.ForEach(user => SettingManager.Instance.ListUserDataGridView.Add(new UserDataGridView(user)));
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
        }

        /// <summary>
        /// グループへユーザを追加
        /// </summary>
        /// <param name="userId_">追加するユーザーID</param>
        /// <param name="listGroupId_">追加するグループIDのリスト</param>
        public async Task AddGroupUserFromId(BoxUser user_, IList<string> listGroupId_) 
        {
            LogFile.Instance.WriteLine($"AddGroupUser[{user_.Name}] [{user_.Id}] [{user_.Login}] [{string.Join(",", listGroupId_)}]");
            //if (_client != null)
            //{
            foreach (var groupId in listGroupId_)
                {
                    var request = new BoxGroupMembershipRequest()
                    {
                        User = new BoxRequestEntity() { Id = user_.Id },
                        Group = new BoxGroupRequest() { Id = groupId }
                    };

                    try
                    {
                        await _client?.GroupsManager.AddMemberToGroupAsync(request);

                        SettingManager.Instance.ListMembershipGroupNameLogin.Add(new MembershipGroupNameLoginView(groupId,user_.Login));
                    }
                    catch (Exception ex_)
                    {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            }
            //}
        }

        /// <summary>
        /// グループ名に対しユーザーを追加
        /// </summary>
        /// <param name="userId_"></param>
        /// <param name="listGroupName_"></param>
        /// <returns></returns>
        public async Task AddGroupUserFromName(BoxUser user_, IList<string> listGroupName_)
        {
            //Debug.WriteLine(string.Format("■BoxManager AddGroupUser userId_[{0}] listGroupName_[{1}]", user_.Id, string.Join(",", listGroupName_)));
            LogFile.Instance.WriteLine($"AddGroupUser[{user_.Name}] [{user_.Id}] [{user_.Login}] [{string.Join(",", listGroupName_)}]");

            // グループ名からグループIDに変換
            await AddGroupUserFromId(user_, SettingManager.Instance.ConvertGroupNameToId(listGroupName_));
        }

        /// <summary>
        /// メンバシップ(グループとユーザーの紐付け)削除
        /// </summary>
        /// <param name="list_">削除するメンバシップ(グループとユーザーの紐付け)のリスト</param>
        public async Task DeleteGroupUser(IList<BoxGroupMembership> list_)
        {
            LogFile.Instance.WriteLine($"DeleteGroupUser [{list_.Count}]");
            //if (_client != null)
            //{
                foreach (var member in list_)
                {
                LogFile.Instance.WriteLine($"DeleteGroupUser Member[{member.Id}] Group[{member.Group.Id}] User[{member.User.Id}]");
                try
                {
                        var rtn = await _client?.GroupsManager.DeleteGroupMembershipAsync(member.Id);
                    }
                    catch (Exception ex_)
                    {
                        LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                    }
                }
            //}
        }

        /// <summary>
        /// ユーザーグループ更新
        /// </summary>
        /// <param name="user_"></param>
        /// <param name="listGroupName_"></param>
        /// <returns></returns>
        public async Task UpDateGroupUserFromName(BoxUser user_, IList<string> listGroupName_)
        {
            LogFile.Instance.WriteLine($"UpDateGroupUser [{user_.Id}] [{user_.Name}] [{user_.Login}] [{string.Join(",", listGroupName_)}]");

            // 全グループID取得
            var listGroupIdAll = SettingManager.Instance.ListGroupDataGridView.Select(g_ => g_.GroupId);
            var listMembership =new List<BoxGroupMembership>();
            foreach (var groupId in listGroupIdAll)
            {
                var list = await GetListMemberIdFromGroup(groupId);
                foreach (var membership in list.Entries)
                {
                    if (membership.User.Id != user_.Id)
                    {
                        continue;
                    }
                    listMembership.Add(membership);
                }              
            }
            // ユーザーを含むメンバシップ削除
            await DeleteGroupUser(listMembership);
            // グループ追加
            await AddGroupUserFromName(user_, listGroupName_);

            var listGroupId = SettingManager.Instance.ConvertGroupNameToId(listGroupName_);
        }

        /// <summary>
        /// ユーザーの作成
        /// </summary>
        /// <param name="userName_">名前</param>
        /// <param name="userLogin_">メールアドレス</param>
        /// <returns></returns>
        public async Task<BoxUser> CreateUser(string userName_, string userLogin_)
        {
            LogFile.Instance.WriteLine($"CreateUser [{userName_}][{userLogin_}]");
            //if (_client != null)
            //{
                try
                {
                    var request = new BoxUserRequest()
                    { 
                        Name = userName_,
                        Login = userLogin_,
                    };
                    return await _client.UsersManager.CreateEnterpriseUserAsync(request);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
            return null;
        }

        /// <summary>
        /// ユーザーの更新
        /// </summary>
        /// <param name="userId_">ユーザーID</param>
        /// <param name="userName_">名前</param>
        /// <param name="userLogin_">メールアドレス</param>
        /// <returns></returns>
        public async Task<BoxUser> UpdateUser(string userId_,string userName_, string userLogin_)
        {
            LogFile.Instance.WriteLine($"UpdateUser [{userId_}][{userName_}][{userLogin_}]");
            //if (_client != null)
            //{
                try
                {
                    var request = new BoxUserRequest()
                    {
                        Id = userId_,
                        Name = userName_,
                        Login = userLogin_,
                    };
                    return await _client.UsersManager.UpdateUserInformationAsync(request);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
                }
            //}
            return null;
        }

        /// <summary>
        /// IDデータの有無
        /// </summary>
        /// <param name="data_"></param>
        /// <param name="propertyName_"></param>
        /// <returns></returns>
        private bool IsNoData(string data_, [CallerMemberName] string propertyName_="")
        {
            LogFile.Instance.WriteLine($"IsNoData [{data_}][{propertyName_}]");

            switch (data_)
            {
                case "N/A":
                case "Please Input":
                case "":
                    return false;
            }
            return true;
        }
    }
}
