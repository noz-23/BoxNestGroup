using BoxNestGroup.Managers;
using BoxNestGroup.Views;
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
    public partial class MainWindow : System.Windows.Window
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