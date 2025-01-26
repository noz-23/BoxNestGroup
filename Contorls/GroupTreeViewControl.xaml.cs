using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using System.Windows;
using System.Windows.Input;

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
        private XmlGroupTreeView _moveItem = null;

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

            //moveElemnt?.IsFocused = false;

            //DragDrop.DoDragDrop(_treeView, _downItem, System.Windows.DragDropEffects.Copy);
            var elemnt = e_.OriginalSource as FrameworkElement;
            var _moveItem = elemnt?.DataContext as XmlGroupTreeView;

            this.Cursor = System.Windows.Input.Cursors.Arrow;
            if(_downItem != _moveItem
            && _downItem!=null)
            {
                this.Cursor =  System.Windows.Input.Cursors.ScrollNS;
            }
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
                    if (upItem?.ContainsView(_downItem) == false)
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
