using System.IO;
using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Diagnostics;
using BoxNestGroup.Windows;
using ClosedXML.Excel;

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

        //public const string MENU_USER_SEELCT = "変更";
        //public const string MENU_USER_NAME = "ユーザー名";
        //public const string MENU_USER_ID = "ユーザーID";
        //public const string MENU_USER_MAIL ="メールアドレス";
        //public const string MENU_USER_NOW = "現在[所　属]";
        //public const string MENU_USER_ALL = "現在[全所属]";
        //public const string MENU_USER_MOD = "変更[所　属]";
        //public const string MENU_USER_NEST = "変更[全所属]";
        //public const string MENU_USER_USED = "容量制限";
        //public const string MENU_USER_COLABO = "外部コラボ";

        //public const string MENU_GROUP_NAME = "グループ名";
        //public const string MENU_GROUP_ID = "グループID";
        public const string MENU_GROUP_NEST_MAX = "ネスト最大数";
        public const string MENU_GROUP_FOLDER_NUM = "フォルダ数";
        public const string MENU_GROUP_USER_NUM = "ユーザー数";

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
            SettingManager.Instance.ListGroupIdName.SavaData();
        }


        /// <summary>
        /// データの表示クリア
        /// </summary>
        /// <param name="name_">ログイン名 of ファイル名</param>
        private void clearBox(string name_)
        {

            SettingManager.Instance.ListGroupDataGridView.Clear();
            SettingManager.Instance.ListUserDataGridView.Clear();
            SettingManager.Instance.ListMembershipGroupNameLogin.Clear();
            SettingManager.Instance.ListGroupIdName.Clear();
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
        /// DataGridの入力で改行するための処理
        /// 　Shift+Returnで改行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridUserTextColumnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && ( e.KeyboardDevice.Modifiers & ModifierKeys.Shift) >0)
            {
                Debug.WriteLine("　DataGridUserTextColumnKeyDown : {0} {1}", sender, e);

                var textBox = sender as System.Windows.Controls.TextBox;
                if (textBox == null) 
                {
                    return;
                }
                int pos = textBox.CaretIndex;
                textBox.Text = textBox.Text.Insert(pos, "\n");
                textBox.CaretIndex = pos+1;
                e.Handled = true;

            }
        }

        /// <summary>
        /// タブ移動した場合の表示更新(フィルター)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabControlBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■_tabControlBoxSelectionChanged : {0} {1}", sender, e);
        }

        /// <summary>
        /// 「設定」ボタン操作
        ///  設定画面の表示
        /// </summary>
        private void _buttonSettingsClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■settingsClick : {0} {1}", sender, e);
            //
            var win = new SettingsWindow();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示
        }

        /// <summary>
        /// 「About」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonAboutClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■settingsClick : {0} {1}", sender, e);
            //
            var win = new AboutWindow();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示
        }

        /// <summary>
        /// 「About」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonOpenFolderClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■openFolderClick : {0} {1}", sender, e);
            Debug.WriteLine("　openFolderClick open: {0}", FolderManager.Instance.CommonGroupFolderPath);
            //
            Process.Start("explorer.exe", FolderManager.Instance.CommonGroupFolderPath);
        }

        /// <summary>
        /// 「承認」ボタン操作
        ///  BoxへOAuth2承認
        /// </summary>
        private async void _buttonWebAuthClick(object sender, RoutedEventArgs e)
        {
            if (BoxManager.Instance.IsHaveClientID == false
             || BoxManager.Instance.IsSecretID == false)
            {
                System.Windows.MessageBox.Show("｢設定｣から｢アプリID｣と｢シークレットID｣を設定してください。", "注意");
                return;
            }

            Debug.WriteLine("■webAuthClick : {0} {1}", sender, e);
            //
            if (BoxManager.Instance.IsHaveAccessToken == false)
            {
                // トークンがない場合は取得
                var win = new BoxOauthWebWindow() { Owner = this };
                win.ShowDialog(); // モーダルで表示
            }

            var wait = new WaitWindow() { Owner = this };
            wait.Show();

            try
            {
                BoxManager.Instance.OAuthToken();
                await BoxManager.Instance.RefreshToken();

                //var box =await renewBox();
                await renewBox();

                _buttonWebAuth.IsEnabled = false;
                _buttonOffLine.IsEnabled = false;

            }
            catch (Exception ex)
            {
                // 承認失敗したらクリア
                Debug.WriteLine(ex.Message);
                BoxManager.Instance.SetTokens(string.Empty, string.Empty);

                clearBox("再取得");
            }

            wait.Close();
        }


        /// <summary>
        /// 「オフライン」ボタン処理
        /// 　オフライン(Excel)でやる場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonOffLineButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■renewUserButtonClick : {0} {1}", sender, e);

            var dlg = new OpenFileDialog();

            dlg.Filter = "EXCEL ファイル|*.xlsx";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var wait = new WaitWindow();
            wait.Owner = this;
            wait.Show();

            var path = dlg.FileName;
            var name = path.Substring(path.LastIndexOf("\\") + 1);

            clearBox(name);
            // ファイルを開く処理
            using (var workbook = new XLWorkbook(path))
            {
                var worksheet = workbook.Worksheet(1); // 1スタート(0なし)
                                                       // シート読み込み
                SettingManager.Instance.LoadExcelSheet(worksheet);
            }

            wait.Close();
        }
    }
}