using GoogleMapsUnofficial.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class MyLocationUserControl : UserControl
    {
        int count = 0;
        public MyLocationUserControl()
        {
            this.InitializeComponent();
            GeoLocatorHelper.LocationFetched += GeoLocatorHelper_LocationFetched;
        }
        
        private async void GeoLocatorHelper_LocationFetched(object sender, Geoposition e)
        {
            var pos = e;
            if (pos == null || pos.Coordinate == null || pos.Coordinate.Point == null) return;
            BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
            Geopoint snPoint = new Geopoint(snPosition);
            await Task.Delay(10);
            var Map = MapView.MapControl;
            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                MapViewVM.UserLocation.Location = snPoint;
                Map.Center = snPoint;
                await MapView.MapControl.TryZoomToAsync(16);
            });
        }

        private async void RunBTNClick()
        {
            count++;
            await Task.Delay(350);
            if (count == 0) return;
            if (count == 1)
            {
                try
                {
                    await Task.Delay(10);
                    GeoLocatorHelper.GetUserLocation();
                }
                catch { }
            }
            else
            {
                try
                {
                    if (!GeoLocatorHelper.IsLocationBusy)
                    {
                        if (MapViewVM.Compass != null)
                        {
                            MapViewVM.CompassEnabled = !MapViewVM.CompassEnabled;
                            if (MapViewVM.CompassEnabled)
                                thisbtn.Content = "";
                            else thisbtn.Content = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
            count = 0;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AppCore.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, RunBTNClick);
        }
        
    }
}
