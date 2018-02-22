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
        int count = 0;
        public MyLocationUserControl()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            count++;
            await Task.Delay(350);
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                if (count == 0) return;
                if (count == 1)
                {
                    try
                    {
                        if(MapViewVM.GeoLocate.LocationStatus == PositionStatus.Ready)
                        {
                            await Task.Delay(10);
                            Geoposition pos = await MapViewVM.GeoLocate.GetGeopositionAsync();
                            if (pos == null) return;
                            BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                            Geopoint snPoint = new Geopoint(snPosition);
                            await Task.Delay(10);
                            var Map = MapView.MapControl;
                            Map.Center = snPoint;
                            Map.ZoomLevel = 16;
                            MapViewVM.UserLocation.Location = snPoint;
                        }
                    }
                    catch { }
                }
                else
                {
                    if(MapViewVM.Compass != null)
                    {
                        MapViewVM.CompassEnabled = !MapViewVM.CompassEnabled;
                        if (MapViewVM.CompassEnabled)
                            (sender as Button).Content = "";
                        else (sender as Button).Content = "";
                    }
                }
                count = 0;
            });
        }

        private async void thisbtn_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            await new Windows.UI.Popups.MessageDialog("Hold").ShowAsync();
        }
    }
}
