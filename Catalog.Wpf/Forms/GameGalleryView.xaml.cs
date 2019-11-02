using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms
{
    public partial class GameGalleryView : UserControl
    {
        public GameGalleryView()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> GameDoubleClick;

        protected virtual void OnGameDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GameDoubleClick?.Invoke(this, e);
        }
    }
}