using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Core;

namespace GoogleMapsUnofficial.ViewModel.SearchProviderControls
{
    class TextSearchProviderVM : INotifyPropertyChanged
    {
        private string _searchquery;
        private ObservableCollection<PlaceAutoComplete.Prediction> _searchres; 
        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchQuery
        {
            get { return _searchquery; }
            set
            {
                _searchquery = value;
                if(value.Length >= 3)
                {
                    Search();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchQuery"));
            }
        }
        public ObservableCollection<PlaceAutoComplete.Prediction> SearchResults
        {
            get { return _searchres; }
            set { _searchres = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchResults")); }
        }
        public TextSearchProviderVM()
        {
            SearchResults = new ObservableCollection<PlaceAutoComplete.Prediction>();
        }

        public async void Search()
        {
            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                SearchResults.Clear();
                var s = await PlaceAutoComplete.GetAutoCompleteResults(SearchQuery, location: MapView.MapControl.Center, radius: 50000);
                if (s == null) return;
                foreach (var item in s.predictions)
                {
                    SearchResults.Add(item);
                }
            });
        }
    }
}
