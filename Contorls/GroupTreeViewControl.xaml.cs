using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Extensions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

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
        //private XmlGroupTreeView _downItem = null;
        //private XmlGroupTreeView _moveItem = null;

        private System.Windows.Point _startPoint =new System.Windows.Point();
        private void _mouseDown(object sender_, MouseButtonEventArgs e_)
        {
            _startPoint = e_.GetPosition(null);
        }

        private void _mouseMove(object sender_, System.Windows.Input.MouseEventArgs e_)
        {
            var nowPoint = e_.GetPosition(null);
            if (e_.LeftButton == MouseButtonState.Released == true)
            {
                return;
            }
            if(Math.Abs(nowPoint.X -_startPoint.X) < SystemParameters.MinimumHorizontalDragDistance)
            {
                return;
            }
            if (Math.Abs(nowPoint.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (_treeView.SelectedItem is XmlGroupTreeView selectView)
            {
                DragDrop.DoDragDrop(_treeView, selectView, System.Windows.DragDropEffects.Move);   
            }
        }

        private void _drop(object sender_, System.Windows.DragEventArgs e_)
        {
            if (e_.Data.GetData(typeof(XmlGroupTreeView)) is XmlGroupTreeView dragView)
            {
                var dropPositon = e_.GetPosition(_treeView);
                var hit =VisualTreeHelper.HitTest(_treeView, dropPositon);
                if (hit.VisualHit.GetParentOfType<ItemsControl>() is ItemsControl dropItem)
                {
                    var dropView =dropItem.DataContext as XmlGroupTreeView;

                    if (dragView?.ContainsView(dropView) == false)
                    {
                        var listRemove = dragView.Parent?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listRemove.Remove(dragView);
                        dragView.Parent = dropView;

                        var listAdd = dropView?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listAdd.Add(dragView);
                    }
                }

            }
        }




        /*
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
                            if (_downItem?.ContainsView(upItem) == false)
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
                    PreviewMouseDown="_previewMouseDown"
                    PreviewMouseMove="_previewMouseMove"
                    PreviewMouseUp="_previewMouseUp"

         */
    }
}
