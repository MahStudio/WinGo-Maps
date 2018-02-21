using GoogleMapsUnofficial.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
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
            var logfile = await KnownFolders.PicturesLibrary.CreateFileAsync("WhereIsCrash.Log", CreationCollisionOption.GenerateUniqueName);
            count++;
            await FileIO.WriteTextAsync(logfile, "Count++;");
            await Task.Delay(350);
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                if (count == 0) return;
                if (count == 1)
                {
                    await FileIO.WriteTextAsync(logfile, "Count is 1");
                    try
                    {
                        await FileIO.WriteTextAsync(logfile, "try");
                        var accessStatus = await Geolocator.RequestAccessAsync();
                        await FileIO.WriteTextAsync(logfile, "RequestAccessAsync");
                        if (accessStatus == GeolocationAccessStatus.Allowed)
                        {
                            await FileIO.WriteTextAsync(logfile, "Allowed");
                            if (MapViewVM.GeoLocate == null) return;
                            await FileIO.WriteTextAsync(logfile, "GeoLocate is NOT NULL");
                            if (MapViewVM.GeoLocate.LocationStatus == PositionStatus.Ready)
                            {
                                await FileIO.WriteTextAsync(logfile, "GeoLocate.LocationStatus == PositionStatus.Ready");
                                Geoposition pos = await MapViewVM.GeoLocate.GetGeopositionAsync();
                                await FileIO.WriteTextAsync(logfile, "GeoLocate.GetGeopositionAsync");
                                if (pos == null) return;
                                await FileIO.WriteTextAsync(logfile, "pos is NOT NULL");
                                Geopoint snPoint = pos.Coordinate.Point;
                                await FileIO.WriteTextAsync(logfile, "snPoint" + snPoint.Position.Latitude + "," + snPoint.Position.Longitude);
                                var Map = MapView.MapControl;
                                await Task.Delay(10);
                                if (Map == null)
                                    await FileIO.WriteTextAsync(logfile, "Map is NULL");
                                else
                                    await FileIO.WriteTextAsync(logfile, "Map is NOT NULL");
                                Map.Center = snPoint;
                                await FileIO.WriteTextAsync(logfile, "Map.Center");
                                Map.ZoomLevel = 16;
                                await FileIO.WriteTextAsync(logfile, "Map.ZoomLevel");
                                MapViewVM.UserLocation.Location = snPoint;
                                await FileIO.WriteTextAsync(logfile, "UserLocation.Location");
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    if (MapViewVM.Compass != null)
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
