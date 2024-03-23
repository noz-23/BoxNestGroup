using BoxNestGroup.GridView;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BoxNestGroup.Manager;
using BoxNestGroup.View;
using Org.BouncyCastle.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BoxNestGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; } = new MainWindow(); // App.xaml の編集が必要

        private MainWindow()
        {
            InitializeComponent();

            Loaded += windowLoaded;

            //dataGridGroup.ItemsSource = new ObservableCollection<BoxGroupDataGridView>();
            //var listGroupDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            //dataGridGroup.ItemsSource = listGroupDataGridRow.ToList();

            //var listUserDataGridRow = new ObservableCollection<BoxUserDataGridView>();
            //dataGridUser.ItemsSource = listUserDataGridRow.ToList();

            //
            //var listTreeViewRow = new ObservableCollection<FolderGroupTreeView>();
            //treeViewFolder.ItemsSource = listTreeViewRow.ToList();
            //SettingManager.Instance.Load();

        }

        private async void windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定の読み込みはここ
            SettingManager.Instance.Load();
            //

            //if (BoxManager.Instance.IsHaveAccessToken == true)
            //{
            //    await SetLoginUserText();

            //    var listGroup =await renewListGroupData();
            //    await renewListUserData();
            //}
            //else
            //{
            //    labelLogin.Content = "ログイン：" + "再取得";

            //    var listGroupDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            //    dataGridGroup.ItemsSource = listGroupDataGridRow.ToList();

            //    var listUserDataGridRow = new ObservableCollection<BoxUserDataGridView>();
            //    dataGridUser.ItemsSource = listUserDataGridRow.ToList();
            //}
            await renewBox();
            renewFolderData();

            checkFolderToDataGridViewGroupData();

            //throw new NotImplementedException();
        }

        private async void webAuthClick(object sender, RoutedEventArgs e)
        {
            var win = new BoxOauthWebWindow();
            //win.Show();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示

            await renewBox();
            renewFolderData();

            checkFolderToDataGridViewGroupData();
        }
        private void settingsClick(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow();
            //win.Show();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示
        }
        private async void aboutClick(object sender, RoutedEventArgs e)
        {
            //await SetLoginUserText();
        }

        private async Task renewBox()
        {
            if (BoxManager.Instance.IsHaveAccessToken == true)
            {
                var flg=await setLoginUserText();
                if (flg == true)
                {
                    var listGroup = await renewListGroupData();
                    await renewListUserData();
                    return;
                }

            }
            labelLogin.Content = "ログイン：" + "再取得";
            var listGroupDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            dataGridGroup.ItemsSource = listGroupDataGridRow.ToList();

            var listUserDataGridRow = new ObservableCollection<BoxUserDataGridView>();
            dataGridUser.ItemsSource = listUserDataGridRow.ToList();
        }

        private async Task<bool> setLoginUserText() 
        {
            try
            {
                var userName = await BoxManager.Instance.LoginUserName();
                labelLogin.Content = "ログイン：" + userName;
                return true;
            }
            catch (Exception ex_)
            {
                Console.WriteLine(ex_.ToString());
                //
                var flg = await BoxManager.Instance.RefreshToken();
                if (flg == true)
                {
                    var userName = await BoxManager.Instance.LoginUserName();
                    labelLogin.Content = "ログイン：" + userName;
                    return true;
                }
                else
                {
                    labelLogin.Content = "ログイン：" + "再取得";
                    return false;
                }
            }
        }

        //private async Task setGroupGridView()
        //{
        //    var userName = await BoxManager.Instance.LoginUserName();
        //}

        //private async void renewFoloderButtonClick(object sender, RoutedEventArgs e)
        //{
        //    renewFolderData();
        //    renewListGroupData();
        //}

        private async void makeGroupButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (BoxGroupDataGridView group in dataGridGroup.ItemsSource)
            {
                if (group.GroupId != string.Empty)
                {
                    continue;
                }

                var rtn =await BoxManager.Instance.CreateGroupName(group.GroupName);
                if (FolderManager.Instance.IsHaveGroup(group.GroupName) ==false)
                {
                    FolderManager.Instance.CreateFolder(rtn);
                    renewFolderData();
                }
            }
            var list = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = list.ToList();
        }

        //private async void renewGroupButtonClick(object sender, RoutedEventArgs e)
        //{
        //    //renewGroup();
        //    //renewFolder();
        //}


        private async void makeUserButtonClick(object sender, RoutedEventArgs e)
        {
        }
        private async void renewUserButtonClick(object sender, RoutedEventArgs e)
        {
        }

        /*
         * 更新フォルダ
         *  引数　：なし
         *  戻り値：なし
         */
        private void renewFolderData()
        {
            var list = FolderManager.Instance.ListCommonFolder();
            treeViewFolder.ItemsSource = list.ToList();
        }
        /*
         * 更新グループ
         *  引数　：なし
         *  戻り値：なし
         */
        private async Task<ObservableCollection<BoxGroupDataGridView>> renewListGroupData()
        {
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();

            return listDataGridRow;
        }

        private async Task renewListUserData()
        {
        }

        private void dataGridGroupRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var newValue = e.Row;
            var oldValue = e.Row.Item;

            Console.WriteLine("{0}", e);

        }

        private async void dataGridGroupCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var newGroupName = (e.EditingElement as System.Windows.Controls.TextBox).Text;
            var oldGroup = (e.Row.Item as BoxGroupDataGridView);

            if (oldGroup.GroupId == string.Empty)
            {
                return;
            }

            Console.WriteLine("dataGridGroupCellEditEnding  Id[{0}] old[{1}] -> new[{2}]", oldGroup.GroupId, oldGroup.GroupName, newGroupName);
            var rtn =BoxManager.Instance.UpdateGroupName(oldGroup.GroupId, newGroupName);
            FolderManager.Instance.UpdateFolder(oldGroup.GroupName, newGroupName);

            renewFolderData();

        }

        private void treeViewFolderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeViewFolderDrop(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void treeViewFolderMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        // フォルダのみあったら作成候補に追加
        private void checkFolderToDataGridViewGroupData()
        {
            // Boxにあるけどフォルダにない場合は、BoxのDataGridView更新時に作成している
            var listFolder = new HashSet<string>( FolderManager.Instance.ListFolderName);
            var listBox = BoxManager.Instance.ListGroup;

            foreach ( var group in listBox)
            {
                listFolder.Remove(group.Name);
            }

            var listGridView = new List< BoxGroupDataGridView >(dataGridGroup.ItemsSource as List<BoxGroupDataGridView>);
            foreach (var name in listFolder)
            {
                listGridView.Add(new BoxGroupDataGridView() { GroupName = name });
            }
            dataGridGroup.ItemsSource = listGridView;
        }

    }
}