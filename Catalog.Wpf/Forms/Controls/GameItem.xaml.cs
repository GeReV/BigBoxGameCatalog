using System.Windows;
using System.Windows.Controls;
using Catalog.Model;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class GameItem : UserControl
    {
        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register(
            nameof(ItemType), typeof(ItemType), typeof(GameItem), new PropertyMetadata(default(ItemType)));

        public static readonly DependencyProperty MissingProperty = DependencyProperty.Register(
            nameof(Missing), typeof(bool), typeof(GameItem), new PropertyMetadata(false));

        public bool Missing
        {
            get => (bool) GetValue(MissingProperty);
            set => SetValue(MissingProperty, value);
        }

        public ItemType ItemType
        {
            get => (ItemType) GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        public GameItem()
        {
            InitializeComponent();
        }
    }
}