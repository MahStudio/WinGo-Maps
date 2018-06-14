using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
//Fixed bugs of search bar

namespace GoogleMapsUnofficial.View.OnMapControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchUserControl : UserControl
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
                await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
                {
                    if (SearchResults == null) SearchResults = new ObservableCollection<PlaceAutoComplete.Prediction>();
                    SearchResults.Clear();
                    var saved = SavedPlacesVM.GetSavedPlaces();
                    saved = saved.Where(x => x.PlaceName.Contains(searchExpression)).ToList();
                    foreach (var item in saved)
                    {
                        SearchResults.Add(new PlaceAutoComplete.Prediction() { place_id = $"{item.Latitude},{item.Longitude}", description = "Saved:" + item.PlaceName });
                    }
                    if (await SettingsSetters.GetAccessToContcts())
                    {
                        var contacts = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                        if (contacts != null)
                        {
                            var search = await contacts.FindContactsAsync(searchExpression);
                            var res = search.Where(x => x.Addresses.Count > 0);
                            foreach (var item in res)
                            {
                                var cadd = item.Addresses.FirstOrDefault();
                                SearchResults.Add(new PlaceAutoComplete.Prediction() { description = "Contacts:" + item.Name, place_id = $"{cadd.StreetAddress}, {cadd.Locality}" });
                            }
                        }
                    }
                    if (searchExpression.Length >= 3)
                    {
                        var s = await PlaceAutoComplete.GetAutoCompleteResults(searchExpression, location: MapView.MapControl.Center, radius: 50000);
                        if (s == null) return;
                        foreach (var item in s.predictions)
                        {
                            SearchResults.Add(item);
                        }
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
                SearchBox.Focus(FocusState.Keyboard);
                SearchBox.Text = value;
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
            if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone)
                this.Height = 48;
            this.DataContext = new ViewModel();
            SearchBox.LostFocus += SearchBox_LostFocus;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //SearchBox.Visibility = Visibility.Collapsed;
            //BTNExpand.Visibility = Visibility.Visible;
            SearchBox.Text = "";
            try
            {
                (DataContext as ViewModel).SearchResults.Clear();
            }
            catch { }
        }
        private async void Control2_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //SearchReq.Invoke(args.QueryText, null);
            //SearchBox.Visibility = Visibility.Collapsed;
            //BTNExpand.Visibility = Visibility.Visible;
            var t = SearchBox.Text;
            SearchBox.Text = "";
            try
            {
                (DataContext as ViewModel).SearchResults.Clear();
            }
            catch { }
            if (args.ChosenSuggestion != null) return;
            MapView.StaticSearchGrid.PopUP = true;
            MapView.StaticSearchGrid.SearchText = t;
            MapView.StaticSearchGrid.IsNearbySearch = false;
        }

        private async void Control2_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var select = (args.SelectedItem as PlaceAutoComplete.Prediction);
            if (select == null) return;
            if (select.description.StartsWith("Saved:"))
            {
                SearchBox.Text = "";
                var loc = select.place_id.Split(',');
                var lat = loc[0]; var lng = loc[1];
                MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = Convert.ToDouble(lat),
                    Longitude = Convert.ToDouble(lng)
                });
                MapView.StaticMapView.SearchResultPoint = new Geopoint(
                    new BasicGeoposition()
                    {
                        Latitude = Convert.ToDouble(lat),
                        Longitude = Convert.ToDouble(lng)
                    });
                (DataContext as ViewModel).SearchResults.Clear();
                SearchBox.Text = "";
                return;
            }
            if (select.description.StartsWith("Contacts:"))
            {
                var Addres = await SearchHelper.TextSearch(select.place_id);
                if (Addres != null)
                {
                    if (Addres.results != null && Addres.results.Any())
                    {
                        var loc = Addres.results.FirstOrDefault().geometry.location;
                        MapView.MapControl.Center = new Geopoint(
                            new BasicGeoposition()
                            {
                                Latitude = loc.lat,
                                Longitude = loc.lng
                            });
                        MapView.StaticMapView.SearchResultPoint = new Geopoint(
                            new BasicGeoposition()
                            {
                                Latitude = loc.lat,
                                Longitude = loc.lng
                            });
                        (DataContext as ViewModel).SearchResults.Clear();
                        SearchBox.Text = "";
                    }
                }
                return;
            }
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
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput || args.Reason == AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
            {
                if (SearchBox.Text != string.Empty)
                    (this.DataContext as ViewModel).SuggestForSearch((sender as AutoSuggestBox).Text);
            }
        }

    }
}
