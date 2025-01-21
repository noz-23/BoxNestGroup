using BoxNestGroup.Views;
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
using BoxNestGroup.Managers;
using System.Windows.Input;
using System.Windows.Forms;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// GroupTreeView.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupTreeViewControl : System.Windows.Controls.UserControl
    {
        public GroupTreeViewControl()
        {
            InitializeComponent();
        }
        //private System.Windows.Input.Cursor noneCursor = new System.Windows.Input.Cursor("none.cur");
        //private System.Windows.Input.Cursor moveCursor = new System.Windows.Input.Cursor("move.cur");
        private XmlGroupTreeView _downItem = null;

        private void _previewMouseDown(object sender_, MouseButtonEventArgs e_)
        {
            //if (!(sender_ is ItemsControl itemsControl))
            //    return;

            //var pos = e_.GetPosition(itemsControl);
            //var pos = e_.GetPosition(_treeView);
            var textbox = e_.OriginalSource as FrameworkElement;
            _downItem = textbox?.DataContext as XmlGroupTreeView;

            //Debug.WriteLine($"[{pos.ToString()}] [{_downItem?.ToString()}]");


        }

        private void _previewMouseMove(object sender_, System.Windows.Input.MouseEventArgs e_)
        {
            //if (!(sender_ is System.Windows.Controls.TreeView) || _treeView.SelectedItem == null)
            //    return;

            //var cursorPoint = _treeView.PointToScreen(e_.GetPosition(_treeView));

                //DragDrop.DoDragDrop(_treeView, _downItem, System.Windows.DragDropEffects.Copy);
                this.Cursor = (_downItem != null) ? System.Windows.Input.Cursors.Hand : System.Windows.Input.Cursors.Arrow;
        }

        private void _previewMouseUp(object sender_, MouseButtonEventArgs e_)
        {
            //if (!(sender_ is ItemsControl itemsControl))
            //    return;

            //var pos = e_.GetPosition(itemsControl);
            //var pos = e_.GetPosition(_treeView);
            if (_downItem != null)
            {
                var textbox = e_.OriginalSource as FrameworkElement;
                var upItem = textbox?.DataContext as XmlGroupTreeView;

                if (upItem == null)
                {
                    if (_downItem.Parent == null)
                    {
                        SettingManager.Instance.ListXmlGroupTreeView.Remove(_downItem);
                    }
                    else
                    {
                        _downItem.Parent.ListChild.Remove(_downItem);
                    }
                    _downItem.Parent = null;
                    SettingManager.Instance.ListXmlGroupTreeView.Add(_downItem);

                }
                else if (upItem != _downItem)
                {
                    if (upItem?.Contains(_downItem) == false)
                    {
                        _downItem.Parent?.ListChild.Remove(_downItem);
                        if (_downItem.Parent == null)
                        {
                            SettingManager.Instance.ListXmlGroupTreeView.Remove(_downItem);
                        }
                        _downItem.Parent = upItem;
                        upItem.ListChild.Add(_downItem);
                    }
                }

            }
            _downItem = null;
            this.Cursor = System.Windows.Input.Cursors.Arrow;

        }

    }
}
