using GoogleMapsUnofficial.ViewModel.OfflineMapDownloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        Geopoint bgn_pnt;
        Geopoint end_pnt;
        public static MapControl MapControl;
        public MapView()
        {
            this.InitializeComponent();
            MapControl = Map;
            Map.TileSources.Clear();
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            if ( connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess )
            {
                Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}") { AllowCaching = true }));
            }
            else
            {
                Map.TileSources.Add(new MapTileSource(new LocalMapTileDataSource("ms-appdata:///local/MahMaps/mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg")));
            }
        }

        private async void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            var dt = (DataContext as ViewModel.MapViewVM);
            var Origin = dt.UserLocation.Location;
            var Destination = args.Location;
            var r = await ViewModel.DirectionsControls.DirectionsHelper.GetDirections(Origin.Position, Destination.Position, ViewModel.DirectionsControls.DirectionsHelper.DirectionModes.walking);
            if(r == null || r.routes.Count() == 0)
            {
                await new MessageDialog("No way to your destination!!!").ShowAsync();
                return;
            }
            var route = ViewModel.DirectionsControls.DirectionsHelper.GetDirectionAsRoute(r);
            Map.MapElements.Add(route);
        }
    }
}
