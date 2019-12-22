using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Catalog.Wpf.ViewModel
{
    public class ManageTagsViewModel
    {
        private readonly CatalogContext database;

        public ManageTagsViewModel(CatalogContext database)
        {
            this.database = database;

            Tags = new ObservableCollection<TagViewModel>(
                database
                    .Tags
                    .Select(tag => new TagViewModel(tag))
            );

            DeleteTagCommand  = new DelegateCommand(DeleteTag);
        }

        public ObservableCollection<TagViewModel> Tags { get; }

        public ICommand DeleteTagCommand { get; }

        private void DeleteTag(object parameter)
        {
            if (!(parameter is TagViewModel tag))
            {
                return;
            }

            var messageBoxResult = MessageBox.Show($"Are you sure you want to delete tag \"{tag.Title}\"? (This operation is irreversible)", "Delete Tag", MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            database.Remove(tag.Tag);
            database.SaveChanges();

            Tags.Remove(tag);
        }
    }
}
