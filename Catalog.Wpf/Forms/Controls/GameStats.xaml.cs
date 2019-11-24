using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class GameStats : UserControl
    {
        public static readonly DependencyProperty GameItemGroupsProperty = DependencyProperty.Register(
            nameof(GameItemGroups), typeof(IEnumerable<GameItemGroupViewModel>), typeof(GameStats),
            new PropertyMetadata(default(ILocalResource)));

        public static readonly DependencyProperty WrapProperty = DependencyProperty.Register(
            nameof(Wrap), typeof(bool), typeof(GameStats), new PropertyMetadata(true));

        public bool Wrap
        {
            get => (bool) GetValue(WrapProperty);
            set => SetValue(WrapProperty, value);
        }

        public IEnumerable<GameItemGroupViewModel> GameItemGroups
        {
            get => (IEnumerable<GameItemGroupViewModel>) GetValue(GameItemGroupsProperty);
            set => SetValue(GameItemGroupsProperty, value);
        }

        public GameStats()
        {
            InitializeComponent();
        }
    }
}