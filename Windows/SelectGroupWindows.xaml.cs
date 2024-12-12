using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BoxNestGroup.Windows
{
    /// <summary>
    /// SelectGroupWindows.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectGroupWindows : Window
    {
        //private ObservableCollection<string> _listGroup = null;
        private HashSet<string> _listSelect  = null;

        public string ListSelectGroup
        {
            get
            {
                return string.Join("\n", FolderManager.Instance.ListUniqueGroup(_listSelect));
            }
        }
        public SelectGroupWindows( string groups)
        {
            InitializeComponent();

            _listSelect = new HashSet<string>( groups.Split("\n"));
            _setCheckedItem(_treeBoxGroup.ItemsSource as ObservableCollection<FolderGroupTreeView>);
        }
        private void _setCheckedItem(ObservableCollection<FolderGroupTreeView> list_)
        {
            if (list_ != null)
            {
                foreach (FolderGroupTreeView item in list_)
                {
                    item.Checked = _listSelect?.Contains(item.GroupName) ??false;
                    _setCheckedItem(item.ListChild);
                }
            }
        }

        private void _getCheckItem(ObservableCollection<FolderGroupTreeView> list_)
        {
            if (list_ != null)
            {
                foreach (FolderGroupTreeView item in list_)
                {
                    if (item.Checked == true)
                    {
                        _listSelect.Add(item.GroupName);
                    }
                    _getCheckItem(item.ListChild);
                }
            }
        }


        private void _canselButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void _okButtonClick(object sender, RoutedEventArgs e)
        {
            _listSelect.Clear();
            _getCheckItem(_treeBoxGroup.ItemsSource as ObservableCollection<FolderGroupTreeView>);

            DialogResult = true;
            Close();
        }
    }
}
