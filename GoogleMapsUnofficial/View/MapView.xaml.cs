using GoogleMapsUnofficial.ViewModel.SettingsView;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

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
            Map.ZoomInteractionMode = SettingsSetters.GetZoomControlsVisible();
            Map.RotateInteractionMode = SettingsSetters.GetRotationControlsVisible();
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
                string mapuri = "http://mt1.google.com/vt/lyrs=r&hl=" + AppCore.OnMapLanguage + "&z={zoomlevel}&x={x}&y={y}";
                Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource(mapuri)
                { AllowCaching = true })
                { AllowOverstretch = AllowOverstretch, IsFadingEnabled = FadeAnimationEnabled, ZoomLevelRange = new MapZoomLevelRange() { Max = 22, Min = 0 } });
            }
            else
            {
                Map.TileSources.Add(new MapTileSource(new LocalMapTileDataSource("ms-appdata:///local/MahMaps/mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg")) { AllowOverstretch = AllowOverstretch, IsFadingEnabled = FadeAnimationEnabled });
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter != null)
            {
                await Task.Delay(500);
                if(e.Parameter.ToString().StartsWith("search,"))
                {
                    Searchgrid.PopUP = true;
                    Searchgrid.SearchText = e.Parameter.ToString().Replace("search,?api=1&query=", string.Empty);
                }
            }
        }

        private void Hm_UriRequested(HttpMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            var res = TileCoordinate.ReverseGeoPoint(args.X, args.Y, args.ZoomLevel);
            args.Request.Uri = new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={res.Latitude},{res.Longitude}&zoom={args.ZoomLevel}&maptype=traffic&size=256x256&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute);
        }
    }
}
