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
    }
}
