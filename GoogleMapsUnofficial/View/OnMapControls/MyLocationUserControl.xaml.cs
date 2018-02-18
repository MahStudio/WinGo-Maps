using GoogleMapsUnofficial.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class MyLocationUserControl : UserControl
    {
        public MyLocationUserControl()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    Geolocator geolocator = new Geolocator();

                    Geoposition pos = await MapViewVM.GeoLocate.GetGeopositionAsync();
                    
                    BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                    Geopoint snPoint = new Geopoint(snPosition);
                    await Task.Delay(10);
                    var Map = MapView.MapControl;
                    Map = View.MapView.MapControl;
                    Map.Center = snPoint;
                    Map.ZoomLevel = 16;
                    MapViewVM.UserLocation.Location = snPoint;
                }
            });
        }
    }
}
