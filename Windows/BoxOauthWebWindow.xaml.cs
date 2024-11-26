using BoxNestGroup.Managers;
using System.Net;
using System.Windows;
using System.Diagnostics;

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
        /// ログイン後の表示
        /// </summary>
        private string _htmlLogin = @"\html\login.html";

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

            setListner();

            setAuthBox();
        }

        /// <summary>
        /// listner(ユーザーコード受信)設定
        /// </summary>
        private void setListner() 
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format(Settings.Default.ListenUrl, Settings.Default.PortNumber));
            _listener.Start();
        }

        /// <summary>
        /// Box承認画面の表示
        /// </summary>
        private void setAuthBox()
        {
            var url = BoxManager.Instance.AuthorizationUrl;
            Debug.WriteLine("■ setAuthBox url : {0}", url);
            BoxOAuthWebBrowser.Source = new System.Uri(url);
        }

        /// <summary>
        /// 画面を閉じた時の処理
        /// 　開いたlisnerを閉じて削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener?.Close();
            _listener = null;
        }

        /// <summary>
        /// ユーザーコード受信待ち
        /// </summary>
        /// <returns>ユーザーコード</returns>
        private async Task<string> getUserCode()
        {
            try
            {
                var context = await _listener?.GetContextAsync() ?? null;
                Debug.WriteLine("■getUserCode StatusCode[{0}]", context.Response.StatusCode);

                if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var request = context.Request;

                    var rawUrl = request.RawUrl;    // 		RawUrl	"/callback?code=yWtJmL9c2xgtZd3V2jTwCDiMMASK0sF5"	string
                    var userCode = rawUrl.Replace(@"/callback?code=", "");

                    return userCode;
                }
            }
            catch (System.Exception ex_)
            {
                Debug.WriteLine("■getUserCode : {0}", ex_.ToString());
            }
            return string.Empty;
       }

        /// <summary>
        /// Box承認画面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void boxOAuthWebBrowserNavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_loadedTopPage == false)
            {
                _loadedTopPage = true;

                var userCode = await getUserCode();
                if (userCode != string.Empty)
                {
                    await BoxManager.Instance.CreateBoxClient(userCode);

                    _listener?.Stop();
                    
                    var currentFolder = System.IO.Directory.GetCurrentDirectory();
                    var uri = currentFolder + _htmlLogin;

                    uri ="file:///" + uri.Replace("\\","/");
                    Debug.WriteLine("■ setAuthBox url : {0}", uri);
                    BoxOAuthWebBrowser.Source = new System.Uri(uri);
                    

                    Thread.Sleep(1000);
                }

                this.Close();
            }
        }
        /*       
*/
    }
}
