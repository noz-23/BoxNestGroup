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
using System.Collections.Generic;

namespace BoxNestGroup
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private async void webAuthClick(object sender, RoutedEventArgs e)
		{
			if (BoxManager.Instance.IsHaveAccessToken == false)
			{
				var win = new BoxOauthWebWindow();
				//win.Show();
				win.Owner = this;
				win.ShowDialog(); // モーダルで表示

				BoxManager.Instance.OAuthToken();
			}
			else
			{
				await BoxManager.Instance.RefreshToken();
			}

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
		private void aboutClick(object sender, RoutedEventArgs e)
		{
			//await SetLoginUserText();
			var win = new AboutWindow();
			win.Owner = this;
			win.ShowDialog(); // モーダルで表示
		}

		private async void makeGroupButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (BoxGroupDataGridView group in dataGridGroup.ItemsSource)
			{
				if (group.GroupId != string.Empty)
				{
					continue;
				}

				var rtn = await BoxManager.Instance.CreateGroupName(group.GroupName);
				if (FolderManager.Instance.IsHaveGroup(group.GroupName) == false)
				{
					FolderManager.Instance.CreateFolder(rtn);
					renewFolderData();
				}
			}
			var list = await BoxManager.Instance.ListGroupData();
			dataGridGroup.ItemsSource = list.ToList();
		}

		private void makeUserButtonClick(object sender, RoutedEventArgs e)
		{
		}

		private void addClick(object sender, RoutedEventArgs e)
		{
            Console.WriteLine("■addClick : {0} {1}", sender, e);
			//
            var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
			var selectGroup =treeViewFolder.SelectedItem as FolderGroupTreeView;

            if (listUser == null)
            {
                return;
            }
            if (selectGroup ==null)
			{
				return;
			}


			foreach(var user in listUser)
			{
				if (user.Selected == false) 
				{

                    continue;
				}
                if (selectGroup.GroupName == Settings.Default.ClearGroupName)
                {
                    // クリア設定の場合はクリア
                    user.ListNestGroup = string.Empty;
                    user.ListModGroup = selectGroup.GroupName;
                    continue;
                }

                var addList =new List<string>();

                var nowList =user.ListModGroup;
				if (nowList != string.Empty)
				{
					addList.AddRange(nowList.Split("\n"));

                }


				addList.Add(selectGroup.GroupName);
                user.ListModGroup= string.Join("\n", new HashSet<string>(addList));

                // 重複はHashSetで消えるから、そのまま追加
                addList.AddRange(FolderManager.Instance.ListUniqueGroup(new List<string>([selectGroup.GroupName])));

                user.ListNestGroup = string.Join("\n",new HashSet<string>(addList));

                //ListPathFindFolderName
            }

			dataGridUser.ItemsSource = new List<BoxUserDataGridView>( listUser);
            dataGridUser.UpdateLayout();

        }

		private void renewUserButtonClick(object sender, RoutedEventArgs e) { }

    }
}