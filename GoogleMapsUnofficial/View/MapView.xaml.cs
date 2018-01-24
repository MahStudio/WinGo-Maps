using GoogleMapsUnofficial.ViewModel.OfflineMapDownloader;
using GoogleMapsUnofficial.ViewModel.SettingsView;
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
        public static MapControl MapControl;
        public MapView()
        {
            this.InitializeComponent();
            MapControl = Map;
            Map.Style = MapStyle.None;
            Map.TileSources.Clear();
            var AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            var FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
            if (InternalHelper.InternetConnection())
            {
                //var hm = new HttpMapTileDataSource() { AllowCaching = true };
                //var md = new MapTileSource(hm) { AllowOverstretch = false };
                //Map.TileSources.Add(md);
                //hm.UriRequested += Hm_UriRequested;
                //New
                //Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("https://www.googleapis.com/tile/v1/tiles/{x}/{y}/{zoomlevel}?session=sessiontoken&key=AIzaSyCFQ-I2-SPtdtVR4TCa6665mLMX5n_I5Sc")
                //{ AllowCaching = true })
                //{ AllowOverstretch = false, IsFadingEnabled = false, ZoomLevelRange = new MapZoomLevelRange() { Max = 22, Min = 1 } });
                //OLD
                Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}")
                    { AllowCaching = true })
                        { AllowOverstretch = AllowOverstretch, IsFadingEnabled = FadeAnimationEnabled, ZoomLevelRange = new MapZoomLevelRange() { Max = 22, Min = 0 } });
            }
            else
            {
                Map.TileSources.Add(new MapTileSource(new LocalMapTileDataSource("ms-appdata:///local/MahMaps/mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg")) { AllowOverstretch = AllowOverstretch, IsFadingEnabled = FadeAnimationEnabled });
            }
        }

        private void Hm_UriRequested(HttpMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            var res = TileCoordinate.ReverseGeoPoint(args.X, args.Y, args.ZoomLevel);
            args.Request.Uri = new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={res.Latitude},{res.Longitude}&zoom={args.ZoomLevel}&maptype=traffic&size=256x256&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute);
        }
    }
}
