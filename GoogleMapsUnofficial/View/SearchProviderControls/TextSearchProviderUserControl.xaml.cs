using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.SearchProviderControls
{
    public sealed partial class TextSearchProviderUserControl : UserControl
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
                TextSearchProviderVM.SearchQuery = value;
            }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
         "SearchText",
         typeof(string),
         typeof(TextSearchProviderUserControl),
         new PropertyMetadata(null)
        );
        public TextSearchProviderUserControl()
        {
            this.InitializeComponent();
            this.Loaded += TextSearchProviderUserControl_Loaded;
        }

        private void TextSearchProviderUserControl_Loaded(object sender, RoutedEventArgs e)
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
                TextSearchProviderVM.SearchQuery = (sender as TextBox).Text;
            }
            catch { }
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var select = e.ClickedItem as PlaceAutoComplete.Prediction;
            if (select == null) return;
            var res = await GeocodeHelper.GetInfo(select.place_id);
            if (res == null) return;
            var ploc = res.results.FirstOrDefault().geometry.location;
            MapView.MapControl.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.lat,
                    Longitude = ploc.lng
                });
        }
        
    }
}
