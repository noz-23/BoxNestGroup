﻿using BoxNestGroup.Managers;
using BoxNestGroup.Views;
using BoxNestGroup.Windows;
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
            if (Math.Abs(nowPoint.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (_treeView.SelectedItem is XmlGroupTreeView selectView)
            {
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
                        var listRemove = dragView.Parent?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listRemove.Remove(dragView);
                        dragView.Parent = dropView;

                        var listAdd = dropView?.ListChild ?? SettingManager.Instance.ListXmlGroupTreeView;
                        listAdd.Add(dragView);
                    }
                }

            }
        }

        private void _mouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var win = new MakeGroupWindow();
            win.ShowDialog();
        }
    }
}
