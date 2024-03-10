using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BoxNestGroup
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            textBoxClientID.Text = Settings.Default.ClientID;
            textBoxSecretID.Text = Settings.Default.SecretID;
            textBoxPortNumber.Text = Settings.Default.PortNumber.ToString();
            textBoxToken.Text = Settings.Default.UserToken;
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.ClientID = textBoxClientID.Text;
            Settings.Default.SecretID = textBoxSecretID.Text;
            Settings.Default.PortNumber = Convert.ToUInt16(this.textBoxPortNumber.Text);

            Settings.Default.Save();
            this.Close();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
