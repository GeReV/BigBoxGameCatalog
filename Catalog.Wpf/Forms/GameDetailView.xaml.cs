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
        private GridViewColumnHeader? listViewSortCol;
        private SortAdorner? listViewSortAdorner;

        public static readonly DependencyProperty GameContextMenuProperty = DependencyProperty.Register(
            nameof(GameContextMenu), typeof(ContextMenu), typeof(GameDetailView), new PropertyMetadata(default(ContextMenu)));

        public ContextMenu GameContextMenu
        {
            get => (ContextMenu) GetValue(GameContextMenuProperty);
            set => SetValue(GameContextMenuProperty, value);
        }

        public GameDetailView()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs>? GameDoubleClick;

        protected virtual void OnGameDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                GameDoubleClick?.Invoke(this, e);
            }
        }

        private void ListViewHeader_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is GridViewColumnHeader column))
            {
                return;
            }

            if (column.Tag == null)
            {
                return;
            }

            var sortBy = column.Tag.ToString();

            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol)?.Remove(listViewSortAdorner);

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
