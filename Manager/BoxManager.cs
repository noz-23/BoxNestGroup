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


/*
 * https://github.com/box/box-windows-sdk-v2
 */

namespace BoxNestGroup.Manager
{
    internal class BoxManager
    {
        static public BoxManager Instance { get; } = new BoxManager();

        private BoxClient _client ;
        private BoxConfig _config ;

        /*
        private class Token
        {
            public string access_token { get; set; }
        }
        */

        private string _authEnticationUrl = "https://api.box.com/oauth2/token";
        private string _clientID = Settings.Default.ClientID;
        private string _sercretID = Settings.Default.SecretID;

        // 参照 URL
        public string AuthorizationUrl { get { return _config.AuthCodeUri.ToString(); } }

        //private string _userToken = "";
        private BoxManager()
        {
            var redirectUri = new Uri(string.Format(Settings.Default.RedirectUrl, Settings.Default.PortNumber));

            _config = new BoxConfig(_clientID, _sercretID, redirectUri);
            _client = new BoxClient(_config);

            if (IsHaveAccessToken ==true)
            {
                var userToken = Settings.Default.UserToken;
                var session = new OAuthSession(userToken, "N/A", 3600, "bearer");
                _client =new BoxClient(_config, session);
            }
        }
        public async Task CreateBoxClient(HttpListener listener_)
        {
            var userCode = await getUserCode(listener_);

            var session = await _client.Auth.AuthenticateAsync(userCode);
            _client = new BoxClient(_config, session);

            setUserToken(_client.Auth.Session.AccessToken);
        }
        public async Task CreateBoxClient(string userCode_)
        {
            var session = await _client.Auth.AuthenticateAsync(userCode_);
            _client = new BoxClient(_config, session);

            setUserToken(_client.Auth.Session.AccessToken);
        }

        // アクセストークンを持ってるか？
        public bool IsHaveAccessToken
        {
            get
            {
                var userToken = Settings.Default.UserToken;
                return (userToken != string.Empty);
            }
        }

        // リフレッシュトークン取得
        private async Task<string> refreshToken()
        {
            try
            {
                var userToken = Settings.Default.UserToken;
                var newSession = await _client.Auth.RefreshAccessTokenAsync(userToken);

                _client = new BoxClient(_config, newSession);

                var currentUser = await _client.UsersManager.GetCurrentUserInformationAsync();
                return currentUser.Name;
            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_);
                return "再取得";
            }
            return string.Empty;
        }


        private async Task<string> getUserCode(HttpListener listener_)
        {

            var context = await listener_.GetContextAsync();
            var request = context.Request;

            var rawUrl = request.RawUrl;    // 		RawUrl	"/callback?code=yWtJmL9c2xgtZd3V2jTwCDiMMASK0sF5"	string
            var userCode = rawUrl.Replace(@"/callback?code=", "");

            return userCode;
        }

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
        private void setUserToken(string userToken_)
        {
            Settings.Default.UserToken = userToken_;
            Settings.Default.Save();
        }

        // ログインしてるユーザー名
        public async Task<string> LoginUserName()
        {
            try
            {
                var currentUser = await _client.UsersManager.GetCurrentUserInformationAsync();
                return currentUser.Name;
            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_);
                return await refreshToken();
            }
            return string.Empty;
        }

        public async Task<ObservableCollection<BoxGroupDataGridView>> ListGroupData()
        {
            var listDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            try
            {

                var listGroup = await _client.GroupsManager.GetAllGroupsAsync();

                foreach ( var group in listGroup.Entries) 
                {
                    var addData = new BoxGroupDataGridView();

                    addData.GroupName = group.Name;
                    addData.GroupId =group.Id;

                    listDataGridRow.Add(addData);
                }

            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_);

            }
            return listDataGridRow;
        }

        public async Task CreateGroup(string name_)
        {
            var request = new BoxGroupRequest() { Name = name_ };
            await _client.GroupsManager.CreateAsync(request);
        }
    }
}
