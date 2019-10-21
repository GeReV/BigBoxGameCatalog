using System.Windows;
using System.Windows.Controls;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class GameInfoCard : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game),
            typeof(MainWindowViewModel.Game), typeof(GameInfoCard),
            new FrameworkPropertyMetadata(default(MainWindowViewModel.Game)));

        public GameInfoCard()
        {
            InitializeComponent();
        }

        public MainWindowViewModel.Game Game
        {
            get => (MainWindowViewModel.Game) GetValue(GameProperty);
            set
            {
                SetValue(GameProperty, value);

                DataContext = value;
            }
        }
    }
}