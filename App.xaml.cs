using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace BoxNestGroup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App():base()
        {
            LogFile.Instance.Create();

            var currentFolder = Directory.GetCurrentDirectory();
            Console.WriteLine("currentFolder:" + currentFolder.ToString());

            var commonGroupFolder = currentFolder + @"\" + Settings.Default.CommonGroupFolder;

            // 基本フォルダがない場合は作成
            if (Directory.Exists(commonGroupFolder) == false)
            {
                Directory.CreateDirectory(commonGroupFolder);
                Console.WriteLine("commonGroupFolder" + commonGroupFolder.ToString());
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
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
