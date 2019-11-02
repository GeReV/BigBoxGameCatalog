using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.Wpf.Commands;
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

        private void FileItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CommandExecutor.Execute(new OpenFileCommand(), ((FileItem) sender).DataContext);
        }

        private void Screenshot_OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CommandExecutor.Execute(new OpenFileCommand(), ((ListBoxItem) sender).DataContext);
        }

        private void Screenshot_OnItemKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            CommandExecutor.Execute(new OpenFileCommand(), ((ListBoxItem) sender).DataContext);
        }
    }
}