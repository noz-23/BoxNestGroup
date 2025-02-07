using BoxNestGroup.Managers;
using BoxNestGroup.Properties;
using System.IO;
using System.Net;
using System.Windows;
using System.Diagnostics;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.InkML;
using BoxNestGroup.Files;

namespace BoxNestGroup
{
    /// <summary>
    /// BoxOauthWebWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BoxOauthWebWindow : Window
    {
        /// <summary>
        /// リダイレクト(localhost)の操作
        /// </summary>
        private HttpListener? _listener =null;

        /// <summary>
        /// 画面表示後の処理
        /// </summary>
        private bool _loadedTopPage =false;

        /// <summary>
        /// Box承認画面操作
        /// </summary>
        public BoxOauthWebWindow()
        {
            InitializeComponent();

            _setListner();

            _setAuthBox();
        }

        /// <summary>
        /// listner(ユーザーコード受信)設定
        /// </summary>
        private void _setListner() 
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format(Settings.Default.ListenUrl, Settings.Default.PortNumber));
            _listener.Start();
        }

        /// <summary>
        /// Box承認画面の表示
        /// </summary>
        private void _setAuthBox()
        {
            var url = BoxManager.Instance.AuthorizationUrl;
            LogFile.Instance.WriteLine($"■ setAuthBox url : {url}");
            _boxOAuthWebBrowser.Source = new System.Uri(url);
        }

        /// <summary>
        /// 画面を閉じた時の処理
        /// 　開いたlisnerを閉じて削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _windowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener?.Close();
            _listener = null;
        }

        /// <summary>
        /// ユーザーコード受信待ち
        /// </summary>
        /// <returns>ユーザーコード</returns>
        private async Task<string> _getUserCode()
        {
            try
            {
                if (_listener == null)
                {
                    return string.Empty;
                }

                var context = await _listener.GetContextAsync();
                if (context == null)
                {
                    return string.Empty;
                }
                LogFile.Instance.WriteLine($"StatusCode [{context.Response.StatusCode}]");

                if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var request = context.Request;

                    var rawUrl = request.RawUrl;    // 		RawUrl	"/callback?code=yWtJmL9c2xgtZd3V2jTwCDiMMASK0sF5"	string
                    var userCode = rawUrl?.Replace(@"/callback?code=", "")??string.Empty;

                    return userCode;
                }
            }
            catch (System.Exception ex_)
            {
                LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");
            }
            return string.Empty;
       }

        /// <summary>
        /// Box承認画面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _boxOAuthWebBrowserNavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_loadedTopPage == false)
            {
                _loadedTopPage = true;

                var userCode = await _getUserCode();
                if (userCode != string.Empty)
                {
                    await BoxManager.Instance.CreateBoxClient(userCode);

                    _listener?.Stop();
                    
                    var loginFile = System.IO.Directory.GetCurrentDirectory()+@"\"+ Properties.Resource.FILE_NAME_LOGIN_HTML;
                    //var uri = currentFolder + _htmlLogin;

                    //uri ="file:///" + uri.Replace(@"\","/");
                    //Debug.WriteLine("■ setAuthBox url : {0}", uri);
                    //BoxOAuthWebBrowser.Source = new System.Uri(uri);

                    _boxOAuthWebBrowser.NavigateToString(File.ReadAllText(File.ReadAllText(loginFile)));

                    Thread.Sleep(1000);
                }

                this.Close();
            }
        }
        /*       
*/
    }
}
