using GMapsUWP;
using GMapsUWP.Photos;
using GMapsUWP.Place;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using WinGoMapsX.Helpers;
using WinGoMapsX.View.DirectionsControls;
using WinGoMapsX.ViewModel.PlacesControls;

namespace WinGoMapsX.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private double? _heading;
        private Geopoint location;
        private Geopoint center;
        public Geopoint Location
        {
            get => location;
            set
            {
                location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
            }
        }
        public Geopoint Center
        {
            get => center;
            set
            {
                center = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Center"));
            }
        }
        public double? Heading
        {
            get => _heading;
            set { _heading = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heading")); }
        }
        //public static Geopoint UserLocation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MapViewVM : INotifyPropertyChanged
    {
        public bool HavingParameter { get; set; }
        public NewDirections NewDirections { get; set; }
        public void Update(string PropName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));
        }

        private bool _rightpaneopen;
        private bool _ispaneopen;
        private ViewModel _userlocation;
        private Compass _compass;
        private Visibility _locflagvisi;
        private Visibility _moreinfvis;
        //private Visibility _headinglocvis;

        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility LocationFlagVisibility
        {
            get => _locflagvisi;
            set { _locflagvisi = value; Update("LocationFlagVisibility"); }
        }
        public Visibility MoreInfoVisibility
        {
            get => _moreinfvis;
            set { _moreinfvis = value; Update("MoreInfoVisibility"); Update("MoreInfoHyperVisibility"); }
        }
        //public Visibility HeadingLocIndicatorVisibility
        //{
        //    get => _headinglocvis;
        //    set { _headinglocvis = value; Update("HeadingLocIndicatorVisibility"); Update("NormalLocIndicatorVisibility"); }
        //}
        public Visibility MoreInfoHyperVisibility { get => MoreInfoVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed; }
        public bool IsPaneOpen
        {
            get => _ispaneopen;
            set { _ispaneopen = value; Update("IsPaneOpen"); }
        }
        public bool RightPaneOpen
        {
            get => _rightpaneopen;
            set { _rightpaneopen = value; Update("RightPaneOpen"); }
        }
        public bool CompassMode { get; set; }

        public Geopoint LastRightTapPos { get; set; }
        public ViewModel UserLocation { get => _userlocation; set { _userlocation = value; Update("UserLocation"); } }

        public AppCommand MapRightTapCmd { get; set; }
        public AppCommand FindDirectionsCmd { get; set; }
        CoreDispatcher Dispatcher { get; set; }
        public MapControl Map { get; set; }

        PlaceDetailsHelper.Rootobject _searchres { get; set; }
        Uri _puru { get; set; }
        public PlaceDetailsHelper.Rootobject SearchResult { get => _searchres; set { _searchres = value; Update("SearchResult"); } }
        public Uri PictureURI { get => _puru; set { _puru = value; Update("PictureURI"); } }

        public Compass CompassDevice { get => _compass; set { _compass = value; HandleCompass(); } }

        private void HandleCompass()
        {
            if (CompassDevice != null)
            {
                CompassDevice.GetCurrentReading();
                CompassDevice.ReportInterval = 1000;
                CompassDevice.ReadingChanged += CompassDevice_ReadingChanged;
            }
        }

        private async void CompassDevice_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            if (CompassMode && args.Reading.HeadingTrueNorth.HasValue && args.Reading.HeadingTrueNorth.Value != Double.NaN)
                await Map.TryRotateAsync(args.Reading.HeadingTrueNorth.Value);
        }

        public MapViewVM()
        {
            CompassDevice = Compass.GetDefault();
            IsPaneOpen = RightPaneOpen = false;
            LocationFlagVisibility = MoreInfoVisibility = Visibility.Collapsed;
            Dispatcher = Window.Current.Dispatcher;
            FindDirectionsCmd = AppCommand.GetInstance();
            FindDirectionsCmd.ExecuteFunc = FindDirections;
            MapRightTapCmd = AppCommand.GetInstance();
            MapRightTapCmd.ExecuteFunc = MapRightTap;
            UserLocation = new ViewModel();
            GeoLocatorHelper.LocationChanged += GeoLocatorHelper_LocationChanged;
            GeoLocatorHelper.LocationFetched += GeoLocatorHelper_LocationFetched;
        }

        public void OnNavigatedTo()
        {
            List<MapIcon> Micons = new List<MapIcon>();
            foreach (var item in Map.MapElements)
            {
                if (item.GetType() == typeof(MapIcon))
                    Micons.Add(item as MapIcon);
            }
            foreach (var item in Micons)
            {
                Map.MapElements.Remove(item);
            }
            foreach (var item in SavedPlacesVM.GetSavedPlaces())
            {
                Map.MapElements.Add(new MapIcon()
                {
                    Location = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = item.Latitude, Longitude = item.Longitude }),
                    Title = item.PlaceName
                });
            }
        }

        private async void MapRightTap(object obj)
        {
            NewDirections.Destination = LastRightTapPos = obj as Geopoint;
            IsPaneOpen = true;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RunRightTapped);
        }

        private async void RunRightTapped()
        {
            var t = (await PlaceSearchHelper.NearbySearch(LastRightTapPos.Position, 5));
            if (t != null)
            {
                var pic = t.Results.Where(x => LastRightTapPos.DistanceTo(new Geopoint(new BasicGeoposition() { Latitude = x.Geometry.Location.Latitude, Longitude = x.Geometry.Location.Longitude })) < 1)
                    .OrderBy(x => LastRightTapPos.DistanceTo(new Geopoint(new BasicGeoposition() { Latitude = x.Geometry.Location.Latitude, Longitude = x.Geometry.Location.Longitude }))).FirstOrDefault();
                //var pic = t.Results.Where(x => x.photos != null).LastOrDefault();
                if (pic != null)
                {
                    //LastPlaceID = pic.place_id;
                    if (pic.Photos != null)
                    {
                        PictureURI = PhotosHelper.GetPhotoUri(pic.Photos.FirstOrDefault().PhotoReference, 350, 350);
                    }
                    else PictureURI = null;
                    var det = await PlaceDetailsHelper.GetPlaceDetails(pic.PlaceId);
                    SearchResult = det;
                    if (det == null)
                        await new MessageDialog(det.Status).ShowAsync();
                }
            }
            else await new MessageDialog("Something went wrong!").ShowAsync();
        }

        public void ReRun() => GeoLocatorHelper.GetUserLocation();

        private async void GeoLocatorHelper_LocationFetched(object sender, Geoposition e) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate { await Task.Delay(500); if (HavingParameter) { HavingParameter = false; return; } LocationFlagVisibility = Visibility.Visible; await Map.TrySetViewAsync(e.Coordinate.Point, Map.MaxZoomLevel, 0, null, MapAnimationKind.Bow); Update("UserLocation"); });

        private async void GeoLocatorHelper_LocationChanged(object sender, Geocoordinate e) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate { UserLocation.Location = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = e.Point.Position.Latitude, Longitude = e.Point.Position.Longitude }, AltitudeReferenceSystem.Terrain); Update("UserLocation"); });

        private void FindDirections(object obj)
        {
            if (NewDirections.Origin == null) NewDirections.Origin = UserLocation.Location;
            NewDirections.DirectionFinder();
            IsPaneOpen = false;
        }
    }
}
