using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Extensions;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class GameInfoCard : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(
            nameof(Game),
            typeof(GameViewModel),
            typeof(GameInfoCard),
            new FrameworkPropertyMetadata(default(GameViewModel), null, CoerceValueCallback)
        );

        private static object CoerceValueCallback(DependencyObject d, object basevalue)
        {
            if (basevalue is GameViewModel gameViewModel)
            {
                EnrichGame(gameViewModel.GameCopy);
            }

            return basevalue;
        }

        public GameInfoCard()
        {
            InitializeComponent();
        }

        public GameViewModel Game
        {
            get => (GameViewModel)GetValue(GameProperty);
            set => SetValue(GameProperty, value);
        }

        private static void EnrichGame(GameCopy game)
        {
            using var database = Application.Current.Database();

            var entity = database.Attach(game);

            entity
                .Reference(g => g.Publisher)
                .Load();

            entity
                .Collection(g => g.GameCopyDevelopers)
                .Query()
                .Include(gcd => gcd.Developer)
                .Load();

            entity
                .Collection(g => g.Items)
                .Query()
                .Include(item => item.Files)
                .Include(item => item.Scans)
                .Load();
        }

        private void FileItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CommandExecutor.Execute(new OpenFileCommand(), ((FileItem)sender).DataContext);
        }

        private void Screenshot_OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CommandExecutor.Execute(new OpenFileCommand(), ((ListBoxItem)sender).DataContext);
        }

        private void Screenshot_OnItemKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            CommandExecutor.Execute(new OpenFileCommand(), ((ListBoxItem)sender).DataContext);
        }

        private void Hyperlink_OnRequestNavigate(object _, RequestNavigateEventArgs e)
        {
            Process.Start(
                new ProcessStartInfo(e.Uri.AbsoluteUri)
                {
                    UseShellExecute = true
                }
            );

            e.Handled = true;
        }
    }
}
