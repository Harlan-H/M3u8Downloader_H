using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using System.Windows.Shapes;
using System.Collections;
using M3u8Downloader_H.Extensions;
using System.Linq;
using System;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Behaviors
{
   
    public class ListItemDragAndDropBehavihor : Behavior<FrameworkElement>
    {
        private class DragAdorner : Adorner
        {
            private readonly Point _startPoint;
            private Point _currentPoint;
            private readonly UIElement _element;
            private readonly FrameworkElement _container;
            private readonly UIElement _visual;
            public DragAdorner(FrameworkElement element,FrameworkElement container, Point startPoint) : base(element)
            {
                _element = element;
                _container = container;
                _startPoint = startPoint;
                _visual = CreateVisual(element);


                _container.AllowDrop = true;
                _container.DragOver += HandleDragOver;
                _container.Drop += HandleDrop;
            }


            private static Rectangle CreateVisual(FrameworkElement element) => new()
            {
                Fill = new VisualBrush(element),
                Width = element.RenderSize.Width,
                Height = element.RenderSize.Height,
                IsHitTestVisible = false,
                RenderTransform = new ScaleTransform(1.1, 1.1),
                Effect = new DropShadowEffect { Opacity = 0.5 },
            };

            protected override Size ArrangeOverride(Size finalSize)
            {
                _visual.Arrange(new Rect(finalSize));
                return _visual.DesiredSize;
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _visual.Arrange(new Rect(constraint));
                return _visual.DesiredSize;
            }

            protected override Visual GetVisualChild(int index) => _visual;

            protected override int VisualChildrenCount => 1;

            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                var transformGroup = new GeneralTransformGroup();
                transformGroup.Children.Add(base.GetDesiredTransform(transform));
                transformGroup.Children.Add(new TranslateTransform(_currentPoint.X - _startPoint.X, _currentPoint.Y - _startPoint.Y));
                return transformGroup;
            }

            internal void HandleDragOver(object sender, DragEventArgs e)
            {
                if (Parent == null) return;

                if (Parent is AdornerLayer layer)
                {
                    _currentPoint = e.GetPosition(_container);
                    layer.Update(_element);
                }
            }

            internal void HandleDrop(object sender, DragEventArgs e)
            {
                if (Parent != null)
                {
                    ((AdornerLayer)Parent).Remove(this);
                    _container.AllowDrop = false;
                    _container.DragOver -= HandleDragOver;
                    _container.Drop -= HandleDrop;
                }
            }
        }

        private Point _mouseDownPoint;
        private bool _dragging;
        private DragAdorner _adorner = default!;

        public IList ItemSource
        {
            get { return (IList)GetValue(DataToDragProperty); }
            set { SetValue(DataToDragProperty, value); }
        }

        public static readonly DependencyProperty DataToDragProperty =
            DependencyProperty.Register("ItemSource", typeof(IList), typeof(ListItemDragAndDropBehavihor));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            M3UMediaInfo? droppedData = e.Data.GetData(typeof(M3UMediaInfo)) as M3UMediaInfo;
            M3UMediaInfo? target = ((FrameworkElement)sender).DataContext as M3UMediaInfo;

            int removedIdx = ItemSource!.IndexOf(droppedData);
            int targetIdx = ItemSource.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                ItemSource.Insert(targetIdx + 1, droppedData);
                ItemSource.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (ItemSource.Count + 1 > remIdx)
                {
                    ItemSource.Insert(targetIdx, droppedData);
                    ItemSource.RemoveAt(remIdx);
                }
            }
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging) return;
            if (!e.LeftButton.HasFlag(MouseButtonState.Pressed)) return;

            if (IsUserTryingToDrag(e.GetPosition(AssociatedObject)))
            {
                StartDragOperation(e);
            }
            
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragging = false;
            _mouseDownPoint = e.GetPosition((FrameworkElement)sender);
        }

        private void StartDragOperation(MouseEventArgs e)
        {
            _dragging = true;

            var listBox = AssociatedObject.GetAllAncestors().OfType<ListBox>().FirstOrDefault();

            if (listBox is not null)
            {
                _adorner = new DragAdorner(AssociatedObject, listBox, e.GetPosition(listBox));
                AdornerLayer.GetAdornerLayer(AssociatedObject).Add(_adorner);
            }

            DragDrop.DoDragDrop(AssociatedObject, AssociatedObject.DataContext, DragDropEffects.Move);
        }

        private bool IsUserTryingToDrag(Point currentPoint)
            => Math.Abs(currentPoint.Y - _mouseDownPoint.Y)  >= SystemParameters.MinimumVerticalDragDistance;
        
    }
}
