using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PKHeX.Mobile.Models;
using Xamarin.Forms;

namespace PKHeX.ViewModels
{
    public class LoadableViewModel : BaseViewModel
    {
        public ObservableCollection<LoadableSAV> Saves { get; set; } = new ObservableCollection<LoadableSAV>();
        public ObservableCollection<LoadableSAV> Filtered { get; set; }

        public ICommand SearchCommand => new Command<string>(SearchItems);

        private void SearchItems(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Filtered = null;
            }
            else
            {
                var filteredItems = Saves
                    .Where(bear => bear.File.Metadata.FileName is { } s && s.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                Filtered = new ObservableCollection<LoadableSAV>(filteredItems);
            }
        }
    }
}
