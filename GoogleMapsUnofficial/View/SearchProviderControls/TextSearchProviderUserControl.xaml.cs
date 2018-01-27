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
    public sealed partial class TextSearchProviderUserControl : UserControl
    {
        public TextSearchProviderUserControl()
        {
            this.InitializeComponent();
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
