using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Models;
using Box.V2.Models.Request;
using BoxNestGroup.GridView;
using BoxNestGroup.View;
using System.Collections.ObjectModel;
using System.Xml.Linq;


/*
 * https://github.com/box/box-windows-sdk-v2
 */

namespace BoxNestGroup.Manager
{
    /// <summary>
    /// Boxの通信などの管理
    /// </summary>
    internal class BoxManager
    {
        /// <summary>
        ///  シングルトン
        /// </summary>
        static public BoxManager Instance { get; } = new BoxManager();

        // 設定
        private BoxConfig ?_config =null;
        // 通信クライアント
        private BoxClient ?_client =null;

        // token
        private const string _authEnticationUrl = "https://api.box.com/oauth2/token";

        //public List<BoxGroup> ListGroup { get; private set; } = new List<BoxGroup>();
        //public List<BoxUser> ListUser { get; private set; } = new List<BoxUser>();

        // 参照 URL
        public string AuthorizationUrl { get { return (_config !=null) ?_config.AuthCodeUri.ToString():string.Empty; } }

        //private string _userToken = "";
        private BoxManager()
        {
            RenewConfig();
        }

        public void RenewConfig()
        {
            var redirectUri = new Uri(string.Format(Settings.Default.RedirectUrl, Settings.Default.PortNumber));
            _config = new BoxConfig(Settings.Default.ClientID, Settings.Default.SecretID, redirectUri);
            _client = new BoxClient(_config);
        }

        public async Task CreateBoxClient(string userCode_)
        {
            Console.WriteLine("■CreateBoxClient userCode_:{0}", userCode_);
            var session = await _client.Auth.AuthenticateAsync(userCode_);
            _client = new BoxClient(_config, session);
            //
            //
            try
            {
                session = await _client.Auth.RefreshAccessTokenAsync(session.AccessToken);
                _client = new BoxClient(_config, session);
            }
            catch (Exception ex)
            {
                Console.WriteLine("　CreateBoxClient:{0}", ex.Message);
            }

            SetTokens(_client.Auth.Session.AccessToken, _client.Auth.Session.RefreshToken);
        }

        // アクセストークンを持ってるか？
        public bool IsHaveAccessToken
        {
            get
            {
                var userToken = Settings.Default.AccessToken;
                Console.WriteLine("■IsHaveAccessToken:{0}", userToken);
    
                switch (userToken)
                {
                    case "N/A":
                    case "":
                        return false;
                }
                return true;
            }
        }

        public bool IsHaveRefreshToken
        {
            get
            {
                var refreshToken = Settings.Default.RefreshToken;
                Console.WriteLine("■IsHaveRefreshToken:{0}", refreshToken);

                switch (refreshToken)
                {
                    case "N/A":
                    case "":
                        return false;
                }
                return true;
            }
        }

        public void OAuthToken()
        {
            // トークンがあればそのトークンを使って再セッション
            var accessToken = Settings.Default.AccessToken;
            string refreshToken = (IsHaveRefreshToken == false) ? "" : Settings.Default.RefreshToken;
            Console.WriteLine("■OAuthToken : AccessToken[{0}] RefreshToken[{1}]", accessToken, refreshToken);

            // リフレッシュトークン取得
            var session = new OAuthSession(accessToken, refreshToken, Constants.AccessTokenExpirationTime, Constants.BearerTokenType);
            _client = new BoxClient(_config, session);

            SetTokens(session.AccessToken, session.RefreshToken);

        }

        public async Task RefreshToken()
        {
            Console.WriteLine("■RefreshToken");
            var session = await _client.Auth.RefreshAccessTokenAsync(Settings.Default.AccessToken);
            _client = new BoxClient(_config, session);

            SetTokens(session.AccessToken, session.RefreshToken);
        }


        // ユーザートークンの保存
        public void SetTokens(string accessToken_, string refreshToken_)
        {
            Console.WriteLine("■ SeTokens Access[{0}] Refresh[{1}]", accessToken_, refreshToken_);

            Settings.Default.AccessToken = accessToken_;
            Settings.Default.RefreshToken = refreshToken_;

            Settings.Default.Save();
        }

        // ログインしてるユーザー名
        public async Task<string> LoginUserName()
        {
            if (_client == null) 
            {
                return string.Empty;
            }
            try
            {
                var currentUser = await _client.UsersManager.GetCurrentUserInformationAsync();
                Console.WriteLine("■LoginUserName:" + currentUser.Name);
                return currentUser.Name;
            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_);
                //throw;
                return string.Empty;
            }
        }

