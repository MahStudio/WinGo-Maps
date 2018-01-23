using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace GoogleMapsUnofficial.ViewModel.SearchProviderControls
{
    class NearbySearchProviderVM : INotifyPropertyChanged
    {
        private string _searchquery;
        private ObservableCollection<SearchHelper.Result> _searchres;
        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchQuery
        {
            get { return _searchquery; }
            set
            {
                _searchquery = value;
                if (value.Length >= 3)
                {
                    Search();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchQuery"));
            }
        }
        public ObservableCollection<SearchHelper.Result> SearchResults
        {
            get { return _searchres; }
            set { _searchres = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchResults")); }
        }
        public NearbySearchProviderVM()
        {
            SearchResults = new ObservableCollection<SearchHelper.Result>();
        }
        private async void Search()
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                SearchResults.Clear();
                var s = await SearchHelper.NearbySearch(MapView.MapControl.Center.Position, 5000, Keyword:SearchQuery);
                if (s == null) return;
                foreach (var item in s.results)
                {
                    SearchResults.Add(item);
                }
            });
        }

    }
}
