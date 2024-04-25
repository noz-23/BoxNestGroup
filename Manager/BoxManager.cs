using Box.V2.Config;
using Box.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Windows.Media.Animation;
using System.Collections.Specialized;
using Box.V2.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;
using Box.V2.Auth;
using BoxNestGroup.GridView;
using System.Collections.ObjectModel;
using Box.V2.Models.Request;
using Org.BouncyCastle.Utilities.Collections;
using BoxNestGroup.View;
using Windows.Media.Protection.PlayReady;


/*
 * https://github.com/box/box-windows-sdk-v2
 */

namespace BoxNestGroup.Manager
{
    internal class BoxManager
    {
        static public BoxManager Instance { get; } = new BoxManager();

        // 設定
        private BoxConfig ?_config;
        // 通信クライアント
        private BoxClient ?_client;

        // token
        private string _authEnticationUrl = "https://api.box.com/oauth2/token";
        //private string _clientID = Settings.Default.ClientID;
        //private string _sercretID = Settings.Default.SecretID;

        public List<BoxGroup> ListGroup { get; private set; } = new List<BoxGroup>();
        public List<BoxUser> ListUser { get; private set; } = new List<BoxUser>();

        // 参照 URL
        public string AuthorizationUrl { get { return _config.AuthCodeUri.ToString(); } }

        //private string _userToken = "";
        private BoxManager()
        {
            RenewConfig();
            //
            //if (IsHaveAccessToken == true)
            //{
            //    // トークンがあればそのトークンを使って再セッション
            //    var userToken = Settings.Default.UserToken;
            //    Console.WriteLine("■BoxManager userToken:{0}", userToken);
            //    var session = new OAuthSession(userToken, "N/A", 3600, "bearer");
            //    _client = new BoxClient(_config, session);
            //}
        }

        public void RenewConfig()
        {
            var redirectUri = new Uri(string.Format(Settings.Default.RedirectUrl, Settings.Default.PortNumber));
            _config = new BoxConfig(Settings.Default.ClientID, Settings.Default.SecretID, redirectUri);
            _client = new BoxClient(_config);
        }
        // 
        //public async Task CreateBoxClient(HttpListener listener_)
        //{
        //    var userCode = await getUserCode(listener_);

        //    var session = await _client.Auth.AuthenticateAsync(userCode);
        //    _client = new BoxClient(_config, session);

        //    setUserToken(_client.Auth.Session.AccessToken);
        //}

        public async Task CreateBoxClient(string userCode_)
        {
            var session = await _client.Auth.AuthenticateAsync(userCode_);
            _client = new BoxClient(_config, session);

            SetUserToken(_client.Auth.Session.AccessToken);
        }

