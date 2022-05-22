using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Catalog.Wpf.Forms
{
    public sealed partial class SkiaGameGalleryView : UserControl, IScrollInfo
    {
        private Size extent;
        private Vector offset;

        private SkiaTextureAtlas atlas = new();

        public static readonly DependencyProperty GameContextMenuProperty = DependencyProperty.Register(
            nameof(GameContextMenu),
            typeof(ContextMenu),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(ContextMenu))
        );

        public ContextMenu GameContextMenu
        {
            get => (ContextMenu)GetValue(GameContextMenuProperty);
            set => SetValue(GameContextMenuProperty, value);
        }

        public static readonly DependencyProperty GamesProperty = DependencyProperty.Register(
            nameof(Games),
            typeof(CollectionView),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(null, GamesPropertyChangedCallback)
        );

        private static void GamesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SkiaGameGalleryView)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= view.GamesCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += view.GamesCollectionChanged;
            }

            var collectionView = (CollectionView)e.NewValue;

            view.BuildAtlas(collectionView);

            view.InvalidateArrange();
            view.Surface.InvalidateVisual();
        }

        private void BuildAtlas(ICollectionView collectionView)
        {
            var games = collectionView.SourceCollection.Cast<GameViewModel>();

            var list = games.Select(g => g.CoverPath)
                .OfType<string>();

            atlas.BuildAtlas(list);
        }

        private void GamesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs _) =>
            Dispatcher.InvokeAsync(
                () =>
                {
                    InvalidateArrange();
                    Surface.InvalidateVisual();
                },
                DispatcherPriority.Render
            );

        public CollectionView Games
        {
            get => (CollectionView)GetValue(GamesProperty);
            set => SetValue(GamesProperty, value);
        }

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            nameof(ItemWidth),
            typeof(double),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(120d, ItemWidthPropertyChangedCallback)
        );

        private static void ItemWidthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SkiaGameGalleryView)d;

            control.InvalidateArrange();
        }

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        private double ItemHeight => Math.Floor(ItemWidth * 4 / 3);

        private int ItemsPerRow => (int)(ViewportWidth / ItemWidth);

        public event EventHandler<EventArgs>? GameDoubleClick;

        public SkiaGameGalleryView()
        {
            InitializeComponent();
        }

        private void Canvas_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.White);

            Render(canvas);
        }

        private void Render(SKCanvas canvas)
        {
            canvas.Save();
            canvas.Translate(0, (float)-VerticalOffset);

            using var paint = new SKPaint
            {
                Color = SKColors.Gray,
                Style = SKPaintStyle.Fill,
            };

            var start = (int)(VerticalOffset / ItemHeight) * ItemsPerRow;
            var end = (int)Math.Min(Games.Count, start + (Math.Ceiling(ViewportHeight / ItemHeight) + 1) * ItemsPerRow);

            var total = end - start;

            var images = new List<string>(total);
            var rects = new List<SKRect>(total);

            for (var i = start; i < end; i++)
            {
                var game = (GameViewModel)Games.GetItemAt(i);

                var rect = SKRect.Create(
                    (float)((i % ItemsPerRow) * ItemWidth),
                    (float)(Math.Floor(i / (float)ItemsPerRow) * ItemHeight),
                    (float)ItemWidth,
                    (float)ItemHeight
                );

                if (game.CoverPath != null)
                {
                    images.Add(game.CoverPath);
                    rects.Add(rect);
                }
                else
                {
                    canvas.DrawRect(rect, paint);
                }
            }

            atlas.DrawSprites(canvas, images.ToArray(), rects.ToArray());

            canvas.Restore();
        }

        private void OnGameDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                GameDoubleClick?.Invoke(this, e);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);

            VerifyScrollData();

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            VerifyScrollData();

            return size;
        }

        private void VerifyScrollData()
        {
            extent = new Size(
                ItemWidth * ItemsPerRow,
                Math.Ceiling(Games.Count / (double)ItemsPerRow) * ItemHeight
            );

            offset.X = Math.Max(0, Math.Min(offset.X, ExtentWidth - ViewportWidth));
            offset.Y = Math.Max(0, Math.Min(offset.Y, ExtentHeight - ViewportHeight));

            ScrollOwner?.InvalidateScrollInfo();
        }

        #region IScrollInfo

        private const double LINE_SIZE = 16;
        private const double WHEEL_SIZE = LINE_SIZE * 3;

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LINE_SIZE);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LINE_SIZE);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LINE_SIZE);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LINE_SIZE);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - WHEEL_SIZE);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + WHEEL_SIZE);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - WHEEL_SIZE);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + WHEEL_SIZE);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            throw new NotImplementedException();
        }

        public void SetHorizontalOffset(double newOffset)
        {
            newOffset = Math.Max(0, Math.Min(newOffset, ExtentWidth - ViewportWidth));
            if (Math.Abs(newOffset - offset.X) > double.Epsilon * 10)
            {
                offset.X = newOffset;

                InvalidateArrange();
                Surface.InvalidateVisual();
            }
        }

        public void SetVerticalOffset(double newOffset)
        {
            newOffset = Math.Max(0, Math.Min(newOffset, ExtentHeight - ViewportHeight));
            if (Math.Abs(newOffset - offset.Y) > double.Epsilon * 10)
            {
                offset.Y = newOffset;

                InvalidateArrange();
                Surface.InvalidateVisual();
            }
        }

        public ScrollViewer? ScrollOwner { get; set; }


        public bool CanHorizontallyScroll { get; set; }

        public bool CanVerticallyScroll { get; set; }

        public double ExtentWidth => extent.Width;
        public double ExtentHeight => extent.Height;

        public double HorizontalOffset => offset.X;
        public double VerticalOffset => offset.Y;

        public double ViewportWidth => Surface.ActualWidth;
        public double ViewportHeight => Surface.ActualHeight;

        #endregion
    }
}
