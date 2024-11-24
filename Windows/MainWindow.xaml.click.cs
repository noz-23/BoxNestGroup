using BoxNestGroup.Manager;
using BoxNestGroup.View;
using BoxNestGroup.Windows;
using ClosedXML.Excel;
using System.ComponentModel;
using System.Diagnostics;
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
            Process.Start("explorer.exe",FolderManager.Instance.CommonGroupFolderPath);
        }

        /// <summary>
        /// 「承認」ボタン操作
        ///  BoxへOAuth2承認
        /// </summary>

        private async void _buttonWebAuthClick(object sender, RoutedEventArgs e)
		{
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
		/// 「グループ作成」ボタン操作
		/// </summary>
		private async void _buttonMakeGroupButtonClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("■makeGroupButtonClick : {0} {1}", sender, e);

            var list = _dataGridGroup.ItemsSource as BoxGroupDataGridModel;
			if (list == null)
			{
				return;
			}
			//var listGroup = view.SourceCollection;
            //
            foreach (BoxGroupDataGridView group in list)
			{
                if (group== null)
                {
                    continue;
                }

                if (group.GroupId != string.Empty)
				{
					// IDが入っている場合は何もしない
					continue;
				}

				var createName = group.ModGroupName.Trim();

                if (createName == string.Empty)
                {
                    // IDが入っている場合は何もしない
                    continue;
                }

                var groupCreate = await BoxManager.Instance.CreateGroup(createName);

				if (groupCreate == null)
				{
					continue;
				}
                FolderManager.Instance.CreateFolder(createName);

                //if (FolderManager.Instance.Contains(createName) == false)
                //{
                //	// フォルダがない場合は作成
                //	FolderManager.Instance.CreateFolder(groupCreate);
                //}
            }
            //var box = await renewBox();
            await renewBox();
            //Debug.WriteLine("　makeGroupButtonClick box:{0}", box);
            //await setView();
        }

        /// <summary>
        /// 「ユーザー作成」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonMakeUserButtonClick(object sender, RoutedEventArgs e)
		{
            Debug.WriteLine("■makeUserButtonClick : {0} {1}", sender, e);
        }

        /// <summary>
        /// 「追加」ボタン操作
        ///  About画面の表示
        /// </summary>
        private void _buttonAddUserGroupClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("■addUserGroupClick : {0} {1}", sender, e);
			//
			//var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
			//var listUser = SettingManager.Instance.ListUserDataGridRow;
            var listUser = SettingManager.Instance.ListUserDataGridView;
            var selectGroup = _treeViewFolder.SelectedItem as FolderGroupTreeView;

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
				//if (user.Selected == false)
				//{
				//	// チェックがない場合は処理しない
				//	continue;
				//}
				if (selectGroup.GroupName == Settings.Default.ClearGroupName)
				{
					// クリア設定の場合はクリア
					//user.ListNestGroup = string.Empty;
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
				addList.AddRange(FolderManager.Instance.ListUniqueGroup(addList));

				//user.ListNestGroup = string.Join("\n", new HashSet<string>(addList));
			}

			//dataGridUser.ItemsSource = new List<BoxUserDataGridView>(listUser);
			//dataGridUser.UpdateLayout();
			//await setView();
		}

		/// <summary>
		/// 「更新」ボタン操作
		///  設定されているグループに更新
		/// </summary>
		private async void _buttonUserGroupChangeButtonClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("■renewUserButtonClick : {0} {1}", sender, e);
            //var listUser = dataGridUser.ItemsSource as List<BoxUserDataGridView>;
            var listUser = _dataGridUser.ItemsSource as BoxUserDataGridModel;
            if (listUser == null)
            {
                return;
            }
            //var listUser = view.SourceCollection;

            //if (listUser == null)
			//{
			//	return;
			//}

            var wait = new WaitWindow() { Owner = this };
            wait.Show();

			//_buttonRenewUser.IsEnabled = false;

            foreach (BoxUserDataGridView user in listUser)
			{
                if (user == null)
                {
                    continue;
                }
                if (user.ListModGroup == string.Empty)
				{
					continue;
				}
				var listNow = new List<string>(user.ListNowAllGroup.Split("\n"));
				var listMod = new List<string>(user.ListModAllGroup.Split("\n"));

                // 削除するユーザとグループの管理IDリストを取得し削除
                var listDel = new List<string>(listNow);
				foreach (var name in listMod)
				{
					// 更新後にあるフォルダはそのまま
					listDel.Remove(name);
				}
                Debug.WriteLine(string.Format("Name[{0}] /ID[{1}]", user.UserName, user.UserId));

                Debug.WriteLine(string.Format("\trenewUserButtonClick listDel: {0}", string.Join(",", listDel)));
				var listDelMem = SettingManager.Instance.ListBoxGroupMembership.ListGroupMembershipFromUserId(user.UserId, listDel);
				await BoxManager.Instance.DeleteGroupUser(listDelMem);

				// 更新するリストのグループIDを取得後、ユーザーとグループを紐づける(更新)
				var listAdd = new List<string>(listMod);
				foreach (var name in listNow)
				{
					// 更新前と更新後がそのままのの場合は削除
					listAdd.Remove(name);
				}
				Debug.WriteLine(string.Format("\trenewUserButtonClick listAdd: {0}", string.Join(",", listAdd)));
				var listAddGroupId = SettingManager.Instance.ListGroupDataGridView.ListNestGroupId(listAdd);
				await BoxManager.Instance.AddGroupUser(user.UserId, listAddGroupId);
			}

            //SettingManager.Instance.LoadFile();

            //var box = await renewBox();
            await renewBox();
            //Debug.WriteLine("　renewUserButtonClick box:{0}", box);
            //await setView();
            //await checkFolderToDataGridViewGroupData();

            wait.Close();
            //_buttonRenewUser.IsEnabled = true;
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

				//_buttonMakeGroup.IsEnabled = false;
				_buttonMakeUser.IsEnabled = false;
				//_buttonRenewUser.IsEnabled = false;
				_buttonOffLine.IsEnabled = false;
				_buttonWebAuth.IsEnabled = false;

			}
            //await setView();

            wait.Close();
		}

		/// <summary>
		/// ｢保存｣ボタン操作
		/// 　Excelに出力する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			// ファイルを保存処理
			SettingManager.Instance.SaveExcelFile(path);
        }
    }
}