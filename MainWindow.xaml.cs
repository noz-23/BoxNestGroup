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

            //dataGridGroup.ItemsSource = new ObservableCollection<BoxGroupDataGridView>();
            var listDataGridRow = new ObservableCollection<BoxGroupDataGridView>();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();

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
            await SetLoginUserText();
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

        private async void renewGroupButtonClick(object sender, RoutedEventArgs e)
        {
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();            
        }

        private async void makeGroupButtonClick(object sender, RoutedEventArgs e) 
        {
            foreach(BoxGroupDataGridView group in dataGridGroup.ItemsSource)
            {
                if(group.GroupId != string.Empty) 
                {
                    continue;
                }

                await BoxManager.Instance.CreateGroup(group.GroupName);
                FolderManager.Instance.CreateGroup(group.GroupName);
            }
            var listDataGridRow = await BoxManager.Instance.ListGroupData();
            dataGridGroup.ItemsSource = listDataGridRow.ToList();
        }

        private async void renewFolder()
        {
            await FolderManager.Instance.ListFolder();
        }
    }
}