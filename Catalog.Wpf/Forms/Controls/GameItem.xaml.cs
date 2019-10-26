using System.Windows;
using System.Windows.Controls;
using Catalog.Model;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class GameItem : UserControl
    {
        public static readonly DependencyProperty ItemTProperty = DependencyProperty.Register(
            nameof(ItemType), typeof(ItemType), typeof(GameItem), new PropertyMetadata(default(ItemType)));

        public ItemType ItemType
        {
            get => (ItemType) GetValue(ItemTProperty);
            set => SetValue(ItemTProperty, value);
        }

        public GameItem()
        {
            InitializeComponent();
        }
    }
}