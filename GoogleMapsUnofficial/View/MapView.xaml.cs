using GoogleMapsUnofficial.ViewModel.SettingsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
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
            if (e.Parameter != null)
            {
                //Search Uri association handler
                if (((Uri)e.Parameter).Segments[2].ToLower() == "search/")
                {
                    Searchgrid.PopUP = true;
                    Searchgrid.SearchText = ((Uri)e.Parameter).DecodeQueryParameters().Where(x => x.Key == "query").FirstOrDefault().Value;
                }
                //Directions Uri association handler
                if (((Uri)e.Parameter).Segments[2].ToLower() == "dir/")
                {
                    var parameters = ((Uri)e.Parameter).DecodeQueryParameters();
                    var origin = parameters.Where(x => x.Key == "origin").FirstOrDefault();
                    var destination = parameters.Where(x => x.Key == "destination").FirstOrDefault();
                    var travelmode = parameters.Where(x => x.Key == "travelmode").FirstOrDefault();
                    var waypoints = parameters.Where(x => x.Key == "waypoints").FirstOrDefault();
                    ViewModel.DirectionsControls.DirectionsHelper.DirectionModes Mode = ViewModel.DirectionsControls.DirectionsHelper.DirectionModes.walking;
                    Geopoint OriginPoint = null;
                    Geopoint DestinationPoint = null;
                    List<BasicGeoposition> lst = null;
                    if (travelmode.Value != null)
                    {
                        if (travelmode.Value.ToString() == "driving") Mode = ViewModel.DirectionsControls.DirectionsHelper.DirectionModes.driving;
                        else if (travelmode.Value.ToString() == "bicycling ") Mode = ViewModel.DirectionsControls.DirectionsHelper.DirectionModes.bicycling;
                        else if (travelmode.Value.ToString() == "transit") Mode = ViewModel.DirectionsControls.DirectionsHelper.DirectionModes.transit;
                    }
                    if (origin.Value != null)
                    {
                        var latlng = origin.Value.Split(',');
                        var Latitude = Convert.ToDouble(latlng[0]);
                        var Longitude = Convert.ToDouble(latlng[1]);
                        OriginPoint = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = Latitude,
                            Longitude = Longitude
                        });
                    }
                    if (destination.Value != null)
                    {
                        var latlng = destination.Value.Split(',');
                        var Latitude = Convert.ToDouble(latlng[0]);
                        var Longitude = Convert.ToDouble(latlng[1]);
                        DestinationPoint = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = Latitude,
                            Longitude = Longitude
                        });
                    }
                    if (waypoints.Value != null)
                    {
                        lst = new List<BasicGeoposition>();
                        var latlngs = destination.Value.Split('|');
                        foreach (var item in latlngs)
                        {
                            var latlng = item.Split(',');
                            BasicGeoposition point = new BasicGeoposition();
                            point.Latitude = Convert.ToDouble(latlng[0]);
                            point.Longitude = Convert.ToDouble(latlng[1]);
                            lst.Add(point);
                        }
                    }
                    if (OriginPoint != null && DestinationPoint != null)
                    {
                        ViewModel.DirectionsControls.DirectionsHelper.Rootobject Result = null;
                        if (lst == null)
                            Result = await ViewModel.DirectionsControls.DirectionsHelper.GetDirections(OriginPoint.Position, DestinationPoint.Position, Mode);
                        else
                            Result = await ViewModel.DirectionsControls.DirectionsHelper.GetDirections(OriginPoint.Position, DestinationPoint.Position, Mode, lst);
                        if (Result != null)
                        {
                            Map.MapElements.Add(ViewModel.DirectionsControls.DirectionsHelper.GetDirectionAsRoute(Result, Colors.Purple));
                        }
                    }
                }
                //Display a map
                if (((Uri)e.Parameter).Segments[2].ToLower() == "@")
                {
                    await Task.Delay(1500);
                    var parameters = ((Uri)e.Parameter).DecodeQueryParameters();
                    var mapaction = parameters.Where(x => x.Key == "map_action").FirstOrDefault();
                    if(mapaction.Value != null && mapaction.Value == "pano")
                    {
                        await new MessageDialog("StreetView Not Supported yet").ShowAsync();
                    }
                    var center = parameters.Where(x => x.Key == "center").FirstOrDefault();
                    var zoom = parameters.Where(x => x.Key == "zoom").FirstOrDefault();
                    var cp = center.Value.Split(',');
                    BasicGeoposition pointer = new BasicGeoposition() { Latitude = Convert.ToDouble(cp[0]), Longitude = Convert.ToDouble(cp[1]) };
                    Map.Center = new Geopoint(pointer);
                    if (zoom.Value != null)
                        Map.ZoomLevel = Convert.ToDouble(zoom.Value);

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
