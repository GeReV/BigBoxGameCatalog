using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms
{
    public partial class GameGalleryView : UserControl, IGameView
    {
        public static readonly DependencyProperty GameContextMenuProperty = DependencyProperty.Register(
            nameof(GameContextMenu),
            typeof(ContextMenu),
            typeof(GameGalleryView),
            new PropertyMetadata(default(ContextMenu))

        );
        public ContextMenu GameContextMenu
        {
            get => (ContextMenu)GetValue(GameContextMenuProperty);
            set => SetValue(GameContextMenuProperty, value);
        }

        public GameGalleryView()
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
    }
}
