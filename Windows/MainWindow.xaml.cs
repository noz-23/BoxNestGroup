using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using BoxNestGroup.Properties;
using BoxNestGroup.Windows;
using ClosedXML.Excel;
using System.Windows;

namespace BoxNestGroup
{

    public delegate Task ReNewDelegate();
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        public static MainWindow Instance { get; private set; } = new MainWindow(); // App.xaml の編集が必要

        private MainWindow()
        {
            InitializeComponent();

            // https://tocsworld.wordpress.com/2014/08/13/%E5%A4%9A%E8%A8%80%E8%AA%9E%E5%AF%BE%E5%BF%9Cc%E3%80%81wpf%E7%B7%A8/
            var dictionary = new ResourceDictionary();
            //var uri= new Uri(Directory.GetCurrentDirectory()+ @"\Resources\StringResource.xaml", UriKind.Relative);
            //var uri = new Uri(@"/Resources/StringResource.xaml", UriKind.Relative);
            //var uri = new Uri(@"StringResource.xaml", UriKind.Relative);
            //dictionary.Source = uri;

            //this.Resources.MergedDictionaries.Add(dictionary);
            //
            //https://qiita.com/NumAniCloud/items/3d64199aee8876d53f67
            //https://qiita.com/aonim/items/36d3894c5fe721d9ab49

        }
        //public const string MENU_GROUP_NAME = "グループ名";
        //public const string MENU_GROUP_ID = "グループID";
        //public const string MENU_GROUP_NEST_MAX = "ネスト最大数";
        //public const string MENU_GROUP_FOLDER_NUM = "フォルダ数";
        //public const string MENU_GROUP_USER_NUM = "ユーザー数";

        /// <summary>
        /// 起動後の処理
        /// 　表示関係はここで処理しないと出ない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定(フォルダとBox比較)の読み込みはここ
        }
        private void _windowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
            //SettingManager.Instance.ListGroupIdName.SavaData();
            SettingManager.Instance.ListXmlGroupTreeView.Save();
        }


        /// <summary>
        /// データの表示クリア
        /// </summary>
        private void clearBox()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                SettingManager.Instance.ListGroupDataGridView.Clear();
                SettingManager.Instance.ListUserDataGridView.Clear();
                SettingManager.Instance.ListMembershipGroupNameLogin.Clear();
            });
        }

        /// <summary>
        /// Box関係の表示更新
        /// </summary>
        /// <returns>なし</returns>
        //private async Task<string> renewBox()
        private async Task renewBox()
        {
            // タブ移動でDavaViewでエラーになるため

            await BoxManager.Instance.GetLoginUser();
            await BoxManager.Instance.GetListGroupData();
            await BoxManager.Instance.GetListUserData();
        }

        /// <summary>
        /// タブ移動した場合の表示更新(フィルター)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabControlBoxSelectionChanged(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");
        }

        /// <summary>
        /// 「設定」ボタン操作
        ///  設定画面の表示
        /// </summary>
        private void _buttonSettingsClick(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");
            //
            var win = new SettingsWindow();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示
        }

        /// <summary>
        /// 「About」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonAboutClick(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");
            //
            var win = new AboutWindow();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示
        }

        /// <summary>
        /// 「About」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonOpenFolderClick(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");
            //Debug.WriteLine("　openFolderClick open: {0}", FolderManager.Instance.CommonGroupFolderPath);
            //
            //Process.Start("explorer.exe", FolderManager.Instance.CommonGroupFolderPath);
        }

        /// <summary>
        /// 「承認」ボタン操作
        ///  BoxへOAuth2承認
        /// </summary>
        private void _buttonWebAuthClick(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");

            if (BoxManager.Instance.IsHaveClientID == false
             || BoxManager.Instance.IsSecretID == false)
            {
                System.Windows.MessageBox.Show("｢設定｣から｢アプリID｣と｢シークレットID｣を設定してください。", "注意");
                return;
            }
            //
            if (BoxManager.Instance.IsHaveAccessToken == false)
            {
                // トークンがない場合は取得
                var win = new BoxOauthWebWindow() { Owner = this };
                win.ShowDialog(); // モーダルで表示
            }

            var wait = new WaitWindow() { Owner = this };
            wait.Run += async(win_)=>
            {
                try
                {
                    BoxManager.Instance.OAuthToken();
                    await BoxManager.Instance.RefreshToken();

                    //var box =await renewBox();
                    await renewBox();

                    _buttonWebAuth.IsEnabled = false;
                    _buttonOffLine.IsEnabled = false;

                }
                catch (Exception ex_)
                {
                    // 承認失敗したらクリア
                    LogFile.Instance.WriteLine($"Exception [{ex_.Message}]");

                    BoxManager.Instance.SetTokens(string.Empty, string.Empty);

                    clearBox();
                }

            };
            wait.ShowDialog();
        }


        /// <summary>
        /// 「オフライン」ボタン処理
        /// 　オフライン(Excel)でやる場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _buttonOffLineButtonClick(object sender_, RoutedEventArgs e_)
        {
            LogFile.Instance.WriteLine($"[{sender_.ToString()}] [{e_.ToString()}]");

            var dlg = new OpenFileDialog();

            dlg.Filter = "EXCEL ファイル|*.xlsx";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var wait = new WaitWindow() { Owner = this };

            wait.Run += async (win_) =>
            {
                var path = dlg.FileName;

                clearBox();
                // ファイルを開く処理
                using (var workbook = new XLWorkbook(path))
                {
                    var worksheet = workbook.Worksheet(1); // 1スタート(0なし)
                                                           // シート読み込み
                    SettingManager.Instance.LoadExcelSheet(worksheet);
                }
                await Task.Delay(500);
            };

            wait.ShowDialog();
            await Task.Delay(500);
        }
    }
}