using BoxNestGroup.GridView;
using BoxNestGroup.Manager;
using BoxNestGroup.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//using Microsoft.Exchange.WebServices.Data;

namespace BoxNestGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        public static MainWindow Instance { get; } = new MainWindow(); // App.xaml の編集が必要
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

        public const string MENU_USER_SEELCT = "選択";
        public const string MENU_USER_NAME = "ユーザー名";
        public const string MENU_USER_ID = "ユーザーID";
        public const string MENU_USER_MAIL ="メールアドレス";
        public const string MENU_USER_NOW = "現在[所　属]";
        public const string MENU_USER_ALL = "現在[全所属]";
        public const string MENU_USER_MOD = "変更[追　加]";
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
        private async void windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定(フォルダとBox比較)の読み込みはここ
            //SettingManager.Instance.LoadFile();
            foreach (var column in dataGridUser.Columns)
            {
                if (column.Header.ToString() == "変更[追　加]") 
                {
                    var textColumn = column as DataGridTextColumn;
                    textColumn.EditingElementStyle = (Style)this.Resources["dataGridUserTextBoxStyle"];
                }
            }

            //
            //menuMain.Items

            //

            await checkFolderToDataGridViewGroupData();
            renewFolderData();
        }

        /// <summary>
        /// データの表示クリア
        /// </summary>
        /// <param name="name_">ログイン名 of ファイル名</param>
        private void clearBox(string name_)
        {
            labelLogin.Content = "ログイン：" + name_;
            SettingManager.Instance.ListGroupDataGridRow.Clear();
            dataGridGroup.ItemsSource = SettingManager.Instance.ListGroupDataGridRow.ToList();
            SettingManager.Instance.ListUserDataGridRow.Clear();
            dataGridUser.ItemsSource = SettingManager.Instance.ListUserDataGridRow.ToList();
        }

        /// <summary>
        /// Box関係の表示更新
        /// </summary>
        /// <returns>なし</returns>
        private async Task renewBox()
        {
            await setLoginUserText();
            await renewListGroupData();
            await renewListUserData();
        }

        /// <summary>
        /// Boxからのログインの表示更新
        /// </summary>
        /// <returns>なし</returns>
        private async Task setLoginUserText() 
        {
            var userName = await BoxManager.Instance.LoginUserName();
            if (userName == string.Empty)
            {
                BoxManager.Instance.SetTokens(string.Empty, string.Empty);
                labelLogin.Content = "ログイン：" + "再取得";
                return ;
            }
            labelLogin.Content = "ログイン：" + userName;
        }
        /// <summary>
        /// フォルダツリーの表示更新
        /// </summary>
        private void renewFolderData()
        {
            treeViewFolder.ItemsSource = FolderManager.Instance.ListFolderTree.ToList();
        }
        /// <summary>
        /// Boxグループの表示更新
        /// </summary>
        /// <returns></returns>
        private async Task renewListGroupData()
        {
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();
        }

        /// <summary>
        /// Boxユーザーの表示更新
        /// </summary>
        /// <returns></returns>
        private async Task renewListUserData()
        {
            var listDataGridRow = await BoxManager.Instance.ListUserData();
            dataGridUser.ItemsSource = listDataGridRow.ToList();
        }

        private void dataGridGroupRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Console.WriteLine("■dataGridGroupRowEditEnding : {0} {1}", sender, e);
        }
        
        /// <summary>
        /// グループ名の編集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridGroupCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Console.WriteLine("■dataGridGroupCellEditEnding : {0} {1}", sender, e);

            var editBox = e.EditingElement as System.Windows.Controls.TextBox;

            var newGroupName = editBox.Text;
            var oldGroup = (e.Row.Item as BoxGroupDataGridView);

            if (oldGroup.GroupId == string.Empty)
            {
                return;
            }

            var result = System.Windows.MessageBox.Show("グループ名を変更しますか\n["+ oldGroup.GroupName+"]→[" + newGroupName+"]", "確認",MessageBoxButton.OKCancel);
            if(result != MessageBoxResult.OK)
            {
                editBox.Text = oldGroup.GroupName;
                return; 
            }

            Console.WriteLine("　dataGridGroupCellEditEnding  Id[{0}] old[{1}] -> new[{2}]", oldGroup.GroupId, oldGroup.GroupName, newGroupName);
            var rtn =BoxManager.Instance.UpdateGroupName(oldGroup.GroupId, newGroupName);
            FolderManager.Instance.UpdateFolder(oldGroup.GroupName, newGroupName);

            renewFolderData();
        }

        private void dataGridUserCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Console.WriteLine("■dataGridUserCellEditEnding : {0} {1}", sender, e);

            var editBox = e.EditingElement as System.Windows.Controls.TextBox;
            if (editBox == null)
            {
                return;
            }


            var row = e.Row.DataContext as BoxUserDataGridView;

            var modList = new List<string>(editBox.Text.Split("\n"));

            // 重複はHashSetで消えるから、そのまま追加
            var nestList = new HashSet<string>();
            nestList.UnionWith(FolderManager.Instance.ListUniqueGroup(modList));

            //row.ListNestGroup = string.Join("\n", new HashSet<string>(addList));
            row.ListModGroup = string.Join("\n", modList);
            row.ListNestGroup =string.Join("\n", nestList);

            var list = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
            dataGridUser.ItemsSource = list.ToList();
            dataGridUser.UpdateLayout();
        }

        /// <summary>
        /// フォルダのみあったらBoxグループの作成候補に追加
        /// </summary>
        private async Task checkFolderToDataGridViewGroupData()
        {
            // Boxにあるけどフォルダにない場合は、BoxのDataGridView更新時に作成している
            // Boxでのグループ名変更も更新している
            var listFolder = new HashSet<string>(FolderManager.Instance.ListFolderName);
            var listGroup = SettingManager.Instance.ListGroupDataGridRow;

            // Boxグループにあるフォルダ名は削除
            foreach (var group in listGroup)
            {
                listFolder.Remove(group.GroupName);
            }

            // 別のリストをコピーしてから置き換える
            var listGridView = new List<BoxGroupDataGridView>();
            var list =dataGridGroup.ItemsSource as List<BoxGroupDataGridView>;
            if(list !=null)
            {
                listGridView.AddRange(list);
            }
            foreach (var name in listFolder)
            {
                // 新規グループを作成
                var add = new BoxGroupDataGridView(name);
                await add.Inital();

                listGridView.Add(add);
            }
            dataGridGroup.ItemsSource = listGridView;
        }

        // ツリービューのドラッグアンドドロップ処理
        //private System.Windows.Point lastPoint;
        private void treeViewFolderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void treeViewFolderDrop(object sender, System.Windows.DragEventArgs e)
        {
        }

        private void treeViewFolderMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        /// <summary>
        /// DataGridの入力で改行するための処理
        /// 　Alt+Returnで改行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridUserTextColumnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && ( e.KeyboardDevice.Modifiers & ModifierKeys.Shift) >0)
            {
                Console.WriteLine("　DataGridUserTextColumnKeyDown : {0} {1}", sender, e);

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
    }
}