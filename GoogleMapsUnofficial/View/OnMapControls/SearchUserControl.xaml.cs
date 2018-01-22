using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial.View.OnMapControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchUserControl : Page
    {
        private class ViewModel : INotifyPropertyChanged
        {
            public ViewModel()
            {
                SearchResults = new ObservableCollection<PlaceAutoComplete.Prediction>();
                SearchResults.CollectionChanged += SuggestedApps_CollectionChanged;
            }

            private void SuggestedApps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChanged("SearchResults");
            }

            private ObservableCollection<PlaceAutoComplete.Prediction> _searchres;

            public ObservableCollection<PlaceAutoComplete.Prediction> SearchResults
            {
                get
                {
                    return _searchres;
                }
                set
                {
                    _searchres = value;
                    OnPropertyChanged("SearchResults");
                }
            }

            public async void SuggestForSearch(string searchExpression)
            {
                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
                {
                    SearchResults.Clear();
                    var s = await PlaceAutoComplete.GetAutoCompleteResults(searchExpression, location: MapView.MapControl.Center, radius: 50000);
                    if (s == null) return;
                    foreach (var item in s.predictions)
                    {
                        SearchResults.Add(item);
                    }
                });

            }


            public void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public SearchUserControl()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel();
            SearchBox.LostFocus += SearchBox_LostFocus;
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Visibility = Visibility.Collapsed;
            BTNExpand.Visibility = Visibility.Visible;
        }
        private void Control2_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //SearchReq.Invoke(args.QueryText, null);
            SearchBox.Visibility = Visibility.Collapsed;
            BTNExpand.Visibility = Visibility.Visible;
        }

        private async void Control2_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var select = (args.SelectedItem as PlaceAutoComplete.Prediction);
            if (select == null) return;
            var res = await GeocodeHelper.GetInfo(select.place_id);
            if (res == null) return;
            SearchBox.Text = "";
            var ploc = res.results.FirstOrDefault().geometry.location;
            MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.lat,
                    Longitude = ploc.lng
                });
            //SearchReq.Invoke(args.SelectedItem as ClassProduct.Product, null);
            SearchBox.Visibility = Visibility.Collapsed;
            BTNExpand.Visibility = Visibility.Visible;
        }

        private void Control2_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text.Length >= 3)
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    if (SearchBox.Text != string.Empty)
                        (this.DataContext as ViewModel).SuggestForSearch((sender as AutoSuggestBox).Text);
                }
            }
        }

        private async void BTNExpand_Click(object sender, RoutedEventArgs e)
        {
            BTNExpand.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Visible;
            await Task.Delay(10);
            try
            {
                SearchBox.Focus(FocusState.Programmatic);
                InputPane.GetForCurrentView().TryShow();
            }
            catch
            {

            }
            //SearchReq.Invoke(null, null);
        }

    }
}
