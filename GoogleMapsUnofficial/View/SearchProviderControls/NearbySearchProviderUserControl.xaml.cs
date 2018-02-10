using GoogleMapsUnofficial.ViewModel.PlaceControls;
using Windows.Devices.Geolocation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.SearchProviderControls
{
    public sealed partial class NearbySearchProviderUserControl : UserControl
    {
        public string SearchText
        {
            get
            {
                return (string)GetValue(SearchTextProperty);
            }
            set
            {
                SetValue(SearchTextProperty, value);
                NearbySearchProviderVM.SearchQuery = value;
            }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
         "SearchText",
         typeof(string),
         typeof(NearbySearchProviderUserControl),
         new PropertyMetadata(null)
        );
        public NearbySearchProviderUserControl()
        {
            this.InitializeComponent();
            this.Loaded += NearbySearchProviderUserControl_Loaded;
        }

        private void NearbySearchProviderUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtsearch.Focus(FocusState.Programmatic);
                InputPane.GetForCurrentView().TryShow();
            }
            catch
            {

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                NearbySearchProviderVM.SearchQuery = (sender as TextBox).Text;
            }
            catch { }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var select = e.ClickedItem as SearchHelper.Result;
            if (select == null) return;
            var ploc = select.geometry.location;
            MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.lat,
                    Longitude = ploc.lng
                });
        }

        private void PlaceType_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag.ToString())
            {
                case "Restaurant":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.restaurant;
                    break;
                case "Cafe":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.cafe;
                    break;
                case "Book Store":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.book_store;
                    break;
                case "ATM":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.atm;
                    break;
                case "Pharmacy":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.pharmacy;
                    break;
                case "NONE":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.NOTMENTIONED;
                    break;
                default:
                    break;
            }
        }
    }
}
