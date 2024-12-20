﻿using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Models;
using Box.V2.Models.Request;
using BoxNestGroup.GridView;
using BoxNestGroup.View;
using System.Collections.ObjectModel;


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
        public string AuthorizationUrl
        { 
            get
            { 
                return (_config !=null) ?_config.AuthCodeUri.ToString():string.Empty;
            }
        }


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
            Console.WriteLine("■BoxManager CreateBoxClient userCode_:{0}", userCode_);
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
            catch (Exception ex)
            {
                Console.WriteLine("　BoxManager CreateBoxClient:{0}", ex.Message);
            }

            SetTokens(_client.Auth.Session.AccessToken, _client.Auth.Session.RefreshToken);
        }

        /// <summary>
        /// アクセストークンを取得済みか(true:取得済み)
        /// </summary>
        public bool IsHaveAccessToken
        {
            get
            {
                var userToken = Settings.Default.AccessToken;
                Console.WriteLine("■BoxManager IsHaveAccessToken:{0}", userToken);
    
                switch (userToken)
                {
                    case "N/A":
                    case "":
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// リフレッシュトークンを取得済みか(true:取得済み)
        /// </summary>
        public bool IsHaveRefreshToken
        {
            get
            {
                var refreshToken = Settings.Default.RefreshToken;
                Console.WriteLine("■BoxManager IsHaveRefreshToken:{0}", refreshToken);

                switch (refreshToken)
                {
                    case "N/A":
                    case "":
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 承認接続処理
        /// </summary>
        public void OAuthToken()
        {
            // トークンがあればそのトークンを使って再セッション
            var accessToken = Settings.Default.AccessToken;
            string refreshToken = (IsHaveRefreshToken == false) ? "" : Settings.Default.RefreshToken;
            Console.WriteLine("■BoxManager OAuthToken : AccessToken[{0}] RefreshToken[{1}]", accessToken, refreshToken);

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
            Console.WriteLine("■BoxManager RefreshToken");
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
            Console.WriteLine("■BoxManager SeTokens Access[{0}] Refresh[{1}]", accessToken_, refreshToken_);

            Settings.Default.AccessToken = accessToken_;
            Settings.Default.RefreshToken = refreshToken_;

            Settings.Default.Save();
        }

        /// <summary>
        /// ログインユーザ名の取得
        /// </summary>
        /// <returns>ユーザ名</returns>
        public async Task<string> LoginUserName()
        {
            if (_client != null) 
            {
                try
                {
                    var currentUser = await _client.UsersManager.GetCurrentUserInformationAsync();
                    Console.WriteLine("■BoxManager LoginUserName:" + currentUser.Name);

                    SettingManager.Instance.LoginName = currentUser.Name;

                    return SettingManager.Instance.LoginName;
                }
                catch (Exception ex_)
                {
                    Console.WriteLine(ex_);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// グループデータの取得
        /// </summary>
        /// <returns>全グループの取得(データービュー)</returns>
        public async Task<ObservableCollection<BoxGroupDataGridView>> ListGroupData()
        {
            Console.WriteLine("■BoxManager ListGroupData");
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
                    Console.WriteLine("　ListGroupData:" + ex_.ToString());
                }
            }
            return new ObservableCollection<BoxGroupDataGridView>();
        }
        /// <summary>
        /// グループデータの取得 Box から View 変換
        /// </summary>
        /// <param name="listGroup_"></param>
        /// <returns>データービューでのグループデータ</returns>
        private async Task<ObservableCollection<BoxGroupDataGridView>> listGroupData(List<BoxGroup> listGroup_)
        {
            SettingManager.Instance.ListGroupDataGridRow.Clear();
            foreach (var group in listGroup_)
            {
                var add = new BoxGroupDataGridView(group);
                await add.Inital();

                SettingManager.Instance.ListGroupDataGridRow.Add(add);
                Console.WriteLine("■BoxManager listGroupData add : name[{0}] id[{1}]", add.GroupName, add.GroupId);

            }
            return SettingManager.Instance.ListGroupDataGridRow;
        }

        /// <summary>
        /// Boxにグループを作成
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>作成したBoxグループ</returns>
        public async Task<BoxGroup> CreateGroup(string groupName_)
        {
            Console.WriteLine("■BoxManager CreateGroup [{0}]", groupName_);
            if (_client != null)
            {

                try
                {
                    var request = new BoxGroupRequest() { Name = groupName_ };
                    return await _client.GroupsManager.CreateAsync(request);
                }
                catch (Exception ex_)
                {
                    Console.WriteLine("　BoxManager CreateGroup:" + ex_.ToString());
                }
            }
            return new BoxGroup();
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
            Console.WriteLine("■UpdateGroupName : id[{0}] name[{0}]", groupId_, groupName_);
            if (_client != null)
            {
                try
                {
                    var request = new BoxGroupRequest() { Id = groupId_, Name = groupName_ };
                    return await _client.GroupsManager.UpdateAsync(groupId_, request);
                }
                catch (Exception ex_)
                {
                    Console.WriteLine("　UpdateGroupName:" + ex_.ToString());
                }
            }

            return new BoxGroup();
        }

        /// <summary>
        /// グループのメンバー取得
        /// </summary>
        /// <param name="gorupId_">グループID</param>
        /// <returns>所属するメンバシップ(グループとユーザの紐付け)の取得</returns>
        public async Task<BoxCollection<BoxGroupMembership>> ListMemberIdFromGroup(string gorupId_)
        {
            Console.WriteLine("■BoxManager ListMemberIdFromGroup : [{0}]", gorupId_);
            if (_client != null)
            {
                try
                {
                    return await _client.GroupsManager.GetAllGroupMembershipsForGroupAsync(gorupId_);
                }
                catch (Exception ex_)
                {
                    Console.WriteLine("　BoxManager ListMemberIdFromGroup:" + ex_.ToString());
                }
            }
            return new BoxCollection<BoxGroupMembership>();
        }

        /// <summary>
        /// ユーザーデータの取得
        /// </summary>
        /// <returns>全ユーザーの取得(データービュー)</returns>
        public async Task<ObservableCollection<BoxUserDataGridView>> ListUserData()
        {
            Console.WriteLine("■BoxManager ListUserData");
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
                    Console.WriteLine("　ListUserData:" + ex_.ToString());
                }
            }
            return new ObservableCollection<BoxUserDataGridView>();
        }

        /// <summary>
        /// ユーザー情報の取得 Box から Viewへ変換
        /// </summary>
        /// <param name="listUser_">ユーザーリスト</param>
        /// <returns>全ユーザーの取得(データービュー)</returns>
        private ObservableCollection<BoxUserDataGridView> listUserData(List<BoxUser> listUser_)
        {
            SettingManager.Instance.ListUserDataGridRow.Clear();
            foreach (var user in listUser_)
            {
                var add = new BoxUserDataGridView(user);
                SettingManager.Instance.ListUserDataGridRow.Add(add);
                Console.WriteLine("■BoxManager listUserData add : name[{0}] id[{1}]", add.UserName, add.UserId);

            }
            return SettingManager.Instance.ListUserDataGridRow;
        }

        /// <summary>
        /// グループへユーザを追加
        /// </summary>
        /// <param name="userId_">追加するユーザーID</param>
        /// <param name="listGroupId_">追加するグループIDのリスト</param>
        public async Task AddGroupUser(string userId_, IList<string> listGroupId_) 
        {
            Console.WriteLine("■BoxManager AddGroupUser userId_[{0}] listGroupId_[{1}]", userId_,string.Join(",", listGroupId_));
            if (_client != null)
            {
                foreach (var groupId in listGroupId_)
                {
                    var request = new BoxGroupMembershipRequest()
                    {
                        User = new BoxRequestEntity() { Id = userId_ },
                        Group = new BoxGroupRequest() { Id = groupId }
                    };

                    try
                    {
                        await _client.GroupsManager.AddMemberToGroupAsync(request);
                    }
                    catch (Exception ex_)
                    {
                        Console.WriteLine("　AddGroupUser:" + ex_.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// メンバシップ(グループとユーザーの紐付け)削除
        /// </summary>
        /// <param name="list_">削除するメンバシップ(グループとユーザーの紐付け)のリスト</param>
        public async Task DeleteGroupUser(IList<BoxGroupMembership> list_)
        {
            Console.WriteLine("■BoxManager DeleteGroupUser");
            if (_client != null)
            {
                foreach (var member in list_)
                {
                    Console.WriteLine("　BoxManager DeleteGroupUser id[{0}] group[{1}] user[{2}]", member.Id, member.Group.Id, member.User.Id);
                    try
                    {
                        var rtn = await _client.GroupsManager.DeleteGroupMembershipAsync(member.Id);
                    }
                    catch (Exception ex_)
                    {
                        Console.WriteLine("　BoxManager DeleteGroupUser:" + ex_.ToString());
                    }
                }
            }
        }

    }
}