        // アクセストークンを持ってるか？
        public bool IsHaveAccessToken
        {
            get
            {
                var userToken = Settings.Default.UserToken;
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

        public void OAuthToken()
        {
            // トークンがあればそのトークンを使って再セッション
            var userToken = Settings.Default.UserToken;
            Console.WriteLine("■OAuthToken userToken:{0}", userToken);
            var session = new OAuthSession(userToken, "N/A", 3600, "bearer");
            _client = new BoxClient(_config, session);
        }

        // リフレッシュトークン取得
        public async Task<bool> RefreshToken()
        {
            try
            {
                OAuthToken();
                //
                var userToken = Settings.Default.UserToken;
                //Console.WriteLine("■RefreshToken userToken:{0}", userToken);

                var newSession = await _client.Auth.RefreshAccessTokenAsync(userToken);
                SetUserToken(newSession.RefreshToken);

                _client = new BoxClient(_config, newSession);

                return true;
            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_);
                return false;
            }
        }

        //// 一時的なユーザーコードの取得
        //private async Task<string> getUserCode(HttpListener listener_)
        //{
        //    var context = await listener_.GetContextAsync();
        //    var request = context.Request;

        //    var rawUrl = request.RawUrl;    // 		RawUrl	"/callback?code=yWtJmL9c2xgtZd3V2jTwCDiMMASK0sF5"	string
        //    var userCode = rawUrl.Replace(@"/callback?code=", "");

        //    return userCode;
        //}

        /*
        private async Task<string> getToken( string code_)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
              new KeyValuePair<string, string>("grant_type", "authorization_code"),
              new KeyValuePair<string, string>("code", code_),
              new KeyValuePair<string, string>("client_id", _clientID),
              new KeyValuePair<string, string>("client_secret", _sercretID)
            });

            var response = await client.PostAsync(_authEnticationUrl, content);
            var data = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(data);
            return token.access_token;
        }*/

        /*
        private async Task<string> setUserToken(string userCode_)
        {
            _userToken = await getToken(userCode_);
            Settings.Default.UserToken = _userToken;
            Settings.Default.Save();
            return userCode_;
        }
        */

        // ユーザートークンの保存
        public void SetUserToken(string userToken_)
        {
            Console.WriteLine("■setUserToken userToken_:{0}", userToken_);

            Settings.Default.UserToken = userToken_;
            Settings.Default.Save();
        }

        // ログインしてるユーザー名
        public async Task<string> LoginUserName()
        {
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
            try
            {
                var listGroup = await _client.GroupsManager.GetAllGroupsAsync();

                ListGroup = listGroup.Entries;

            }
            catch (Exception ex_)
            {
                Console.WriteLine("■ListGroupData:" + ex_.ToString());
            }
            return  listGroupData(ListGroup);
        }
        // グループデータの取得 Box から View 変換
        private ObservableCollection<BoxGroupDataGridView> listGroupData(List<BoxGroup> listGroup_)
        {
            //var listDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            SettingManager.Instance.ListGroupDataGridRow.Clear();
            foreach (var group in listGroup_)
            {
                var add = new BoxGroupDataGridView(group);
                add.Inital();

                //listDataGridRow.Add(add);
                SettingManager.Instance.ListGroupDataGridRow.Add(add);

                Console.WriteLine("■listGroupData add : name[{0}] id[{1}]", add.GroupName, add.GroupId);

            }
            //return listDataGridRow;
            return SettingManager.Instance.ListGroupDataGridRow;
        }

        // グループの作成
        public async Task<BoxGroup> CreateGroupName(string name_)
        {
            var request = new BoxGroupRequest() { Name = name_ };
            var rtn = await _client.GroupsManager.CreateAsync(request);

            Console.WriteLine("■ChangeGroupName:" + rtn.ToString());
            return rtn;
        }

        // グループの更新
        public async Task<BoxGroup> UpdateGroupName(string id_, string name_)
        {
            var request = new BoxGroupRequest() { Id = id_, Name = name_ };
            var rtn = await _client.GroupsManager.UpdateAsync(id_, request);

            Console.WriteLine("■UpdateGroupName:" + rtn.ToString());
            return rtn;
        }

        // グループのメンバー取得
        public async Task<BoxCollection<BoxGroupMembership>> ListMemberIdFromGroup(string gorupId_)
        {
            try
            {
                var listGroup = await _client.GroupsManager.GetAllGroupMembershipsForGroupAsync(gorupId_);

                return listGroup;

            }
            catch (Exception ex_)
            {
                Console.WriteLine("■ListGroupMemberId:" + ex_.ToString());
                return null;
            }
        }

        // ユーザー情報の取得
        public async Task<ObservableCollection<BoxUserDataGridView>> ListUserData()
        {
            try
            {
                var listUser = await _client.UsersManager.GetEnterpriseUsersAsync();

                ListUser = listUser.Entries;

            }
            catch (Exception ex_)
            {
                Console.WriteLine("■ListUserData:" + ex_.ToString());
            }
            return listUserData(ListUser);
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

        //public async Task DeletateAllUserGroups(string userId_, List<string> listGroupName_) 
        //{

        //    foreach (var name in listGroupName_)
        //    {
        //        var groupId = SettingManager.Instance.GetGroupId(name);
        //        await _client.GroupsManager.UpdateGroupMembershipAsync("33333");
        //    }

        //}

        public async Task UpadateGroup(string userId_, List<string> listGroupName_) 
        {
            //var updates = new BoxGroupMembershipRequest()
            //{
            //    Role = "member"
            //};
            //BoxGroupMembership updatedMembership = await _client.GroupsManager.UpdateGroupMembershipAsync("33333", updates);

            //var = _client.GroupsManager.GetAllGroupMembershipsForUserAsync(userId_);

            var list =new List<BoxGroupMembership>();
            foreach(var name in listGroupName_)
            {
                //var groupId = SettingManager.Instance.GetGroupId(name);
                //var group = SettingManager.Instance.GetGroupData(name);
                //list.Add(new BoxGroupMembership() { Group );
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
