using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
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
                SearchResults = new ObservableCollection<SearchHelper.Result>();
                SearchResults.CollectionChanged += SuggestedApps_CollectionChanged;
            }

            private void SuggestedApps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChanged("SearchResults");
            }

            private ObservableCollection<SearchHelper.Result> _searchres;

            public ObservableCollection<SearchHelper.Result> SearchResults
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
                SearchResults.Clear();
                var s = await SearchHelper.TextSearch(query: searchExpression);
                foreach (var item in s.results)
                {
                    SearchResults.Add(item);
                }
                //foreach (var item in res.products)
                //{
                //    SuggestedApps.Add(new SearchResult()
                //    {
                //        Id = item.id,
                //        Image = new BitmapImage(new Uri(item.images[0].src, UriKind.RelativeOrAbsolute)),
                //        Price = item.price == null ? "نا موجود" : $"{item.price} ریـال",
                //        Title = item.title
                //    });
                //}

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

        private void Control2_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SearchBox.Text = "";
            MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = (args.SelectedItem as SearchHelper.Result).geometry.location.lat,
                    Longitude = (args.SelectedItem as SearchHelper.Result).geometry.location.lng
                });
            //SearchReq.Invoke(args.SelectedItem as ClassProduct.Product, null);
            SearchBox.Visibility = Visibility.Collapsed;
            BTNExpand.Visibility = Visibility.Visible;
        }

        private void Control2_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (SearchBox.Text != string.Empty)
                    (this.DataContext as ViewModel).SuggestForSearch((sender as AutoSuggestBox).Text);
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
