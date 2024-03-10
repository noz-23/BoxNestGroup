using Box.V2.Config;
using Box.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Security.Policy;
using System.Net.Http;
using System.IO;
using BoxNestGroup.Manager;

namespace BoxNestGroup
{
    /// <summary>
    /// BoxOauthWebWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BoxOauthWebWindow : Window
    {
        private HttpListener _listener =null;
        private string _htmlLogin = @"\html\login.html";

        private bool _loadedTopPage = false;
        private bool _loadedEndPage = false;

        public BoxOauthWebWindow()
        {
            InitializeComponent();

            setListner();

            setAuthBox();
        }

        private void setListner() 
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format(Settings.Default.ListenUrl, Settings.Default.PortNumber));
            _listener.Start();
        }

        private void setAuthBox()
        {
            BoxOAuthWebBrowser.Navigate(BoxManager.Instance.AuthorizationUrl);
        }

        private async void boxOAuthWebBrowserNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_loadedTopPage == false)
            {
                _loadedTopPage = true;

                //var userCode = await getUserCode();

                await BoxManager.Instance.CreateBoxClient(_listener);
                //var context = await _listener.GetContextAsync();

                _listener.Stop();
                _listener.Close();
                _listener = null;


                var currentFolder = Directory.GetCurrentDirectory();
                BoxOAuthWebBrowser.Navigate(currentFolder + _htmlLogin);

                Thread.Sleep(1000);


                await MainWindow.Instance.SetLoginUserText();
                this.Close();

                return;
            }
        }
/*       
        private async Task<string> getUserCode()
        {

            var context = await _listener.GetContextAsync();
            var request = context.Request;

            var rawUrl = request.RawUrl;    // 		RawUrl	"/callback?code=yWtJmL9c2xgtZd3V2jTwCDiMMASK0sF5"	string
            var userCode = rawUrl.Replace(@"/callback?code=", "");

            return userCode;
        }
*/
    }
}
