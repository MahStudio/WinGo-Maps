using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                    Geolocator geolocator = new Geolocator() { DesiredAccuracy = PositionAccuracy.High };
                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    //geolocator.StatusChanged += Geolocator_StatusChanged;
                    // Carry out the operation.
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    
                    BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                    Geopoint snPoint = new Geopoint(snPosition);
                    //UserLoction = new MapIcon
                    //{
                    //    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    //    Location = snPoint,
                    //    NormalizedAnchorPoint = new Point(0.5, 1.0),
                    //    ZIndex = 0,
                    //    Title = "Your Location"
                    //};
                    //MyLandmarks.Add(UserLoction);
                    await Task.Delay(10);
                    var Map = MapView.MapControl;
                    Map = View.MapView.MapControl;
                    //Map.MapElements.Add(UserLoction);
                    Map.Center = snPoint;
                    Map.ZoomLevel = 16;

                }
            });
        }
    }
}
