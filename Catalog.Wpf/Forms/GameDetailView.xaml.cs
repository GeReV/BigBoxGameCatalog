using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Catalog.Wpf.Forms
{
    public partial class GameDetailView : UserControl
    {
        public GameDetailView()
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