using GMapsUWP;
using GMapsUWP.GeoCoding;
using GMapsUWP.Place;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel.OnMapControls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinGoMapsX.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        public MapView()
        {
            this.InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter !=null)
            {
                //Google Maps Override
                if (e.Parameter.ToString().StartsWith("http"))
                {
                    //Google Maps URI Override removed due to removing of Website association of google.com
                }

                //Windows Maps Override
                else
                {
                    var parameters = ((Uri)e.Parameter).DecodeQueryParameters();
                    string cp = "";
                    int zoomlevel = 0;
                    string Querry = "";
                    string Where = "";
                    //{bingmaps:?where=Tabarsi Square%2C north side of the Shrine%2C Mashhad%2C 2399%2C Īrān}
                    if (parameters.Where(x => x.Key == "where").Any())
                        Where = Uri.UnescapeDataString(parameters.Where(x => x.Key == "where").FirstOrDefault().Value.NoHTMLString());
                    if (parameters.Where(x => x.Key == "cp").Any())
                        cp = parameters.Where(x => x.Key == "cp").FirstOrDefault().Value;
                    if (parameters.Where(x => x.Key == "lvl").Any())
                        zoomlevel = Convert.ToInt32(parameters.Where(x => x.Key == "lvl").FirstOrDefault().Value);
                    if (parameters.Where(x => x.Key == "q").Any())
                        Querry = parameters.Where(x => x.Key == "q").FirstOrDefault().Value;
                    if (parameters.Where(x => x.Key == "collection").Any())
                    {
                        var point = parameters.Where(x => x.Key == "collection").FirstOrDefault().Value;
                        var pointargs = point.Split('_');
                        var latitude = pointargs[0].Split('.')[1] + "." + pointargs[0].Split('.')[2];
                        var longitude = pointargs[1];
                        cp = $"{latitude}~{longitude}";
                        if (parameters.Count >= 3)
                            Map.MapElements.Add(new MapIcon() { Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(latitude), Longitude = Convert.ToDouble(longitude) }), Title = pointargs[2].Replace("+", " ") });
                        else
                            Map.MapElements.Add(new MapIcon() { Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(latitude), Longitude = Convert.ToDouble(longitude) }), Title = "Point" });
                        
                    }
                    if (Where != "")
                    {
                        var res = await PlaceSearchHelper.TextSearch(Where);
                        if (res != null)
                        {
                            if (res.Results != null && res.Results.Any())
                            {
                                var loc = res.Results.FirstOrDefault().Geometry.Location;
                                //var rgc = await ReverseGeoCode.GetLocation(Where);
                                Map.Center = new Geopoint(new BasicGeoposition() { Latitude = loc.Latitude, Longitude = loc.Longitude });
                                MapViewVM.MapRightTapCmd.Execute(new Geopoint(new BasicGeoposition() { Latitude = loc.Latitude, Longitude = loc.Longitude }));
                            }
                            else
                            {
                                var rgc = await ReverseGeoCode.GetLocation(Where);
                                var b = rgc.Geometry.Bounds;
                                var loc = rgc.Geometry.Location;
                                await Map.TrySetViewBoundsAsync(new GeoboundingBox(new BasicGeoposition() { Latitude = b.NorthEast.Latitude, Longitude = b.SouthWest.Longitude }, new BasicGeoposition() { Latitude = b.SouthWest.Latitude, Longitude = b.NorthEast.Longitude }), null, MapAnimationKind.Bow);
                                
                            }
                        }
                        else
                        {
                            var rgc = await ReverseGeoCode.GetLocation(Where);
                            var b = rgc.Geometry.Bounds;
                            var loc = rgc.Geometry.Location;
                            await Map.TrySetViewBoundsAsync(new GeoboundingBox(new BasicGeoposition() { Latitude = b.NorthEast.Latitude, Longitude = b.SouthWest.Longitude }, new BasicGeoposition() { Latitude = b.SouthWest.Latitude, Longitude = b.NorthEast.Longitude }), null, MapAnimationKind.Bow);
                        }
                    }
                    if (cp != "")
                    {
                        await Task.Delay(500);
                        var bgp = new BasicGeoposition();
                        bgp.Latitude = Convert.ToDouble(cp.Split('~')[0]);
                        bgp.Longitude = Convert.ToDouble(cp.Split('~')[1]);
                        Map.Center = new Geopoint(bgp);
                        Map.MapElements.Add(new MapIcon() { Location = new Geopoint(bgp), Title = "Point" });
                    }
                    if (zoomlevel != 0) await Map.TryZoomToAsync(zoomlevel);
                    else await Map.TryZoomToAsync(16);
                    if (Querry != "")
                    {
                        await Task.Delay(1500);
                        (Searchbar.DataContext as SearchUserControlVM).TextChangedCmd.Execute(Querry);
                    }
                }
                MapViewVM.HavingParameter = true;
            }
            MapViewVM.Map = Searchbar.Map = ZoomUserControl.Map = DirectionsControl.Map = ChangeViewControl.Map = Map;
            MapViewVM.NewDirections = DirectionsControl;
            StepsTitleProvider.MapView = this;
            MyLocationControl.MapViewVM = (Searchbar.DataContext as SearchUserControlVM).VM = MapViewVM;
            DirectionsControl.StepsTitleProvider = StepsTitleProvider;
            MapViewVM.OnNavigatedTo();

            if (MapViewVM.UserLocation != null) MapViewVM.ReRun();
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
            var drawingAttr = this.inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            drawingAttr.PenTip = PenTipShape.Rectangle;
            drawingAttr.Size = new Size(4, 4);
            drawingAttr.IgnorePressure = true;
            drawingAttr.Color = (Color)Resources["SystemControlBackgroundAccentBrush"];
            this.inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttr);
            if (!InternalHelper.InternetConnection())
            {
                Map.TileSources.Add(new MapTileSource(new LocalMapTileDataSource("ms-appdata:///local/MahMaps/mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg")) { AllowOverstretch = false, IsFadingEnabled = false });
            }
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {

                if (!InternalHelper.InternetConnection())
                {
                    Map.TileSources.Clear();
                    Map.TileSources.Add(new MapTileSource(new LocalMapTileDataSource("ms-appdata:///local/MahMaps/mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg")) { AllowOverstretch = false, IsFadingEnabled = false });
                }
                else
                {
                    var c = (ChangeViewControl.DataContext as ChangeViewUCVM);
                    c.UseGoogleMaps(c.CurrentMapMode, c.ShowTraffic, true, c.AllowOverstretch, c.FadeAnimationEnabled);
                }
            });
        }

        private void InkingBTN_Click(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Visibility == Visibility.Collapsed)
            {
                inkCanvas.Visibility = Visibility.Visible;
                DirectionsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                inkCanvas.Visibility = Visibility.Collapsed;
                DirectionsControl.Visibility = Visibility.Visible;
            }
        }

        private async void Map_MapRightTapped(MapControl sender, MapRightTappedEventArgs args) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate { MapViewVM.IsPaneOpen = false; MapViewVM.MapRightTapCmd.Execute(args.Location); });

        private async void SetOrigin_Click(object sender, TappedRoutedEventArgs e) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate { MapViewVM.IsPaneOpen = false; DirectionsControl.Origin = MapViewVM.LastRightTapPos; });

        private async void AddWaypoint_Click(object sender, TappedRoutedEventArgs e) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate { MapViewVM.IsPaneOpen = false; DirectionsControl.Waypoints.Add(MapViewVM.LastRightTapPos.Position); });

        private void ShareLocation_Click(object sender, TappedRoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
            MapViewVM.IsPaneOpen = false;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetWebLink(new Uri($"https://www.google.com/maps/@?api=1&map_action=map&center={MapViewVM.LastRightTapPos.Position.Latitude},{MapViewVM.LastRightTapPos.Position.Longitude}&zoom={Convert.ToInt16(Map.ZoomLevel)}",
                UriKind.RelativeOrAbsolute));
            if (MapViewVM.SearchResult != null)
            {
                request.Data.Properties.Title = $"{MapViewVM.SearchResult.Result.Name} on Google maps";
                request.Data.Properties.Description = $"See {MapViewVM.SearchResult.Result.Name} on Google Maps. Shared using WinGo Maps for Windows 10.";
            }
            else
            {
                request.Data.Properties.Title = $"No name place on Google maps";
                request.Data.Properties.Description = $"See No name place on Google Maps. Shared using WinGo Maps for Windows 10.";
            }
        }

        private void SplitView_PaneClosed(SplitView sender, object args) => MapViewVM.MoreInfoVisibility = Visibility.Collapsed;

        private void PlacePhone_Click(object sender, TappedRoutedEventArgs e) => PhoneCallManager.ShowPhoneCallUI(MapViewVM.SearchResult.Result.FormatedPhoneNumber, MapViewVM.SearchResult.Result.Name);

        private async void PlaceWebsite_Click(object sender, TappedRoutedEventArgs e) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate { MapViewVM.IsPaneOpen = false; await Launcher.LaunchUriAsync(new Uri(MapViewVM.SearchResult.Result.Website, UriKind.RelativeOrAbsolute)); });

        private async void RatePlace_Click(object sender, TappedRoutedEventArgs e) => await Launcher.LaunchUriAsync(new Uri("https://search.google.com/local/writereview?placeid=" + MapViewVM.LastRightTapPos));

        private void MoreInfoHyperLink_Click(object sender, RoutedEventArgs e) => MapViewVM.MoreInfoVisibility = Visibility.Visible;

        private async void AddBookmark_Click(object sender, TappedRoutedEventArgs e)
        {
            await new BookmarkAdd(new BookmarkAddNeedsClass
            {
                Location = MapViewVM.LastRightTapPos,
                PlaceName = MapViewVM.SearchResult != null ? MapViewVM.SearchResult.Result.Name : ""
            }).ShowAsync();
            MapViewVM.IsPaneOpen = false;
        }
    }
}