        /*
         * グループデータの取得
         */
        public async Task<ObservableCollection<BoxGroupDataGridView>> ListGroupData()
        {
            if (_client != null)
            {
                try
                {
                    var listGroup = await _client.GroupsManager.GetAllGroupsAsync();

                    var list = listGroup.Entries;

                    return await listGroupData(list);
                }
                catch (Exception ex_)
                {
                    Console.WriteLine("■ListGroupData:" + ex_.ToString());
                }
            }
            return new ObservableCollection<BoxGroupDataGridView>();
        }
        // グループデータの取得 Box から View 変換
        private async Task<ObservableCollection<BoxGroupDataGridView>> listGroupData(List<BoxGroup> listGroup_)
        {
            //var listDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            SettingManager.Instance.ListGroupDataGridRow.Clear();
            foreach (var group in listGroup_)
            {
                var add = new BoxGroupDataGridView(group);
                await add.Inital();

                //listDataGridRow.Add(add);
                SettingManager.Instance.ListGroupDataGridRow.Add(add);

                Console.WriteLine("■listGroupData add : name[{0}] id[{1}]", add.GroupName, add.GroupId);

            }
            //return listDataGridRow;
            return SettingManager.Instance.ListGroupDataGridRow;
        }

        // グループの作成
        public async Task<BoxGroup> CreateGroup(string name_)
        {
            Console.WriteLine("■CreateGroup : {0}" , name_);
            if (_client == null)
            {
                return null;
            }

            var request = new BoxGroupRequest() { Name = name_ };
            return await _client.GroupsManager.CreateAsync(request);

        }

        // グループの更新
        public async Task<BoxGroup> UpdateGroupName(string id_, string name_)
        {
            Console.WriteLine("■UpdateGroupName : id[{0}] name[{0}]", id_, name_);
            if (_client == null)
            {
                return null;
            }

            var request = new BoxGroupRequest() { Id = id_, Name = name_ };
            return await _client.GroupsManager.UpdateAsync(id_, request);
        }

        // グループのメンバー取得
        public async Task<BoxCollection<BoxGroupMembership>> ListMemberIdFromGroup(string gorupId_)
        {
            Console.WriteLine("■UpdateGroupName : [{0}]", gorupId_);
            if (_client == null)
            {
                return null;
            }
            return await _client.GroupsManager.GetAllGroupMembershipsForGroupAsync(gorupId_);
        }

        // ユーザー情報の取得
        public async Task<ObservableCollection<BoxUserDataGridView>> ListUserData()
        {
            if (_client != null)
            {
                try
                {
                    var listUser = await _client.UsersManager.GetEnterpriseUsersAsync();

                    var list = listUser.Entries;
                    return listUserData(list);

                }
                catch (Exception ex_)
                {
                    Console.WriteLine("■ListUserData:" + ex_.ToString());
                }
            }
            return new ObservableCollection<BoxUserDataGridView>();
        }

        // ユーザー情報の取得 Box から Viewへ変換
        private ObservableCollection<BoxUserDataGridView> listUserData(List<BoxUser> listUser_)
        {
            //var listDataGridRow = new ObservableCollection<BoxUserDataGridView>();
            SettingManager.Instance.ListUserDataGridRow.Clear();
            foreach (var user in listUser_)
            {
                var add = new BoxUserDataGridView(user);
                //listDataGridRow.Add(add);
                SettingManager.Instance.ListUserDataGridRow.Add(add);
                Console.WriteLine("■listUserData add : name[{0}] id[{1}]", add.UserName, add.UserId);

            }
            //return listDataGridRow;
            return SettingManager.Instance.ListUserDataGridRow;
        }


        public async Task AddGroupUser(string userId_, List<string> listGroupId_) 
        {
            //var updates = new BoxGroupMembershipRequest()
            //{
            //    Role = "member"
            //};
            //BoxGroupMembership updatedMembership = await _client.GroupsManager.UpdateGroupMembershipAsync("33333", updates);

            //var = _client.GroupsManager.GetAllGroupMembershipsForUserAsync(userId_);

            foreach(var id in listGroupId_)
            {
                //var groupId = SettingManager.Instance.GetGroupId(name);
                //var group = SettingManager.Instance.GetGroupData(name);
                //list.Add(new BoxGroupMembership() { Group );
                var request = new BoxGroupMembershipRequest()
                {
                    User = new BoxRequestEntity() { Id = userId_ },
                    Group = new BoxGroupRequest() { Id = id }
                };

                await _client.GroupsManager.AddMemberToGroupAsync(request);
            }


            //await _client.GroupsManager.UpdateGroupMembershipAsync(userId_,)
        }

        public async Task DeleteGroupUser(List<BoxGroupMembership> list_)
        {

            foreach (var member in list_) 
            {
                Console.WriteLine("■DeleteGroupUser id[{0}] group[{1}] user[{2}]" , member.Id, member.Group.Id, member.User.Id);
                var rtn = await _client.GroupsManager.DeleteGroupMembershipAsync(member.Id);
            }
        }

    }
}
