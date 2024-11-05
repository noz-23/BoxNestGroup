using System.IO;
using System.Windows;
using System.Diagnostics;

namespace BoxNestGroup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App():base()
        {
            // ログ作成
            LogFile.Instance.Create();

            // 基本フォルダ
            var currentFolder = Directory.GetCurrentDirectory();
            Debug.WriteLine("■currentFolder:" + currentFolder.ToString());

            var commonGroupFolder = currentFolder + @"\" + Settings.Default.CommonGroupFolder;

            // 基本フォルダがない場合は作成
            if (Directory.Exists(commonGroupFolder) == false)
            {
                Directory.CreateDirectory(commonGroupFolder);
                Debug.WriteLine("■commonGroupFolder" + commonGroupFolder.ToString());
            }

            var commonGroupSetting = currentFolder + @"\" + Settings.Default.CommonGroupSetting;

            // 基本フォルダがない場合は作成
            if (Directory.Exists(commonGroupSetting) == false)
            {
                Directory.CreateDirectory(commonGroupSetting);
                Debug.WriteLine("■commonGroupFolder" + commonGroupSetting.ToString());
            }

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
