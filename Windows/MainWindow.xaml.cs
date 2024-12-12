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
        public static MainWindow Instance { get; } = new MainWindow(); // App.xaml の編集が必要

        /// <summary>
        /// Box読み込み中か
        /// </summary>
        //private bool _loadbox = false;

        private MainWindow()
        {
            InitializeComponent();

            // 表示後の処理
            Loaded += windowLoaded;
            Closed += windowClosed;
        }

        private void windowClosed(object? sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        public const string MENU_USER_SEELCT = "変更";
        public const string MENU_USER_NAME = "ユーザー名";
        public const string MENU_USER_ID = "ユーザーID";
        public const string MENU_USER_MAIL ="メールアドレス";
        public const string MENU_USER_NOW = "現在[所　属]";
        public const string MENU_USER_ALL = "現在[全所属]";
        public const string MENU_USER_MOD = "変更[所　属]";
        public const string MENU_USER_NEST = "変更[全所属]";
        public const string MENU_USER_USED = "容量制限";
        public const string MENU_USER_COLABO = "外部コラボ";

        public const string MENU_GROUP_NAME = "グループ名";
        public const string MENU_GROUP_ID = "グループID";
        public const string MENU_GROUP_NEST_MAX = "ネスト最大数";
        public const string MENU_GROUP_FOLDER_NUM = "フォルダ数";
        public const string MENU_GROUP_USER_NUM = "ユーザー数";

        /// <summary>
        /// 起動後の処理
        /// 　表示関係はここで処理しないと出ない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定(フォルダとBox比較)の読み込みはここ
            //foreach (var column in _dataGridUser.Columns)
            //{
            //    if (column.Header.ToString() == "変更[追　加]") 
            //    {
            //        var textColumn = column as DataGridTextColumn;
            //        textColumn.EditingElementStyle = (Style)this.Resources["_dataGridUserTextBoxStyle"];
            //    }
            //}

            //await setView();
        }

        /// <summary>
        /// データの表示クリア
        /// </summary>
        /// <param name="name_">ログイン名 of ファイル名</param>
        private void clearBox(string name_)
        {
            //textLogin.Content = "ログイン：" + name_;
            //SettingManager.Instance.ListGroupDataGridRow.Clear();
            SettingManager.Instance.ListGroupDataGridView.Clear();

            //SettingManager.Instance.ListUserDataGridRow.Clear();
            SettingManager.Instance.ListUserDataGridView.Clear();
        }

        /// <summary>
        /// Box関係の表示更新
        /// </summary>
        /// <returns>なし</returns>
        //private async Task<string> renewBox()
        private async Task renewBox()
        {
            // タブ移動でDavaViewでエラーになるため
            //_loadbox = true;

            //var logoin =await BoxManager.Instance.LoginUserName();
            await BoxManager.Instance.LoginUserName();

            //var groups =await BoxManager.Instance.ListGroupData();
            await BoxManager.Instance.ListGroupData();
            //var users =await BoxManager.Instance.ListUserData();
            await BoxManager.Instance.ListUserData();

            //var rtn = string.Format("login[{0}] groups.count[{1}] users.count[{2}]", logoin, groups.Count, users.Count);
            //_loadbox = false;
            //return rtn;
        }

        /// <summary>
        /// 表示の更新
        /// </summary>
        /// <returns></returns>
        //private async Task setView()
        //{
        //    if (_loadbox == true)
        //    {
        //        return;
        //    }
        //    //setLoginUserText();
        //    //await setListGroupData();
        //    setListUserData();
        //    //setFolderData();
        //}

        /// <summary>
        /// Boxからのログインの表示更新
        /// </summary>
        /// <returns>なし</returns>
        //private void setLoginUserText() 
        //{
        //    //var userName = await BoxManager.Instance.LoginUserName();
        //    var userName = SettingManager.Instance.LoginName;
        //    //
        //    if (userName == string.Empty)
        //    {
        //        BoxManager.Instance.SetTokens(string.Empty, string.Empty);
        //        //textLogin.Content = "ログイン：" + "再取得";
        //        return ;
        //    }
        //    //textLogin.Content = "ログイン：" + userName;
        //}
        /// <summary>
        /// フォルダツリーの表示更新
        /// </summary>
        //private void setFolderData()
        //{
        //    treeViewFolder.ItemsSource = FolderManager.Instance.ListFolderTree.ToList();
        //}
        /// <summary>
        /// Boxグループの表示更新
        /// </summary>
        /// <returns></returns>
        //private async Task setListGroupData()
        //{
        //    //var listFolder = new HashSet<string>(FolderManager.Instance.ListFolderName);
        //    var listFolder = new HashSet<string>(FolderManager.Instance.ListFolderTree.Select(x=>x.GroupName));

        //    // Boxグループにあるフォルダ名は削除
        //    foreach (var group in SettingManager.Instance.ListGroupDataGridRow)
        //    {
        //        listFolder.Remove(group.GroupName);
        //    }

        //    // 別のリストをコピーしてから置き換える
        //    foreach (var name in listFolder)
        //    {
        //        // 新規グループを作成
        //        var add = new BoxGroupDataGridView(name);
        //        await add.Inital();

        //        SettingManager.Instance.ListGroupDataGridRow.Add(add);
        //    }

        //    // フィルター処理のため
        //    var src =new CollectionViewSource();
        //    src.Source = SettingManager.Instance.ListGroupDataGridRow;
        //    src.View.Filter =(item) => { return isGroupNameFilter((item as BoxGroupDataGridView).GroupName);};
        //    dataGridGroup.ItemsSource = src.View;
        //    //
        //}

        /// <summary>
        /// Boxユーザーの表示更新
        /// </summary>
        /// <returns></returns>
        //private void setListUserData()
        //{
        //    var listDataGridRow = SettingManager.Instance.ListUserDataGridRow;

        //    // フィルター処理のため
        //    var src = new CollectionViewSource();
        //    src.Source = listDataGridRow.ToList();
        //    src.View.Filter = (item) => { return isUserNameFilter((item as BoxUserDataGridView).UserName); };
        //    dataGridUser.ItemsSource = src.View;
        //    //
        //}

        //private void _dataGridGroupRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    Debug.WriteLine("■_dataGridGroupRowEditEnding : {0} {1}", sender, e);
        //}
        
        /// <summary>
        /// ユーザーのグループ更新した時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dataGridUserCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    Debug.WriteLine("■dataGridUserCellEditEnding : {0} {1}", sender, e);

        //    var editBox = e.EditingElement as System.Windows.Controls.TextBox;
        //    if (editBox == null)
        //    {
        //        return;
        //    }

        //    var modList = new List<string>(editBox.Text.Split("\n"));

        //    // 重複はHashSetで消えるから、そのまま追加
        //    var nestList = new HashSet<string>();
        //    nestList.UnionWith(FolderManager.Instance.ListUniqueGroup(modList));

        //    var row = e.Row.DataContext as BoxUserDataGridView;

        //    var list =new List<BoxUserDataGridView>(SettingManager.Instance.ListUserDataGridRow.ToList());
        //    var find =list.Find( (f)=> f.UserMailAddress ==row.UserMailAddress );
        //    if (find == null)
        //    {
        //        return;
        //    }
        //    find.ListModGroup = string.Join("\n", modList);
        //    find.ListChild =string.Join("\n", nestList);

        //    setListUserData();
        //}

        // ツリービューのドラッグアンドドロップ処理
        //private System.Windows.Point lastPoint;
        private void _treeViewFolderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void _treeViewFolderDrop(object sender, System.Windows.DragEventArgs e)
        {
        }

        private void _treeViewFolderMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
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
/*
        /// <summary>
        /// ユーザー名フィルタ
        /// </summary>
        /// <returns>フィルタ( or like 接続)</returns>
        private bool isUserNameFilter(string name_)
        {
            var list = new List<string>(_textBoxUserNameFilter.Text.Split("\n"));

            list.Remove(string.Empty);

            if (list.Count() == 0)
            {
                // フィルタ設定がない場合は表示
                return true;
            }

            var flg = false;
            foreach (var match in list)
            {
                if (match.ToString() == string.Empty)
                {
                    continue;
                }
                flg |= Regex.IsMatch(name_, match);
            }
            return flg;

        }

        /// <summary>
        /// グループ名フィルタ
        /// </summary>
        /// <returns>フィルタ( or like 接続)</returns>
        private bool isGroupNameFilter( string name_)
        {
            var list = new List<string>(textBoxGroupNameFilter.Text.Split("\n"));

            list.Remove(string.Empty);

            if (list.Count== 0)
            {
                // フィルタ設定がない場合は表示
                return true;
            }

            var flg = false;
            foreach (var match in list)
            {
                if (match.ToString() == string.Empty)
                {
                    continue;
                }
                flg |= Regex.IsMatch(name_, match);
            }
            return flg;
        }
*/
        /// <summary>
        /// タブ移動した場合の表示更新(フィルター)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabControlBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("■_tabControlBoxSelectionChanged : {0} {1}", sender, e);
            //await setView();
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
                //Debug.WriteLine("　webAuthClick box:{0}", box);

                //await setView();

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
                //
                /*
                                //_buttonMakeGroup.IsEnabled = false;
                                _buttonMakeUser.IsEnabled = false;
                                //_buttonRenewUser.IsEnabled = false;
                                _buttonOffLine.IsEnabled = false;
                                _buttonWebAuth.IsEnabled = false;
                */
            }
            //await setView();

            wait.Close();
        }

    }
}