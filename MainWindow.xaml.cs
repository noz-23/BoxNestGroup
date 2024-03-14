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
            var listGroupDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            dataGridGroup.ItemsSource = listGroupDataGridRow.ToList();

            var listUserDataGridRow = new ObservableCollection<BoxUserDataGridView>();
            dataGridUser.ItemsSource = listUserDataGridRow.ToList();

            //
            var listTreeViewRow = new ObservableCollection<FolderGroupTreeView>();
            treeViewFolder.ItemsSource = listTreeViewRow.ToList();
        }

        private async void windowLoaded(object sender, RoutedEventArgs e)
        {
            if (BoxManager.Instance.IsHaveAccessToken == true)
            {
                await SetLoginUserText();
            }

            //throw new NotImplementedException();
        }

        private void webAuthClick(object sender, RoutedEventArgs e)
        {
            var win = new BoxOauthWebWindow();
            //win.Show();
            win.Owner = this;
            win.ShowDialog(); // モーダルで表示

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

        public async Task SetLoginUserText() 
        {
            var userName=await BoxManager.Instance.LoginUserName();
            labelLogin.Content = "ログイン：" + userName;
        }

        //private async Task setGroupGridView()
        //{
        //    var userName = await BoxManager.Instance.LoginUserName();
        //}

        private async void renewFoloderButtonClick(object sender, RoutedEventArgs e)
        {
            renewFolder();
            renewGroup();
        }

        private async void makeGroupButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (BoxGroupDataGridView group in dataGridGroup.ItemsSource)
            {
                if (group.GroupId != string.Empty)
                {
                    continue;
                }

                await BoxManager.Instance.CreateGroupName(group.GroupName);
                FolderManager.Instance.CreateFolder(group.GroupName);
            }
            var list = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = list.ToList();
        }

        private async void renewGroupButtonClick(object sender, RoutedEventArgs e)
        {
            renewGroup();
            renewFolder();
        }


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
        private async void renewFolder()
        {
            var list = FolderManager.Instance.ListCommonFolder();
            treeViewFolder.ItemsSource = list.ToList();
        }
        /*
         * 更新グループ
         *  引数　：なし
         *  戻り値：なし
         */
        private async void renewGroup()
        {
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();
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
            Console.WriteLine("dataGridGroupCellEditEnding  Id[{0}] old[{1}] -> new[{2}]", oldGroup.GroupId, oldGroup.GroupName, newGroupName);
            var rtn =BoxManager.Instance.UpdateGroupName(oldGroup.GroupId, newGroupName);
            FolderManager.Instance.UpdateFolder(oldGroup.GroupName, newGroupName);

        }
    }
}