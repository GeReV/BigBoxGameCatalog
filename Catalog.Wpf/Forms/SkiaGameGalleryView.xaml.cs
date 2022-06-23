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
using Catalog.Wpf.Gallery;
using Catalog.Wpf.GlContexts;
using Catalog.Wpf.GlContexts.Wgl;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Catalog.Wpf.Forms
{
    public sealed partial class SkiaGameGalleryView : UserControl, IScrollInfo
    {
        #region Delegates

        public delegate void ItemMouseEventHandler(object sender, ItemMouseEventArgs e);

        #endregion

        private readonly SkiaTextureAtlas atlas;

        private readonly Dictionary<int, GalleryItem> galleryItems = new();

        private readonly GlContext glContext = new WglContext();
        private readonly GRContext grContext;

        private readonly List<float> rowVerticalOffsets = new();

        private int currentMouseOverItemIndex = -1;

        private Size extent;
        private Vector offset;
        private SKSizeI screenCanvasSize;

        private bool skipArrangeItems;

        private SKSurface? surface;

        private DispatcherTimer? toolTipTimer;

        public SkiaGameGalleryView()
        {
            glContext.MakeCurrent();
            grContext = GRContext.CreateGl();

            atlas = new SkiaTextureAtlas();

            InitializeComponent();

            // ToolTipService.SetIsEnabled(this, false);
        }

        private void Rearrange(bool shouldSkipArrangeItems = false)
        {
            skipArrangeItems = shouldSkipArrangeItems;

            Dispatcher.InvokeAsync(
                () =>
                {
                    InvalidateArrange();
                    ScheduleRepaint();
                },
                DispatcherPriority.Render
            );
        }

        private void ScheduleRepaint()
        {
            Surface.InvalidateVisual();
        }

        private GalleryItem GetGalleryItem(GameViewModel game)
        {
            return galleryItems[game.GameCopyId];
        }

        private int ItemIndexAtPoint(Point point)
        {
            point.Y += VerticalOffset;

            var indexX = (int)((point.X - (ItemsPerRow - 1) * ItemHorizontalSpacing) / ItemWidth);

            if (indexX >= ItemsPerRow)
            {
                return -1;
            }

            var rowIndex = rowVerticalOffsets.FindIndex(v => point.Y < v);

            var itemIndex = rowIndex * ItemsPerRow + indexX;

            if (itemIndex >= Games.Count)
            {
                return -1;
            }

            return itemIndex;
        }

        private void ResetToolTipTimer()
        {
            if (toolTipTimer == null)
            {
                return;
            }

            toolTipTimer.Stop();
            toolTipTimer = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.GetPosition(this);

            var mouseHoverItemIndex = ItemIndexAtPoint(mousePos);

            if (currentMouseOverItemIndex != mouseHoverItemIndex)
            {
                if (currentMouseOverItemIndex >= 0 && currentMouseOverItemIndex < Games.Count)
                {
                    OnItemMouseLeave();
                }

                if (mouseHoverItemIndex >= 0)
                {
                    OnItemMouseEnter(mouseHoverItemIndex);
                }

                currentMouseOverItemIndex = mouseHoverItemIndex;
            }

            Surface.InvalidateVisual();
        }

        private void OnItemMouseEnter(int itemIndex)
        {
            var item = (GameViewModel)Games.GetItemAt(itemIndex);

            RaiseEvent(
                new ItemMouseEventArgs(
                    ItemMouseEnterEvent,
                    item,
                    itemIndex
                )
            );

            currentMouseOverItemIndex = itemIndex;
            GetGalleryItem(item).IsHighlighted = true;

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

            // if (ToolTipService.GetToolTip(this) is ToolTip toolTip)
            // {
            //     toolTip.IsOpen = false;
            // }

            var item = (GameViewModel)Games.GetItemAt(currentMouseOverItemIndex);

            GetGalleryItem(item).IsHighlighted = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            FocusManager.SetFocusedElement(this, this);

            var mousePos = Mouse.GetPosition(this);

            var position = ItemIndexAtPoint(mousePos);

            if (position < 0)
            {
                return;
            }

            Games.MoveCurrentToPosition(position);
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

            ScheduleRepaint();

            if (handled)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
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

            if (surface == null)
            {
                return;
            }

            Render(surface.Canvas);

            e.Surface.Canvas.DrawSurface(surface, SKPoint.Empty);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);

            VerifyScrollData();

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // Scrolling requires an arrange, but there's no point in rearranging items when that happens, so we use 
            // this marker to skip it once.
            if (!skipArrangeItems)
            {
                var scrollSize = ArrangeItems(arrangeBounds);

                extent = scrollSize;
            }

            skipArrangeItems = false;

            var size = base.ArrangeOverride(arrangeBounds);

            VerifyScrollData();

            // TODO: Since InvalidateVisual on the control doesn't re-render, we use this here for when children request an arrange. But should this be here?
            ScheduleRepaint();

            return size;
        }

        private Size ArrangeItems(Size arrangeBounds)
        {
            var itemConstraint = new SKSize((float)ItemWidth, float.PositiveInfinity);

            var cursor = SKPoint.Empty;

            var totalHeight = 0f;
            var maxRowHeight = 0.0f;

            var rowStartItemIndex = 0;

            rowVerticalOffsets.Clear();

            for (var i = 0; i < Games.Count; i++)
            {
                var game = (GameViewModel)Games.GetItemAt(i);
                var galleryItem = GetGalleryItem(game);

                galleryItem.Measure(itemConstraint);

                var itemSize = galleryItem.DesiredSize;

                if (cursor.X + itemSize.Width > arrangeBounds.Width)
                {
                    cursor.X = 0;
                    cursor.Y += (float)(maxRowHeight + ItemVerticalSpacing);

                    rowVerticalOffsets.Add(rowVerticalOffsets.LastOrDefault() + maxRowHeight);

                    ArrangeLine(rowStartItemIndex, i, maxRowHeight);

                    totalHeight = cursor.Y;

                    rowStartItemIndex = i;

                    maxRowHeight = itemSize.Height;
                }

                maxRowHeight = Math.Max(maxRowHeight, itemSize.Height);

                cursor.X += (float)(itemSize.Width + ItemHorizontalSpacing);
            }

            // Add final row.
            rowVerticalOffsets.Add(rowVerticalOffsets.LastOrDefault() + maxRowHeight);

            // Arrange remaining items.
            ArrangeLine(rowStartItemIndex, Games.Count, maxRowHeight);

            totalHeight += maxRowHeight;

            return new Size(arrangeBounds.Width, totalHeight);
        }

        private void ArrangeLine(int rowStartItemIndex, int rowEndItemIndex, float itemHeight)
        {
            var finalItemSize = new SKSize((float)ItemWidth, itemHeight);

            for (var j = rowStartItemIndex; j < rowEndItemIndex; j++)
            {
                var g = (GameViewModel)Games.GetItemAt(j);

                GetGalleryItem(g).Arrange(finalItemSize);
            }
        }

        #region Nested type: ItemMouseEventArgs

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

        #endregion

        #region Public Properties

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

        public CollectionView Games
        {
            get => (CollectionView)GetValue(GamesProperty);
            set => SetValue(GamesProperty, value);
        }

        private static void GamesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SkiaGameGalleryView)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= view.GamesCollectionChanged;
            }

            if (e.OldValue is CollectionView oldCollectionView)
            {
                oldCollectionView.CurrentChanging -= view.CurrentItemChanging;
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

            collectionView.CurrentChanging += view.CurrentItemChanging;
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

        private Rect CurrentItemRect
        {
            get
            {
                var index = Games.CurrentPosition;

                var game = (GameViewModel)Games.CurrentItem;
                var galleryItem = GetGalleryItem(game);

                var (indexY, indexX) = index.DivRem(ItemsPerRow);

                var offsetY = indexY > 0 ? rowVerticalOffsets[indexY - 1] : 0f;

                return new Rect(
                    indexX * (ItemWidth + ItemHorizontalSpacing),
                    offsetY,
                    galleryItem.DesiredSize.Width,
                    galleryItem.DesiredSize.Height
                );
            }
        }

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
                        galleryItems.Add(
                            addedGame.GameCopyId,
                            new GalleryItem(this, addedGame, atlas)
                            {
                                Padding = ItemPadding
                            }
                        );
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
                        galleryItems.Add(
                            addedGame.GameCopyId,
                            new GalleryItem(this, addedGame, atlas)
                            {
                                Padding = ItemPadding
                            }
                        );
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

            Rearrange();
        }

        private void CurrentItemChanging(object sender, CurrentChangingEventArgs e)
        {
            if (Games.CurrentItem != null)
            {
                var oldSelectedGame = (GameViewModel)Games.CurrentItem;

                GetGalleryItem(oldSelectedGame).IsSelected = false;
            }
        }

        private void CurrentItemChanged(object? sender, EventArgs e)
        {
            if (Games.CurrentItem != null)
            {
                var newSelectedGame = (GameViewModel)Games.CurrentItem;

                GetGalleryItem(newSelectedGame).IsSelected = true;
            }

            ScheduleRepaint();
        }

        #endregion

        #region Private Properties

        private double ItemWidth => ThumbnailWidth + ItemPadding.Left + ItemPadding.Right;

        private int ItemsPerRow =>
            (int)(Math.Floor(ExtentWidth + ItemHorizontalSpacing) / (ItemWidth + ItemHorizontalSpacing));

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

        #region Rendering

        private void BuildAtlas(IEnumerable<GameViewModel> games)
        {
            var list = games.Select(g => g.CoverPath)
                .OfType<string>();

            atlas.BuildAtlas(grContext, list);
        }

        private void BuildGalleryItems(IEnumerable<GameViewModel> games)
        {
            galleryItems.Clear();

            foreach (var game in games)
            {
                galleryItems.Add(
                    game.GameCopyId,
                    new GalleryItem(this, game, atlas)
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

            var itemRange = GetItemIndicesInView();

            for (var i = itemRange.Start.Value; i < itemRange.End.Value; i++)
            {
                var game = (GameViewModel)Games.GetItemAt(i);
                var galleryItem = GetGalleryItem(game);

                var (indexY, indexX) = i.DivRem(ItemsPerRow);

                var offsetY = indexY > 0 ? rowVerticalOffsets[indexY - 1] : 0f;

                var point = new SKPoint(
                    (float)(indexX * (ItemWidth + ItemHorizontalSpacing)),
                    offsetY
                );

                galleryItem.Paint(canvas, point);
            }

            canvas.Restore();
        }

        private Range GetItemIndicesInView()
        {
            var startRowIndex = Math.Max(0, rowVerticalOffsets.FindIndex(v => VerticalOffset < v));

            var bottom = Math.Min(VerticalOffset + ViewportHeight, ExtentHeight);
            var endRowIndex = rowVerticalOffsets.FindIndex(startRowIndex, v => bottom <= v) + 1;

            var start = startRowIndex * ItemsPerRow;
            var end = Math.Min(Games.Count, endRowIndex * ItemsPerRow);

            return start..end;
        }

        #endregion

        #region IScrollInfo

        private const double LINE_SIZE = 16;
        private const double WHEEL_SIZE = LINE_SIZE * 3;

        private void VerifyScrollData()
        {
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

            Rearrange(true);

            return rectangle;
        }

        public void SetHorizontalOffset(double newOffset)
        {
            newOffset = Math.Max(0, Math.Min(newOffset, ExtentWidth - ViewportWidth));
            if (Math.Abs(newOffset - offset.X) > double.Epsilon * 10)
            {
                offset.X = newOffset;

                Rearrange(true);
            }
        }

        public void SetVerticalOffset(double newOffset)
        {
            newOffset = Math.Max(0, Math.Min(newOffset, ExtentHeight - ViewportHeight));
            if (Math.Abs(newOffset - offset.Y) > double.Epsilon * 10)
            {
                offset.Y = newOffset;

                Rearrange(true);
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
            var itemRange = GetItemIndicesInView();

            var itemCount = itemRange.End.Value - itemRange.Start.Value;

            switch (direction)
            {
                case FocusNavigationDirection.Up:
                {
                    var pos = Games.CurrentPosition - itemCount;
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
                    var pos = Games.CurrentPosition + itemCount;
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

            BringIntoView(CurrentItemRect);
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
