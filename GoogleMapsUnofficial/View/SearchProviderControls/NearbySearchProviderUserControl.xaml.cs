using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.SearchProviderControls
{
    public sealed partial class NearbySearchProviderUserControl : UserControl
    {
        public NearbySearchProviderUserControl()
        {
            this.InitializeComponent();
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
                case "Super market":
                    NearbySearchProviderVM.PlaceType = SearchHelper.PlaceTypesEnum.supermarket;
                    break;
                default:
                    break;
            }
        }
    }
}
