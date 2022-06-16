using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Catalog.Wpf.Gallery;
using Catalog.Wpf.GlContexts;
using Catalog.Wpf.GlContexts.Wgl;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Catalog.Wpf.Forms
{
    public sealed partial class SkiaGameGalleryView : UserControl, IScrollInfo
    {
        public delegate void ItemMouseEventHandler(object sender, ItemMouseEventArgs e);

        public class ItemMouseEventArgs : RoutedEventArgs
        {
            public object Item { get; }
            public int ItemIndex { get; }

            public ItemMouseEventArgs(RoutedEvent routedEvent, object item, int itemIndex) : base(routedEvent)
            {
                Item = item;
                ItemIndex = itemIndex;
            }
        }

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

            var games = collectionView.Cast<GameViewModel>().ToList();

            view.BuildAtlas(games);

            view.BuildGalleryItems(games);

            collectionView.CurrentChanged += view.CurrentItemChanged;

            view.InvalidateArrange();
            view.Surface.InvalidateVisual();
        }

        public static readonly DependencyProperty MouseOverItemProperty = DependencyProperty.Register(
            nameof(MouseOverItem),
            typeof(object),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(object?))
        );

        public object? MouseOverItem
        {
            get => GetValue(MouseOverItemProperty);
            set => SetValue(MouseOverItemProperty, value);
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
        
        public static readonly DependencyProperty ItemHorizontalSpacingProperty = DependencyProperty.Register(
            nameof(ItemHorizontalSpacing),
            typeof(double),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(double), RedrawCallback)
        );

        public double ItemHorizontalSpacing
        {
            get => (double)GetValue(ItemHorizontalSpacingProperty);
            set => SetValue(ItemHorizontalSpacingProperty, value);
        }

        public static readonly DependencyProperty ItemVerticalSpacingProperty = DependencyProperty.Register(
            nameof(ItemVerticalSpacing),
            typeof(double),
            typeof(SkiaGameGalleryView),
            new PropertyMetadata(default(double), RedrawCallback)
        );

        public double ItemVerticalSpacing
        {
            get => (double)GetValue(ItemVerticalSpacingProperty);
            set => SetValue(ItemVerticalSpacingProperty, value);
        }

        // private Rect CurrentItemRect
        // {
        //     get
        //     {
        //         var index = Games.CurrentPosition;
        //
        //         var (indexY, indexX) = index.DivRem(ItemsPerRow);
        //
        //         return new Rect(
        //             indexX * ContainerWidth,
        //             indexY * ContainerHeight,
        //             ContainerWidth,
        //             ContainerHeight
        //         );
        //     }
        // }

        private static void RedrawCallback(DependencyObject d, DependencyPropertyChangedEventArgs _)
        {
            var view = (SkiaGameGalleryView)d;

            view.InvalidateArrange();
        }

        private void GamesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems == null)
                    {
                        return;
                    }

                    foreach (var addedGame in e.NewItems.Cast<GameViewModel>())
                    {
                        galleryItems.Add(addedGame.GameCopyId, new GalleryItem(addedGame, atlas)
                        {
                            Padding = ItemPadding
                        });
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems == null)
                    {
                        return;
                    }

                    foreach (var removedGame in e.OldItems.Cast<GameViewModel>())
                    {
                        galleryItems.Remove(removedGame.GameCopyId);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems == null || e.NewItems == null)
                    {
                        return;
                    }

                    foreach (var removedGame in e.OldItems.Cast<GameViewModel>())
                    {
                        galleryItems.Remove(removedGame.GameCopyId);
                    }

                    foreach (var addedGame in e.NewItems.Cast<GameViewModel>())
                    {
                        galleryItems.Add(addedGame.GameCopyId, new GalleryItem(addedGame, atlas)
                        {
                            Padding = ItemPadding
                        });
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    if (e.NewItems == null)
                    {
                        return;
                    }

                    BuildGalleryItems(e.NewItems.Cast<GameViewModel>());

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Redraw();
        }

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

        #region Private Constants

        private static readonly SKColor ItemBorderColor = new(0xff70c0e7);
        private static readonly SKColor ItemMouseOverBackgroundColor = new(0xffe5f3fb);
        private static readonly SKColor ItemSelectedBackgroundColor = new(0xffcbe8f6);
        private static readonly SKColor HighlightTextColor = new(0xffeee8aa);

        #endregion

        #region Private Members

        private readonly GlContext glContext = new WglContext();
        private readonly GRContext grContext;

        private readonly SkiaTextureAtlas atlas = new();

        private Dictionary<int, GalleryItem> galleryItems = new();

        private SKSurface? surface;
        private SKSizeI screenCanvasSize;

        private Size extent;
        private Vector offset;

        private DispatcherTimer? toolTipTimer;

        private int currentMouseOverItemIndex = -1;

        #endregion

        #region Private Properties

        private double ItemWidth => ThumbnailWidth + ItemPadding.Left + ItemPadding.Right;
        private int ItemsPerRow => (int)(Math.Floor(ExtentWidth + ItemHorizontalSpacing) / (ItemWidth + ItemHorizontalSpacing));

        private DispatcherTimer? ToolTipTimer
        {
            get => toolTipTimer;
            set
            {
                ResetToolTipTimer();
                toolTipTimer = value;
            }
        }

        #endregion

        // private int? ItemIndexAtPoint(Point point)
        // {
        //     point.Y += VerticalOffset;
        //
        //     var indexX = (int)(point.X / ContainerWidth);
        //
        //     if (indexX >= ItemsPerRow)
        //     {
        //         return null;
        //     }
        //
        //     var indexY = (int)(point.Y / ContainerHeight);
        //
        //     var itemIndex = indexY * ItemsPerRow + indexX;
        //
        //     if (itemIndex >= Games.Count)
        //     {
        //         return null;
        //     }
        //
        //     return itemIndex;
        // }

        private void ResetToolTipTimer()
        {
            if (toolTipTimer == null)
            {
                return;
            }

            toolTipTimer.Stop();
            toolTipTimer = null;
        }

        public SkiaGameGalleryView()
        {
            glContext.MakeCurrent();
            grContext = GRContext.CreateGl();

            InitializeComponent();

            // ToolTipService.SetIsEnabled(this, false);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.GetPosition(this);

            // var mouseHoverItemIndex = ItemIndexAtPoint(mousePos);
            //
            // if (currentMouseOverItemIndex != mouseHoverItemIndex)
            // {
            //     if (currentMouseOverItemIndex >= 0 && currentMouseOverItemIndex < Games.Count)
            //     {
            //         OnItemMouseLeave();
            //     }
            //
            //     if (mouseHoverItemIndex.HasValue)
            //     {
            //         OnItemMouseEnter(mouseHoverItemIndex.Value);
            //     }
            //
            //     currentMouseOverItemIndex = mouseHoverItemIndex ?? -1;
            // }

            Surface.InvalidateVisual();
        }

        private void OnItemMouseEnter(int itemIndex)
        {
            var item = Games.GetItemAt(itemIndex);

            RaiseEvent(
                new ItemMouseEventArgs(
                    ItemMouseEnterEvent,
                    item,
                    itemIndex
                )
            );

            MouseOverItem = item;

            var (indexY, indexX) = itemIndex.DivRem(ItemsPerRow);

            // var containerRect = new Rect(
            //     (float)(indexX * ContainerWidth),
            //     (float)(indexY * ContainerHeight),
            //     (float)ItemWidth,
            //     (float)ItemHeight
            // );
            //
            // ToolTipService.SetPlacementRectangle(this, containerRect);
            //
            // if (ToolTipService.GetToolTip(this) is ToolTip toolTip)
            // {
            //     ToolTipTimer = new DispatcherTimer(DispatcherPriority.Normal)
            //     {
            //         Interval = TimeSpan.FromMilliseconds(ToolTipService.GetInitialShowDelay(this)),
            //     };
            //
            //     ToolTipTimer.Tick += (_, _) => RaiseToolTipOpeningEvent(toolTip);
            //     ToolTipTimer.Start();
            // }
        }

        private void RaiseToolTipOpeningEvent(ToolTip toolTip)
        {
            toolTip.IsOpen = true;

            ToolTipTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(ToolTipService.GetShowDuration(this))
            };

            ToolTipTimer.Tick += (_, _) => OnRaiseToolTipClosingEvent(toolTip);
            ToolTipTimer.Start();
        }

        private void OnRaiseToolTipClosingEvent(ToolTip toolTip)
        {
            ResetToolTipTimer();

            toolTip.IsOpen = false;
        }

        private void OnItemMouseLeave()
        {
            RaiseEvent(
                new ItemMouseEventArgs(
                    ItemMouseLeaveEvent,
                    Games.GetItemAt(currentMouseOverItemIndex),
                    currentMouseOverItemIndex
                )
            );

            if (ToolTipService.GetToolTip(this) is ToolTip toolTip)
            {
                toolTip.IsOpen = false;
            }

            MouseOverItem = null;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            FocusManager.SetFocusedElement(this, this);

            var mousePos = Mouse.GetPosition(this);

            // var position = ItemIndexAtPoint(mousePos);
            //
            // if (!position.HasValue)
            // {
            //     return;
            // }
            //
            // Games.MoveCurrentToPosition(position.Value);
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

                    // BringIntoView(CurrentItemRect);

                    break;
                }
                case Key.Home:
                    NavigateToStart();

                    // BringIntoView(CurrentItemRect);
                    break;

                case Key.End:
                    NavigateToEnd();

                    // BringIntoView(CurrentItemRect);
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

                    // BringIntoView(CurrentItemRect);
                    break;

                case Key.PageDown:
                    NavigateByPage(FocusNavigationDirection.Down);

                    // BringIntoView(CurrentItemRect);
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

        public static readonly RoutedEvent ItemMouseEnterEvent = EventManager.RegisterRoutedEvent(
            nameof(ItemMouseEnter),
            RoutingStrategy.Bubble,
            typeof(ItemMouseEventHandler),
            typeof(SkiaGameGalleryView)
        );

        public event RoutedEventHandler ItemMouseEnter
        {
            add => AddHandler(ItemMouseEnterEvent, value);
            remove => RemoveHandler(ItemMouseEnterEvent, value);
        }

        public static readonly RoutedEvent ItemMouseLeaveEvent = EventManager.RegisterRoutedEvent(
            nameof(ItemMouseLeave),
            RoutingStrategy.Bubble,
            typeof(ItemMouseEventHandler),
            typeof(SkiaGameGalleryView)
        );

        public event ItemMouseEventHandler ItemMouseLeave
        {
            add => AddHandler(ItemMouseLeaveEvent, value);
            remove => RemoveHandler(ItemMouseLeaveEvent, value);
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

            if (surface == null)
            {
                return;
            }

            Render(surface.Canvas);
            
            e.Surface.Canvas.DrawSurface(surface, SKPoint.Empty);
        }

        #region Rendering

        private void BuildAtlas(IEnumerable<GameViewModel> games)
        {
            var list = games.Select(g => g.CoverPath)
                .OfType<string>();

            atlas.BuildAtlas(glContext, grContext, list);
        }

        private void BuildGalleryItems(IEnumerable<GameViewModel> games)
        {
            galleryItems.Clear();
            
            foreach (var game in games)
            {
                galleryItems.Add(
                    game.GameCopyId,
                    new GalleryItem(game, atlas)
                    {
                        Padding = ItemPadding
                    }
                );
            }
        }

        private void Render(SKCanvas canvas)
        {
            canvas.Clear(SKColors.White);

            canvas.Save();
            canvas.Translate(0, (float)-VerticalOffset);

            var mouse = Mouse.GetPosition(this);

            mouse.Y += VerticalOffset;

            using var paint = new SKPaint
            {
                StrokeWidth = 1.0f,
                IsAntialias = true
            };

            // var start = Math.Min(Games.Count, (int)(VerticalOffset / ContainerHeight) * ItemsPerRow);
            // var end = (int)Math.Min(Games.Count, start + (Math.Ceiling(ViewportHeight / ItemHeight) + 1) * ItemsPerRow);

            var start = 0;
            var end = Games.Count;

            var total = end - start;

            var cursor = SKPoint.Empty;
            var maxRowHeight = 0.0f;

            for (var i = start; i < end; i++)
            {
                var game = (GameViewModel)Games.GetItemAt(i);
                var galleryItem = galleryItems[game.GameCopyId];

                var (indexY, indexX) = i.DivRem(ItemsPerRow);

                // var point = new SKPoint(
                //     (float)(indexX * ItemWidth + ItemHorizontalSpacing),
                //     (float)(indexY * galleryItem.DesiredSize.Height + ItemVerticalSpacing)
                // );

                var itemSize = galleryItem.DesiredSize;

                maxRowHeight = Math.Max(maxRowHeight, itemSize.Height);

                if (cursor.X + itemSize.Width > ActualWidth)
                {
                    cursor.X = 0;
                    cursor.Y += (float)(maxRowHeight + ItemVerticalSpacing);

                    maxRowHeight = itemSize.Height;
                }

                if (cursor.Y >= ActualHeight)
                {
                    // Can't render any more lines.
                    break;
                }

                galleryItem.Paint(canvas, cursor);

                cursor.X += (float)(itemSize.Width + ItemHorizontalSpacing);

                // var containerRect = SKRect.Create(
                //     
                //     (float)ItemWidth,
                //     (float)ItemHeight
                // );
                //
                // if (Games.CurrentItem == game)
                // {
                //     DrawRect(canvas, containerRect, paint, ItemSelectedBackgroundColor, ItemBorderColor);
                // }
                // else if (i == currentMouseOverItemIndex)
                // {
                //     DrawRect(canvas, containerRect, paint, ItemMouseOverBackgroundColor, ItemBorderColor);
                // }
            }

            canvas.Restore();
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
            var itemConstraint = new SKSize((float)ItemWidth, float.PositiveInfinity);
            
            var result = SKSize.Empty;
            var cursor = SKPoint.Empty;
            var maxRowHeight = 0.0f;

            foreach (var item in galleryItems.Values)
            {
                item.Measure(itemConstraint);

                var itemSize = item.DesiredSize;

                maxRowHeight = Math.Max(maxRowHeight, itemSize.Height);

                if (cursor.X + itemSize.Width > arrangeBounds.Width)
                {
                    cursor.X = 0;
                    cursor.Y += (float)(maxRowHeight + ItemVerticalSpacing);
                    
                    result.Height = cursor.Y;
                    
                    maxRowHeight = itemSize.Height;
                }

                cursor.X += (float)(itemSize.Width + ItemHorizontalSpacing);
                
                result.Width = (float)Math.Max(result.Width, cursor.X - ItemHorizontalSpacing);
            }
            
            result.Height += maxRowHeight;

            extent = result.ToSize();

            var size = base.ArrangeOverride(arrangeBounds);

            offset.X = Math.Max(0, Math.Min(offset.X, ExtentWidth - ViewportWidth));
            offset.Y = Math.Max(0, Math.Min(offset.Y, ExtentHeight - ViewportHeight));

            VerifyScrollData();

            return size;
        }

        #region IScrollInfo

        private const double LINE_SIZE = 16;
        private const double WHEEL_SIZE = LINE_SIZE * 3;

        private void VerifyScrollData()
        {
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

                // InvalidateArrange();
                Surface.InvalidateVisual();
            }
        }

        public void SetVerticalOffset(double newOffset)
        {
            newOffset = Math.Max(0, Math.Min(newOffset, ExtentHeight - ViewportHeight));
            if (Math.Abs(newOffset - offset.Y) > double.Epsilon * 10)
            {
                offset.Y = newOffset;

                // InvalidateArrange();
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
            // var itemsPerPage = (int)(Math.Ceiling(ViewportHeight / ItemHeight) * ItemsPerRow);
            //
            // switch (direction)
            // {
            //     case FocusNavigationDirection.Up:
            //     {
            //         var pos = Games.CurrentPosition - itemsPerPage;
            //         if (pos >= 0)
            //         {
            //             Games.MoveCurrentToPosition(pos);
            //         }
            //         else
            //         {
            //             Games.MoveCurrentToFirst();
            //         }
            //
            //         break;
            //     }
            //     case FocusNavigationDirection.Down:
            //     {
            //         var pos = Games.CurrentPosition + itemsPerPage;
            //         if (pos < Games.Count)
            //         {
            //             Games.MoveCurrentToPosition(pos);
            //         }
            //         else
            //         {
            //             Games.MoveCurrentToLast();
            //         }
            //
            //         break;
            //     }
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            // }
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
