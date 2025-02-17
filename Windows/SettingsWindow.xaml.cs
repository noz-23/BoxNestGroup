﻿using System.Windows;
using BoxNestGroup.Managers;
using BoxNestGroup.Properties;

namespace BoxNestGroup
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// 設定表示
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();

            textBoxClientID.Text = Settings.Default.ClientID;
            textBoxSecretID.Text = Settings.Default.SecretID;
            //
            textBoxPortNumber.Text = Settings.Default.PortNumber.ToString();
            textBoxAccess.Text = Settings.Default.AccessToken;
            textBoxRefresh.Text = Settings.Default.RefreshToken;
        }

        /// <summary>
        /// 「保存」ボタン操作
        /// 　各種データを保存して閉じる
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void saveClick(object sender_, RoutedEventArgs e_)
        {
            Settings.Default.ClientID = textBoxClientID.Text;
            Settings.Default.SecretID = textBoxSecretID.Text;
            Settings.Default.PortNumber = Convert.ToUInt16(textBoxPortNumber.Text);

            Settings.Default.AccessToken = textBoxAccess.Text;
            Settings.Default.RefreshToken = textBoxRefresh.Text;

            Settings.Default.Save();
            BoxManager.Instance.RenewConfig();
            this.Close();
        }

        /// <summary>
        /// 「キャンセル」ボタン操
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void cancelClick(object sender_, RoutedEventArgs e_)
        {
            this.Close();
        }
    }
}
