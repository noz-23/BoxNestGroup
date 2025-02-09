using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using System.IO;
using System.Windows;

namespace BoxNestGroup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App():base()
        {
            // ログ作成
            LogFile.Instance.Create();

            SettingManager.Instance.Create();
            // 基本フォルダ
            var currentFolder = Directory.GetCurrentDirectory();
            LogFile.Instance.WriteLine($"{currentFolder.ToString()}");

        }
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // シングルトン化のため変更
            /* App.xaml 
                <Application x:Class="BoxNestGroup.App"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:local="clr-namespace:BoxNestGroup"
                 StartupUri="MainWindow.xaml"
                >
                ↓ に変更してsingleton化
                <Application x:Class="BoxNestGroup.App"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:local="clr-namespace:BoxNestGroup"
                >
            */

            base.OnStartup(e);
            BoxNestGroup.MainWindow.Instance.Show();
        }
    }

}
