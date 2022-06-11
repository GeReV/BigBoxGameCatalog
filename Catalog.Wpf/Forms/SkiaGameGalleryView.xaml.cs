using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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
using Topten.RichTextKit;
using Style = Topten.RichTextKit.Style;
using TextAlignment = Topten.RichTextKit.TextAlignment;
using TextBlock = Topten.RichTextKit.TextBlock;

namespace Catalog.Wpf.Forms
{
    public sealed partial class SkiaGameGalleryView : UserControl, IScrollInfo
    {
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

            if (e.OldValue is CollectionView oldCollectionView)
            {
                oldCollectionView.CurrentChanged -= view.CurrentItemChanged;
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

            collectionView.CurrentChanged += view.CurrentItemChanged;

            view.InvalidateArrange();
            view.Surface.InvalidateVisual();
        }

        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register(
            nameof(HighlightedText),
            typeof(string),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(string), HighlightedTextPropertyChangedCallback)
        );

        private static void HighlightedTextPropertyChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RedrawCallback(d, e);
        }

        public string HighlightedText
        {
            get => (string)GetValue(HighlightedTextProperty);
            set => SetValue(HighlightedTextProperty, value);
        }

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

        private Rect CurrentItemRect
        {
            get
            {
                var index = Games.CurrentPosition;

                var indexX = index % ItemsPerRow;
                var indexY = index / ItemsPerRow;

                return new Rect(
                    indexX * ContainerWidth,
                    indexY * ContainerHeight,
                    ContainerWidth,
                    ContainerHeight
                );
            }
        }

        private static void RedrawCallback(DependencyObject d, DependencyPropertyChangedEventArgs _)
        {
            var control = (SkiaGameGalleryView)d;

            control.InvalidateArrange();
        }

        private void GamesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs _) => Redraw();

        private void CurrentItemChanged(object? sender, EventArgs e) => Redraw();

        private void Redraw() =>
            Dispatcher.InvokeAsync(
                () =>
                {
                    InvalidateArrange();
                    Surface.InvalidateVisual();
                },
                DispatcherPriority.Render
            );

        private readonly GlContext glContext = new WglContext();
        private readonly GRContext grContext;

        private SKSurface? surface;
        private SKSizeI screenCanvasSize;

        private Size extent;
        private Vector offset;

        private readonly SkiaTextureAtlas atlas = new();

        private static readonly SKColor ItemBorderColor = new(0xff70c0e7);
        private static readonly SKColor ItemMouseOverBackgroundColor = new(0xffe5f3fb);
        private static readonly SKColor ItemSelectedBackgroundColor = new(0xffcbe8f6);
        private static readonly SKColor HighlightTextColor = new(0xffeee8aa);

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

        private int? ItemIndexAtPoint(Point point)
        {
            point.Y += VerticalOffset;

            var indexX = (int)(point.X / ContainerWidth);

            if (indexX >= ItemsPerRow)
            {
                return null;
            }

            var indexY = (int)(point.Y / ContainerHeight);

            var itemIndex = indexY * ItemsPerRow + indexX;

            if (itemIndex >= Games.Count)
            {
                return null;
            }

            return itemIndex;
        }

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

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            FocusManager.SetFocusedElement(this, this);

            var mousePos = Mouse.GetPosition(this);

            var position = ItemIndexAtPoint(mousePos);

            if (!position.HasValue)
            {
                return;
            }

            Games.MoveCurrentToPosition(position.Value);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var handled = true;
            var key = e.Key;

            switch (key)
            {
                case Key.Up:
                case Key.Left:
                case Key.Down:
                case Key.Right:
                {
                    var direction = key switch
                    {
                        Key.Up => FocusNavigationDirection.Up,
                        Key.Down => FocusNavigationDirection.Down,
                        Key.Left => FocusNavigationDirection.Left,
                        Key.Right => FocusNavigationDirection.Right,
                        _ => throw new ArgumentOutOfRangeException(nameof(key))
                    };

                    if (!NavigateByLine(direction))
                    {
                        handled = false;
                    }

                    BringIntoView(CurrentItemRect);

                    break;
                }
                case Key.Home:
                    NavigateToStart();

                    BringIntoView(CurrentItemRect);
                    break;

                case Key.End:
                    NavigateToEnd();

                    BringIntoView(CurrentItemRect);
                    break;
                case Key.Enter:
                {
                    if (e.Key == Key.Enter && (bool)GetValue(KeyboardNavigation.AcceptsReturnProperty) == false)
                    {
                        handled = false;
                        break;
                    }

                    // If ALT is down & Ctrl is up, then we shouldn't handle this. (system menu)
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == ModifierKeys.Alt)
                    {
                        handled = false;
                        break;
                    }

                    // Enter item.
                }
                    break;

                case Key.PageUp:
                    NavigateByPage(FocusNavigationDirection.Up);

                    BringIntoView(CurrentItemRect);
                    break;

                case Key.PageDown:
                    NavigateByPage(FocusNavigationDirection.Down);

                    BringIntoView(CurrentItemRect);
                    break;

                default:
                    handled = false;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        #region Public Events

        public event EventHandler<EventArgs>? GameDoubleClick;

        private void OnGameDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                GameDoubleClick?.Invoke(this, e);
            }
        }

        #endregion

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

        #region Rendering

        private void BuildAtlas(ICollectionView collectionView)
        {
            var games = collectionView.SourceCollection.Cast<GameViewModel>();

            var list = games.Select(g => g.CoverPath)
                .OfType<string>();

            atlas.BuildAtlas(glContext, grContext, list);
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

            var textStyle = new Style
            {
                FontFamily = "system",
                FontSize = FONT_SIZE,
            };
            var highlightTextStyle = textStyle.Modify(backgroundColor: HighlightTextColor);

            var textBlock = new TextBlock
            {
                MaxLines = 1,
                MaxHeight = LINE_HEIGHT,
                MaxWidth = (float)ThumbnailWidth,
                Alignment = TextAlignment.Center,
            };

            var start = Math.Min(Games.Count, (int)(VerticalOffset / ContainerHeight) * ItemsPerRow);
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

                if (Games.CurrentItem == game)
                {
                    DrawRect(surface.Canvas, containerRect, paint, ItemSelectedBackgroundColor, ItemBorderColor);
                }
                else if (containerRect.Contains((float)mouse.X, (float)mouse.Y))
                {
                    DrawRect(surface.Canvas, containerRect, paint, ItemMouseOverBackgroundColor, ItemBorderColor);
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
                    new SKPoint(contentLeft, (float)(contentTop + ThumbnailHeight + LINE_MARGIN)),
                    textBlock,
                    textStyle,
                    highlightTextStyle
                );
            }

            atlas.DrawSprites(surface.Canvas, images.ToArray(), rects.ToArray());

            surface.Canvas.Restore();

            canvas.DrawSurface(surface, SKPoint.Empty);
        }

        private static void DrawRect(
            SKCanvas canvas,
            SKRect rect,
            SKPaint paint,
            SKColor backgroundColor,
            SKColor borderColor
        )
        {
            paint.Color = backgroundColor;
            paint.Style = SKPaintStyle.Fill;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);

            paint.Color = borderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);
        }

        private void DrawText(
            SKCanvas canvas,
            string? text,
            SKPoint point,
            TextBlock textBlock,
            IStyle textStyle,
            IStyle highlightTextStyle
        )
        {
            if (text == null)
            {
                return;
            }

            var term = HighlightedText;

            textBlock.Clear();
            textBlock.AddText(text, textStyle);

            // We add the pre-truncated text ourselves, since trying to style the full text yields unexpected results
            // together with the automated truncation.
            var truncatedText = text[..textBlock.MeasuredLength].Trim();

            textBlock.Clear();
            textBlock.AddText(truncatedText, textStyle);

            if (truncatedText.Length < text.Length)
            {
                textBlock.AddText("…", textStyle);
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                var index = 0;

                while (index < truncatedText.Length)
                {
                    var matchIndex = text.IndexOf(term, index, StringComparison.InvariantCultureIgnoreCase);

                    if (matchIndex < 0)
                    {
                        break;
                    }

                    if (matchIndex > truncatedText.Length)
                    {
                        break;
                    }

                    var len = Math.Min(term.Length, truncatedText.Length - matchIndex);

                    textBlock.ApplyStyle(
                        matchIndex,
                        len,
                        highlightTextStyle
                    );

                    index = matchIndex + len;
                }
            }

            textBlock.Paint(canvas, point);
        }

        #endregion

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

        #region IScrollInfo

        private const double LINE_SIZE = 16;
        private const double WHEEL_SIZE = LINE_SIZE * 3;

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
            if (rectangle.Top < offset.Y)
            {
                SetVerticalOffset(rectangle.Top);
            }

            if (rectangle.Bottom > ViewportHeight + offset.Y)
            {
                SetVerticalOffset(rectangle.Bottom - ViewportHeight);
            }

            Redraw();

            return rectangle;
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

        #region Keyboard Navigation

        private bool NavigateByLine(FocusNavigationDirection direction)
        {
            switch (direction)
            {
                case FocusNavigationDirection.Left:
                    if (Games.CurrentPosition > 0)
                    {
                        Games.MoveCurrentToPrevious();
                    }

                    break;
                case FocusNavigationDirection.Right:
                    if (Games.CurrentPosition < Games.Count - 1)
                    {
                        Games.MoveCurrentToNext();
                    }

                    break;
                case FocusNavigationDirection.Up:
                {
                    var pos = Games.CurrentPosition - ItemsPerRow;
                    if (pos >= 0)
                    {
                        Games.MoveCurrentToPosition(pos);
                    }

                    break;
                }
                case FocusNavigationDirection.Down:
                {
                    var pos = Games.CurrentPosition + ItemsPerRow;
                    if (pos < Games.Count)
                    {
                        Games.MoveCurrentToPosition(pos);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return true;
        }

        private void NavigateByPage(FocusNavigationDirection direction)
        {
            var itemsPerPage = (int)(Math.Ceiling(ViewportHeight / ItemHeight) * ItemsPerRow);

            switch (direction)
            {
                case FocusNavigationDirection.Up:
                {
                    var pos = Games.CurrentPosition - itemsPerPage;
                    if (pos >= 0)
                    {
                        Games.MoveCurrentToPosition(pos);
                    }
                    else
                    {
                        Games.MoveCurrentToFirst();
                    }

                    break;
                }
                case FocusNavigationDirection.Down:
                {
                    var pos = Games.CurrentPosition + itemsPerPage;
                    if (pos < Games.Count)
                    {
                        Games.MoveCurrentToPosition(pos);
                    }
                    else
                    {
                        Games.MoveCurrentToLast();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void NavigateToStart()
        {
            Games.MoveCurrentToFirst();
        }

        private void NavigateToEnd()
        {
            Games.MoveCurrentToLast();
        }

        #endregion
    }
}
