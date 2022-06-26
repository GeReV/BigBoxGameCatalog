using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Catalog.Wpf.Forms
{
    public partial class GameDetailView : UserControl, IGameView
    {
        public static readonly DependencyProperty GameContextMenuProperty = DependencyProperty.Register(
            nameof(GameContextMenu),
            typeof(ContextMenu),
            typeof(GameDetailView),
            new PropertyMetadata(default(ContextMenu))
        );

        private SortAdorner? listViewSortAdorner;
        private GridViewColumnHeader? listViewSortCol;

        public GameDetailView()
        {
            InitializeComponent();
        }

        #region IGameView Members

        public ContextMenu GameContextMenu
        {
            get => (ContextMenu)GetValue(GameContextMenuProperty);
            set => SetValue(GameContextMenuProperty, value);
        }

        #endregion

        public event EventHandler<EventArgs>? GameExpanded;

        private void OnGameExpanded(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                GameExpanded?.Invoke(this, e);
            }
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);

            if (e.OriginalSource is not UIElement element)
            {
                return;
            }

            if (element.FindAncestorOrSelf<ListViewItem>() is not { } listViewItem)
            {
                return;
            }

            if (listViewItem.ContextMenu is null)
            {
                return;
            }

            listViewItem.ContextMenu.DataContext = DataContext;
        }

        private void ListViewHeader_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not GridViewColumnHeader column)
            {
                return;
            }

            if (column.Tag == null)
            {
                return;
            }

            var sortBy = column.Tag.ToString() ?? string.Empty;

            if (listViewSortCol != null)
            {
                if (listViewSortAdorner != null)
                {
                    AdornerLayer.GetAdornerLayer(listViewSortCol)?.Remove(listViewSortAdorner);
                }

                GamesList.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;

            if (listViewSortCol == column && listViewSortAdorner?.Direction == newDir)
            {
                newDir = ListSortDirection.Descending;
            }

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);

            AdornerLayer.GetAdornerLayer(listViewSortCol)?.Add(listViewSortAdorner);

            GamesList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
