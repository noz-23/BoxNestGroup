using BoxNestGroup.GridView;
using BoxNestGroup.Manager;
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
        }

        /// <summary>
        /// 起動後の処理
        /// 　表示関係はここで処理しないと出ない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定(フォルダとBox比較)の読み込みはここ
            //SettingManager.Instance.LoadFile();

            checkFolderToDataGridViewGroupData();
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
            var newValue = e.Row;
            var oldValue = e.Row.Item;

            Console.WriteLine("■dataGridGroupRowEditEnding : {0}", e);
        }

        private void dataGridGroupCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var newGroupName = (e.EditingElement as System.Windows.Controls.TextBox).Text;
            var oldGroup = (e.Row.Item as BoxGroupDataGridView);

            if (oldGroup.GroupId == string.Empty)
            {
                return;
            }

            Console.WriteLine("■dataGridGroupCellEditEnding  Id[{0}] old[{1}] -> new[{2}]", oldGroup.GroupId, oldGroup.GroupName, newGroupName);
            var rtn =BoxManager.Instance.UpdateGroupName(oldGroup.GroupId, newGroupName);
            FolderManager.Instance.UpdateFolder(oldGroup.GroupName, newGroupName);

            renewFolderData();
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
        //    if (e.ChangedButton != MouseButton.Left)
        //    {
        //        return;
        //    }
        //    lastPoint = e.GetPosition(treeViewFolder);
        //    //Console.WriteLine("■treeViewFolderPreviewMouseDown lastPoint :[ {0} - {1} ]", lastPoint.X, lastPoint.Y);
        }

        private void treeViewFolderDrop(object sender, System.Windows.DragEventArgs e)
        {
        //    Console.WriteLine("■treeViewFolderDrop : {0}", e);
        }

        private void treeViewFolderMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        //    //Console.WriteLine("■treeViewFolderMouseMove : {0}", e.LeftButton);
        //    if (e.LeftButton == MouseButtonState.Released)
        //    {
        //        return; 
        //    }

        //    var nowPoint = e.GetPosition(treeViewFolder);
        //    var distance = nowPoint - lastPoint;

        //    if (distance.Length <10.0)
        //    {
        //        return;
        //    }
        //    var selectItem =treeViewFolder.SelectedItem;
        //    DragDrop.DoDragDrop(treeViewFolder, treeViewFolder.SelectedValue, System.Windows.DragDropEffects.Move);
        }


    }
}