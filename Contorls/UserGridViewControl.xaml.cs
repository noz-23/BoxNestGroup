using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// UserGridViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class UserGridViewControl : System.Windows.Controls.UserControl
    {
        public UserGridViewControl()
        {
            InitializeComponent();
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
        /// 「追加/更新」ボタン操作
        ///  About画面の表示
        /// </summary>
        private async void _buttonMakeAndRenewUserGroupClick(object sender, RoutedEventArgs e)
        {
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

        private void _dataGridUserMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("■_dataGridUserMouseDoubleClick : {0} {1}", sender, e);
            // https://qiita.com/kabosu/items/2e905a532632c1512e65
            var elem = e.MouseDevice.DirectlyOver as FrameworkElement;
            if (elem == null)
            {
                return;
            }
            // ParentでDataGridCellが拾えなかった時はTemplatedParentを参照
            // （Borderをダブルクリックした時）
            var cell = (elem.Parent as DataGridCell)?? elem.TemplatedParent as DataGridCell;
            if (cell == null)
            {
                return;
            }
            // ここでcellの内容を処理
            // （cell.DataContextにバインドされたものが入っているかと思います）
            if (cell.Column.Header.ToString() != MainWindow.MENU_USER_NOW)
            {
                return;
            }
            var data = cell.DataContext as UserDataGridView;
            if (data == null)
            {
                return;
            }
            Debug.WriteLine("■_dataGridUserMouseDoubleClick Cell : {0} {1}", data.ListNowGroup, data.ListNowAllGroup);

            var selectGroupWin = new SelectGroupWindows(data.ListNowGroup);

            if (selectGroupWin.ShowDialog() == true)
            {
                data.ListNowAllGroup = selectGroupWin.ListSelectGroup;
            }
        }
    }
}
