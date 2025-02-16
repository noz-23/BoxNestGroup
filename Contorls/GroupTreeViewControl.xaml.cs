using BoxNestGroup.Extensions;
using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BoxNestGroup.Contorls
{
    /// <summary>
    /// GroupTreeView.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupTreeViewControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// ネストグループ ツリービュー
        /// </summary>
        public GroupTreeViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ドラッグアンドドロップ開始位置
        /// </summary>
        private System.Windows.Point _startPoint =new System.Windows.Point();
        /// <summary>
        /// マウスドラッグ
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _mouseDown(object sender_, MouseButtonEventArgs e_)
        {
            _startPoint = e_.GetPosition(null);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
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
            if(Math.Abs(nowPoint.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (_treeView.SelectedItem is XmlGroupTreeView selectView)
            {
                LogFile.Instance.WriteLine($"selectView {selectView.GroupName} -> {selectView.GroupId}");

                // 選択されたViewをセット
                DragDrop.DoDragDrop(_treeView, selectView, System.Windows.DragDropEffects.Move);   
            }
        }

        /// <summary>
        /// ドロップ
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _drop(object sender_, System.Windows.DragEventArgs e_)
        {
            if (e_.Data.GetData(typeof(XmlGroupTreeView)) is XmlGroupTreeView dragView)
            {
                // 選択したViewの取得
                var dropPositon = e_.GetPosition(_treeView);
                var hit =VisualTreeHelper.HitTest(_treeView, dropPositon);
                if (hit.VisualHit.GetParentOfType<ItemsControl>() is ItemsControl dropItem)
                {
                    // ドロップ先のViewを取得
                    var dropView =dropItem.DataContext as XmlGroupTreeView;

                    if (dragView?.ContainsView(dropView) == false)
                    {
                        LogFile.Instance.WriteLine($"selectView {dropView?.GroupName} -> {dropView?.GroupId}");
                        //
                        var listRemove = dragView.Parent?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listRemove.Remove(dragView);
                        dragView.Parent = dropView;

                        var listAdd = dropView?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listAdd.Add(dragView);
                    }
                }

            }
        }
        
        /// <summary>
        /// コンテキストメニュー 追加クリック
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _addClick(object sender_, RoutedEventArgs e_)
        {
            var win = new MakeGroupWindow();
            if (win.ShowDialog() == true)
            {
                var item = _treeView.SelectedItem as XmlGroupTreeView;

                LogFile.Instance.WriteLine($"[{item?.GroupName}]");

                var listMake = win.ListGroup.ToList().FindAll(s_ => s_.IsChecked == true);

                listMake?.ToList().ForEach(view_ => 
                {
                    if (item?.ListChild.ToList().Find(x_ => x_.GroupName == view_.GroupName) != null)
                    {
                        return;
                    }
                    item?.ListChild.Add(new XmlGroupTreeView(view_.GroupName, view_.GroupId, item));

                });
            }
        }

        /// <summary>
        /// コンテキストメニュー 削除クリック
        /// </summary>
        /// <param name="sender_"></param>
        /// <param name="e_"></param>
        private void _deleteClick(object sender_, RoutedEventArgs e_)
        {
            if( _treeView.SelectedItem is XmlGroupTreeView item)
            {
                LogFile.Instance.WriteLine($"[{item?.GroupName}]");
                if (item == null)
                {
                    return;
                }

                var parent = item.Parent?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                parent?.Remove(item);
            }
        }
    }
}
