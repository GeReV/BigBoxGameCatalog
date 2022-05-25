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
using Catalog.Wpf.GlContexts;
using Catalog.Wpf.GlContexts.Wgl;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Catalog.Wpf.Forms
{
    public sealed partial class SkiaGameGalleryView : UserControl, IScrollInfo
    {
        private Size extent;
        private Vector offset;

        private readonly SkiaTextureAtlas atlas = new();

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

            if (e.NewValue is not CollectionView collectionView)
            {
                return;
            }

            view.BuildAtlas(collectionView);

            view.InvalidateArrange();
            view.Surface.InvalidateVisual();
        }

        private void BuildAtlas(ICollectionView collectionView)
        {
            var games = collectionView.SourceCollection.Cast<GameViewModel>();

            var list = games.Select(g => g.CoverPath)
                .OfType<string>();

            atlas.BuildAtlas(glContext, grContext, list);
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

        public static readonly DependencyProperty ThumbnailWidthProperty = DependencyProperty.Register(
            nameof(ThumbnailWidth),
            typeof(double),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(120d, RedrawCallback)
        );

        public double ThumbnailWidth
        {
            get => (double)GetValue(ThumbnailWidthProperty);
            set => SetValue(ThumbnailWidthProperty, value);
        }

        public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register(
            nameof(ItemPadding),
            typeof(Thickness),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(new Thickness(8.0f), RedrawCallback)
        );

        public Thickness ItemPadding
        {
            get => (Thickness)GetValue(ItemPaddingProperty);
            set => SetValue(ItemPaddingProperty, value);
        }

        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            nameof(ItemMargin),
            typeof(Thickness),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(Thickness), RedrawCallback)
        );

        public Thickness ItemMargin
        {
            get => (Thickness)GetValue(ItemMarginProperty);
            set => SetValue(ItemMarginProperty, value);
        }

        private static void RedrawCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SkiaGameGalleryView)d;

            control.InvalidateArrange();
        }


        private readonly GlContext glContext = new WglContext();
        private readonly GRContext grContext;

        private SKSurface? surface;
        private SKSizeI screenCanvasSize;

        private static readonly SKColor ItemBorderColor = new(0xff70c0e7);
        private static readonly SKColor ItemMouseOverBackgroundColor = new(0xffe5f3fb);
        private static readonly SKColor ItemSelectedBackgroundColor = new(0xffcbe8f6);

        private const float ASPECT_RATIO = 4f / 3f;

        private const float CORNER_RADIUS = 2.0f;

        private const float FONT_SIZE = 12f;
        private const float LINE_HEIGHT = 16f;
        private const float LINE_MARGIN = 4f;
        private double ThumbnailHeight => Math.Floor(ThumbnailWidth * ASPECT_RATIO);
        private double ItemWidth => ThumbnailWidth + ItemPadding.Left + ItemPadding.Right;
        private double ItemHeight => ThumbnailHeight + ItemPadding.Top + ItemPadding.Bottom + LINE_HEIGHT;

        private double ContainerWidth => ItemWidth + ItemMargin.Left + ItemMargin.Right;
        private double ContainerHeight => ItemHeight + ItemMargin.Top + ItemMargin.Bottom;
        private int ItemsPerRow => (int)((ViewportWidth + ItemMargin.Left + ItemMargin.Right) / ContainerWidth);

        public event EventHandler<EventArgs>? GameDoubleClick;

        public SkiaGameGalleryView()
        {
            glContext.MakeCurrent();
            grContext = GRContext.CreateGl();

            InitializeComponent();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Surface.InvalidateVisual();
        }

        private void Canvas_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvasSize = new SKSizeI(e.Info.Width, e.Info.Height);

            // check if we need to recreate the off-screen surface
            if (screenCanvasSize != canvasSize)
            {
                surface?.Dispose();
                surface = SKSurface.Create(grContext, true, new SKImageInfo(canvasSize.Width, canvasSize.Height));

                screenCanvasSize = canvasSize;
            }

            Render(e.Surface.Canvas);
        }

        private void Render(SKCanvas canvas)
        {
            if (surface == null)
            {
                throw new Exception();
            }

            surface.Canvas.Clear(SKColors.White);

            surface.Canvas.Save();
            surface.Canvas.Translate(0, (float)-VerticalOffset);

            var mouse = Mouse.GetPosition(this);

            mouse.Y += VerticalOffset;

            using var paint = new SKPaint
            {
                StrokeWidth = 1.0f,
                IsAntialias = true
            };

            using var textPaint = new SKPaint
            {
                TextSize = FONT_SIZE,
                TextAlign = SKTextAlign.Center
            };

            var start = (int)(VerticalOffset / ContainerHeight) * ItemsPerRow;
            var end = (int)Math.Min(Games.Count, start + (Math.Ceiling(ViewportHeight / ItemHeight) + 1) * ItemsPerRow);

            var total = end - start;

            var images = new List<string>(total);
            var rects = new List<SKRect>(total);

            for (var i = start; i < end; i++)
            {
                var game = (GameViewModel)Games.GetItemAt(i);

                var indexX = i % ItemsPerRow;
                var indexY = i / ItemsPerRow;

                var containerRect = SKRect.Create(
                    (float)(indexX * ContainerWidth),
                    (float)(indexY * ContainerHeight),
                    (float)ItemWidth,
                    (float)ItemHeight
                );

                if (containerRect.Contains((float)mouse.X, (float)mouse.Y))
                {
                    paint.Color = ItemMouseOverBackgroundColor;
                    paint.Style = SKPaintStyle.Fill;

                    surface.Canvas.DrawRoundRect(containerRect, CORNER_RADIUS, CORNER_RADIUS, paint);

                    paint.Color = ItemBorderColor;
                    paint.Style = SKPaintStyle.Stroke;

                    surface.Canvas.DrawRoundRect(containerRect, CORNER_RADIUS, CORNER_RADIUS, paint);
                }

                var contentLeft = (float)(containerRect.Left + ItemPadding.Left);
                var contentTop = (float)(containerRect.Top + ItemPadding.Top);

                var thumbnailRect = SKRect.Create(
                    contentLeft,
                    contentTop,
                    (float)ThumbnailWidth,
                    (float)ThumbnailHeight
                );

                if (game.CoverPath != null)
                {
                    images.Add(game.CoverPath);
                    rects.Add(thumbnailRect);
                }
                else
                {
                    paint.Color = SKColors.Gray;
                    paint.Style = SKPaintStyle.Fill;

                    surface.Canvas.DrawRect(thumbnailRect, paint);
                }

                DrawText(
                    surface.Canvas,
                    game.Title,
                    new SKPoint(contentLeft, (float)(contentTop + ThumbnailHeight)),
                    textPaint
                );
            }

            atlas.DrawSprites(surface.Canvas, images.ToArray(), rects.ToArray());

            surface.Canvas.Restore();

            canvas.DrawSurface(surface, SKPoint.Empty);
        }

        private void DrawText(SKCanvas canvas, string? text, SKPoint point, SKPaint paint)
        {
            var fThumbnailWidth = (float)ThumbnailWidth;
            var textBounds = SKRect.Empty;

            paint.BreakText(text, fThumbnailWidth, out _, out var measuredText);
            paint.MeasureText(measuredText, ref textBounds);

            var textBottom = point.Y + LINE_HEIGHT + LINE_MARGIN;
            var textCenter = point.X + fThumbnailWidth * 0.5f;

            canvas.DrawText(measuredText, textCenter, textBottom + textBounds.Top, paint);
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
                ContainerWidth * ItemsPerRow,
                Math.Ceiling(Games.Count / (double)ItemsPerRow) * ContainerHeight
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
