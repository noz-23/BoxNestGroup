using BoxNestGroup.GridView;
using BoxNestGroup.Manager;
using BoxNestGroup.View;
using BoxNestGroup.Windows;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows;

namespace BoxNestGroup
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// ボタン操作関係
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// 「承認」ボタン操作
		///  BoxへOAuth2承認
		/// </summary>

		private async void webAuthClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■webAuthClick : {0} {1}", sender, e);
			//
			if (BoxManager.Instance.IsHaveAccessToken == false)
			{
				// トークンがない場合は取得
				var win = new BoxOauthWebWindow();
				//win.Show();
				win.Owner = this;
				win.ShowDialog(); // モーダルで表示
			}

			try
			{
				BoxManager.Instance.OAuthToken();
				await BoxManager.Instance.RefreshToken();

				await renewBox();
				renewFolderData();

				await checkFolderToDataGridViewGroupData();

				buttonWebAuth.IsEnabled = false;
				buttonOffLine.IsEnabled = false;

			}
			catch (Exception ex)
			{
				// 承認失敗したらクリア
				Console.WriteLine(ex.Message);
				BoxManager.Instance.SetTokens(string.Empty, string.Empty);

				clearBox("再取得");
			}
		}

		/// <summary>
		/// 「設定」ボタン操作
		///  設定画面の表示
		/// </summary>
		private void settingsClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■settingsClick : {0} {1}", sender, e);
			//
			var win = new SettingsWindow();
			win.Owner = this;
			win.ShowDialog(); // モーダルで表示
		}

		/// <summary>
		/// 「About」ボタン操作
		///  About画面の表示
		/// </summary>

		private void aboutClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■settingsClick : {0} {1}", sender, e);
			//
			var win = new AboutWindow();
			win.Owner = this;
			win.ShowDialog(); // モーダルで表示
		}

		/// <summary>
		/// 「グループ作成」ボタン操作
		///  About画面の表示
		/// </summary>
		private async void makeGroupButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■makeGroupButtonClick : {0} {1}", sender, e);
			//
			foreach (BoxGroupDataGridView group in dataGridGroup.ItemsSource)
			{
				if (group.GroupId != string.Empty)
				{
					// IDが入っている場合は何もしない
					continue;
				}

				var groupCreate = await BoxManager.Instance.CreateGroup(group.GroupName);

				if (groupCreate == null)
				{
					continue;
				}

				if (FolderManager.Instance.ListFolderName.Contains(group.GroupName) == false)
				{
					// フォルダがない場合は作成
					FolderManager.Instance.CreateFolder(groupCreate);
					renewFolderData();
				}
			}
			await renewBox();
		}

		/// <summary>
		/// 「ユーザー作成」ボタン操作
		///  About画面の表示
		/// </summary>
		private void makeUserButtonClick(object sender, RoutedEventArgs e)
		{
		}

		/// <summary>
		/// 「追加」ボタン操作
		///  About画面の表示
		/// </summary>
		private void addUserGroupClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■addUserGroupClick : {0} {1}", sender, e);
			//
			var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
			var selectGroup = treeViewFolder.SelectedItem as FolderGroupTreeView;

			if (listUser == null)
			{
				return;
			}
			if (selectGroup == null)
			{
				return;
			}

			foreach (var user in listUser)
			{
				if (user.Selected == false)
				{
					// チェックがない場合は処理しない
					continue;
				}
				if (selectGroup.GroupName == Settings.Default.ClearGroupName)
				{
					// クリア設定の場合はクリア
					user.ListNestGroup = string.Empty;
					user.ListModGroup = selectGroup.GroupName;
					continue;
				}

				var addList = new List<string>();

				var nowList = user.ListModGroup;
				if (nowList != string.Empty)
				{
					addList.AddRange(nowList.Split("\n"));
				}

				addList.Add(selectGroup.GroupName);
				user.ListModGroup = string.Join("\n", new HashSet<string>(addList));

				// 重複はHashSetで消えるから、そのまま追加
				//addList.AddRange(FolderManager.Instance.ListUniqueGroup(new List<string>([selectGroup.GroupName])));
				addList.AddRange(FolderManager.Instance.ListUniqueGroup(addList));

				user.ListNestGroup = string.Join("\n", new HashSet<string>(addList));
			}

			dataGridUser.ItemsSource = new List<BoxUserDataGridView>(listUser);
			dataGridUser.UpdateLayout();

		}

		/// <summary>
		/// 「更新」ボタン操作
		///  設定されているグループに更新
		/// </summary>
		private async void renewUserButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■renewUserButtonClick : {0} {1}", sender, e);
			var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;

			if (listUser == null)
			{
				return;
			}

			foreach (var user in listUser)
			{
				if (user.ListModGroup == string.Empty)
				{
					continue;
				}
				var listNow = new List<string>(user.ListNowGroup.Split("\n"));
				var listNest = new List<string>(new List<string>(user.ListNestGroup.Split("\n")));


				// 削除するユーザとグループの管理IDリストを取得し削除
				var listDel = new List<string>(listNow);
				foreach (var name in listNest)
				{
					// 更新後にあるフォルダはそのまま
					listDel.Remove(name);
				}
				Console.WriteLine("　renewUserButtonClick listDel: {0}", string.Join(",", listDel));
				var listDelMem = SettingManager.Instance.ListGroupMembershipFromUserId(user.UserId, listDel);
				await BoxManager.Instance.DeleteGroupUser(listDelMem);


				// 更新するリストのグループIDを取得後、ユーザーとグループを紐づける(更新)
				var listAdd = new List<string>(listNest);
				foreach (var name in listNow)
				{
					// 更新前と更新後がそのままのの場合は削除
					listAdd.Remove(name);
				}
				Console.WriteLine("　renewUserButtonClick listAdd: {0}", string.Join(",", listAdd));
				var listAddGroupId = SettingManager.Instance.ListNestGroupId(listAdd);
				await BoxManager.Instance.AddGroupUser(user.UserId, listAddGroupId);
			}

			//SettingManager.Instance.LoadFile();

			await renewBox();
			renewFolderData();
            await checkFolderToDataGridViewGroupData();
		}

		private async void offlineButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("■renewUserButtonClick : {0} {1}", sender, e);

			var dlg = new OpenFileDialog();

			dlg.Filter = "EXCEL ファイル|*.xlsx";
			dlg.FilterIndex = 1;
			if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}


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
				dataGridGroup.ItemsSource = SettingManager.Instance.ListGroupDataGridRow.ToList();
				dataGridUser.ItemsSource = SettingManager.Instance.ListUserDataGridRow.ToList();

                await checkFolderToDataGridViewGroupData();

				buttonMakeGroup.IsEnabled = false;
				buttonMakeUser.IsEnabled = false;
				buttonRenewUser.IsEnabled = false;
				buttonOffLine.IsEnabled = false;
				buttonWebAuth.IsEnabled = false;

			}
		}

		private void saveExcelButtonClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();

			dlg.Filter = "EXCEL ファイル|*.xlsx";
			dlg.FilterIndex = 1;
			if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			var path = dlg.FileName;
			// ファイルを開く処理
			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.AddWorksheet(1);

                worksheet.Cell(1, 1).Value = "名前";
				worksheet.Cell(1, 2).Value = "メール";
				worksheet.Cell(1, 3).Value = "グループ";
				worksheet.Cell(1, 4).Value = "ストレージ";
				worksheet.Cell(1, 5).Value = "外部コラボレーション制限";

				int row = 2;
				foreach (var user in SettingManager.Instance.ListUserDataGridRow)
				{
					if (user.ListNestGroup == string.Empty)
					{
						continue;
					}

                    worksheet.Cell(row, 1).Value = user.UserName;
					worksheet.Cell(row, 2).Value = user.UserMailAddress;
					worksheet.Cell(row, 3).Value = user.ListNestGroup.Replace("\n",";");
                    worksheet.Cell(row, 4).Value = (user.UserSpaceUsed == BoxUserDataGridView.APP_UNLIMITED) ? BoxUserDataGridView.BOX_UNLIMITED : user.UserSpaceUsed;
					worksheet.Cell(row, 5).Value = (user.UserExternalCollaborate == BoxUserDataGridView.APP_ENABLED) ? BoxUserDataGridView.BOX_ENABLED : BoxUserDataGridView.BOX_DISABLED;
					row++;

				}

				workbook.SaveAs(path);

            }
		}
	}
}