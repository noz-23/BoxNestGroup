using BoxNestGroup.Views;
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
using BoxNestGroup.Managers;
using System.Collections.ObjectModel;

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// MakeGroupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MakeGroupWindow : Window
    {
        public MakeGroupWindow()
        {
            InitializeComponent();

            foreach (var view in SettingManager.Instance.ListGroupDataGridView)
            {
                ListGroup.Add(new MakeGroupView( view));
            }
        }

        public ObservableCollection<MakeGroupView> ListGroup{get;private set;} =new ObservableCollection<MakeGroupView>();

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canselButtonClick(object sender_, RoutedEventArgs e_)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButtonClick(object sender_, RoutedEventArgs e_)
        {
            DialogResult = true;
            Close();
        }
    }
}
