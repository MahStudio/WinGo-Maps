using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace GoogleMapsUnofficial.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string attractionname;
        private Geopoint location;
        private Geopoint center;
        public Geopoint Center
        {
            get { return center; }
            set
            {
                center = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Center"));
            }
        }
        public Geopoint Location
        {
            get { return location; }
            set
            {
                location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
            }
        }
        public static Geopoint UserLocation { get; set; }
        public string AttractionName
        {
            get { return attractionname; }
            set
            {
                attractionname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AttractionName"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    class MapViewVM : INotifyPropertyChanged
    {
        private Visibility _locflagvisi;
        public Visibility LocationFlagVisibility { get { return _locflagvisi; } set { _locflagvisi = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationFlagVisibility")); } }
        public static Compass Compass { get; set; }
        public static bool CompassEnabled { get; set; }
        public static bool ActiveNavigationMode { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private MapControl Map;
        private CoreWindow CoreWindow;
        public static ViewModel UserLocation { get; set; }
        Geolocator geolocator = new Geolocator();
        public static Geolocator GeoLocate { get; set; }
        public static Geopoint FastLoadGeoPosition { get; set; }
        public MapViewVM()
        {
            CoreWindow = CoreWindow.GetForCurrentThread();
            LocationFlagVisibility = Visibility.Visible;
            UserLocation = new ViewModel() { AttractionName = "My Location" };
            LoadPage();
        }
        
        ~MapViewVM()
        {
            geolocator.PositionChanged -= Geolocator_PositionChanged;
            geolocator = null;
        }
        async void LoadPage()
        {
            geolocator = GeoLocate;
            Compass = Compass.GetDefault();
            if(Compass != null)
                Compass.ReadingChanged += Compass_ReadingChanged;
            ActiveNavigationMode = false;
            await CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                try
                {
                    var accessStatus = await Geolocator.RequestAccessAsync();
                    if (accessStatus == GeolocationAccessStatus.Allowed)
                    {
                        if (FastLoadGeoPosition != null)
                        {
                            // Subscribe to the StatusChanged event to get updates of location status changes.
                            //geolocator.StatusChanged += Geolocator_StatusChanged;
                            geolocator.PositionChanged += Geolocator_PositionChanged;
                            // Carry out the operation.
                            Geopoint pos = FastLoadGeoPosition;

                            var MyLandmarks = new List<MapElement>();

                            Geopoint snPoint = new Geopoint(new BasicGeoposition { Latitude = pos.Position.Latitude, Longitude = pos.Position.Longitude });
                            UserLocation.Location = snPoint;
                            await Task.Delay(10);
                            Map = View.MapView.MapControl;
                            //Map.MapElements.Add(UserLoction);
                            Map.Center = snPoint;
                            Map.CenterChanged += Map_CenterChanged;
                            Map.ZoomLevel = 16;
                            var savedplaces = SavedPlacesVM.GetSavedPlaces();
                            foreach (var item in savedplaces)
                            {
                                Map.MapElements.Add(new MapIcon()
                                {
                                    Location = new Geopoint(new BasicGeoposition() { Latitude = item.Latitude, Longitude = item.Longitude }),
                                    Title = item.PlaceName
                                });
                            }
                            UserLocation.Location = snPoint;
                            //if (Map.Is3DSupported)
                            //{
                            //    Map.Style = MapStyle.Aerial3DWithRoads;
                            //    MapScene mapScene = MapScene.CreateFromLocationAndRadius(snPoint, 500, 150, 70);
                            //    await Map.TrySetSceneAsync(mapScene);
                            //}
                            //var r = await MapLocationFinder.FindLocationsAtAsync(snPoint);
                            //if(r.Locations != null)
                            //{
                            //    var re = r.Locations.FirstOrDefault();
                            //    var rg = RegionInfo.CurrentRegion;
                            //    var rg2 = new RegionInfo(re.Address.Country);
                            //}
                        }
                        else
                        {
                            LocationFlagVisibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        LocationFlagVisibility = Visibility.Collapsed ;
                        var msg = new MessageDialog("We weren't able to access your location. Please check if your device location is on and you have accepted location access to the app in privacy settings.\nHit ok button to navigate location settings and cancel to continue.");
                        msg.Commands.Add(new UICommand("OK", async delegate
                        {
                            await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location", UriKind.RelativeOrAbsolute));
                        }));
                        msg.Commands.Add(new UICommand("Cancel", delegate { }));
                        await msg.ShowAsync();
                    }
                }
                catch { }
            });
        }

        private async void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            await CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, delegate
            {
                if (CompassEnabled)
                    if (args.Reading.HeadingTrueNorth.HasValue)
                        Map.Heading = args.Reading.HeadingTrueNorth.Value;
            });
        }

        private void Map_CenterChanged(MapControl sender, object args)
        {
            try
            {
                UserLocation.Center = Map.Center;
            }
            catch { }
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    if (Map == null || UserLocation == null) return;
                    UserLocation.Location = args.Position.Coordinate.Point;
                    if (ActiveNavigationMode == true)
                        Map.Center = args.Position.Coordinate.Point;
                });
            }
            catch { }
        }
    }
}
