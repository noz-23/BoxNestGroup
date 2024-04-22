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
using BoxNestGroup.Windows;

//using Microsoft.Exchange.WebServices.Data;

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

            // 表示後の処理
            Loaded += windowLoaded;
        }

        private async void windowLoaded(object sender, RoutedEventArgs e)
        {
            // 基本設定(フォルダとBox比較)の読み込みはここ
            SettingManager.Instance.LoadFile();
            // 自動認証の場合
            //await renewBox();
            //renewFolderData();

            checkFolderToDataGridViewGroupData();
        }


        private async System.Threading.Tasks.Task renewBox()
        {
            if (BoxManager.Instance.IsHaveAccessToken == true)
            {
                // トークンありの場合は、ユーザー名を取得して利用できるか確認
                var flg=await setLoginUserText();
                if (flg == true)
                {
                    await renewListGroupData();
                    await renewListUserData();
                    return;
                }
            }
            // リフレッシュトークンとか全部だめな場合は、再取得と表示
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
        private async Task renewListGroupData()
        {
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();

        }

        private async System.Threading.Tasks.Task renewListUserData()
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

        private async void dataGridGroupCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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

        // フォルダのみあったら作成候補に追加
        private void checkFolderToDataGridViewGroupData()
        {
            // Boxにあるけどフォルダにない場合は、BoxのDataGridView更新時に作成している
            var listFolder = new HashSet<string>(FolderManager.Instance.ListFolderName);
            var listBox = BoxManager.Instance.ListGroup;

            foreach (var group in listBox)
            {
                listFolder.Remove(group.Name);
            }

            var listGridView = new List<BoxGroupDataGridView>();
            var list =dataGridGroup.ItemsSource as List<BoxGroupDataGridView>;
            if(list !=null)
            {
                listGridView.AddRange(list);
            }
            foreach (var name in listFolder)
            {
                var add = new BoxGroupDataGridView() { GroupName = name };
                add.Inittal();

                listGridView.Add(add);
            }
            dataGridGroup.ItemsSource = listGridView;
        }

        // ツリービューのドラッグアンドドロップ処理
        private System.Windows.Point lastPoint;
        private void treeViewFolderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            lastPoint = e.GetPosition(treeViewFolder);
            //Console.WriteLine("■treeViewFolderPreviewMouseDown lastPoint :[ {0} - {1} ]", lastPoint.X, lastPoint.Y);
        }

        private void treeViewFolderDrop(object sender, System.Windows.DragEventArgs e)
        {
            Console.WriteLine("■treeViewFolderDrop : {0}", e);
        }

        private void treeViewFolderMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Console.WriteLine("■treeViewFolderMouseMove : {0}", e.LeftButton);
            if (e.LeftButton == MouseButtonState.Released)
            {
                return; 
            }

            var nowPoint = e.GetPosition(treeViewFolder);
            var distance = nowPoint - lastPoint;

            if (distance.Length <10.0)
            {
                return;
            }
            var selectItem =treeViewFolder.SelectedItem;
            DragDrop.DoDragDrop(treeViewFolder, treeViewFolder.SelectedValue, System.Windows.DragDropEffects.Move);
        }


    }
}