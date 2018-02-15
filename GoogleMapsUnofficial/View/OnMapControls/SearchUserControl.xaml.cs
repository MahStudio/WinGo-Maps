using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;
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
            public string SearchQuerry
            {
                set
                {
                    SuggestForSearch(value);
                }
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

        public string SearchQuerry
        {
            set
            {
                SetValue(SearchQuerryProperty, value);
                (this.DataContext as ViewModel).SuggestForSearch(value);
            }
        }
        public static readonly DependencyProperty SearchQuerryProperty = DependencyProperty.Register(
         "SearchQuerry",
         typeof(string),
         typeof(SearchUserControl),
         new PropertyMetadata(null)
        );
        public SearchUserControl()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel();
            SearchBox.LostFocus += SearchBox_LostFocus;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //SearchBox.Visibility = Visibility.Collapsed;
            //BTNExpand.Visibility = Visibility.Visible;
            SearchBox.Text = "";
            (DataContext as ViewModel).SearchResults.Clear();
        }
        private void Control2_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //SearchReq.Invoke(args.QueryText, null);
            //SearchBox.Visibility = Visibility.Collapsed;
            //BTNExpand.Visibility = Visibility.Visible;
            SearchBox.Text = "";
            (DataContext as ViewModel).SearchResults.Clear();
        }

        private async void Control2_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var select = (args.SelectedItem as PlaceAutoComplete.Prediction);
            if (select == null) return;
            var res = await GeocodeHelper.GetInfo(select.place_id);
            SearchBox.Text = "";
            if (res == null || res.results.Length == 0)
            {
                await new MessageDialog("We couldn't find place location!").ShowAsync();
                return;
            }
            var ploc = res.results.FirstOrDefault().geometry.location;
            MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.lat,
                    Longitude = ploc.lng
                });
            MapView.StaticMapView.SearchResultPoint = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.lat,
                    Longitude = ploc.lng
                });
            (DataContext as ViewModel).SearchResults.Clear();
            //SearchReq.Invoke(args.SelectedItem as ClassProduct.Product, null);
            //SearchBox.Visibility = Visibility.Collapsed;
            //BTNExpand.Visibility = Visibility.Visible;
        }

        private void Control2_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text.Length >= 3)
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput || args.Reason == AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
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
