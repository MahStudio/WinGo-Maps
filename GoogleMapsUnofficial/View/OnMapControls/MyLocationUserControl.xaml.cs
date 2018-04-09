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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                var pos = e;
                if (pos == null || pos.Coordinate == null || pos.Coordinate.Point == null) return;
                BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                Geopoint snPoint = new Geopoint(snPosition);
                await Task.Delay(10);
                var Map = MapView.MapControl;
                Map.Center = snPoint;
                await MapView.MapControl.TryZoomToAsync(16);
                MapViewVM.UserLocation.Location = snPoint;
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            count++;
            await Task.Delay(350);
            await CoreWindow.GetForCurrentThread().Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async delegate
            {
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
                                    (sender as Button).Content = "";
                                else (sender as Button).Content = "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await new MessageDialog(ex.Message).ShowAsync();
                    }
                }
                count = 0;
            });
        }
        
    }
}
