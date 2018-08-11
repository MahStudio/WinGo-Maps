﻿using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.System;
using Windows.System.Display;
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
        public Geopoint Location
        {
            get { return location; }
            set
            {
                location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
            }
        }
        //public static Geopoint UserLocation { get; set; }
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
        public static MapViewVM StaticVM { get; set; }
        private Visibility _stepsTitleProvidervisi;
        public Visibility StepsTitleProviderVisibility { get { return _stepsTitleProvidervisi; } set { _stepsTitleProvidervisi = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StepsTitleProviderVisibility")); } }
        private Visibility _locflagvisi;
        public Visibility LocationFlagVisibility { get { return _locflagvisi; } set { _locflagvisi = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationFlagVisibility")); } }
        public static Compass Compass { get; set; }
        public static bool CompassEnabled { get; set; }
        public static bool ActiveNavigationMode { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private MapControl Map;
        public static ViewModel UserLocation { get; set; }
        Geolocator geolocator = new Geolocator();
        public static Geolocator GeoLocate { get; set; }
        public static Geopoint FastLoadGeoPosition { get; set; }
        public MapViewVM()
        {
            LocationFlagVisibility = Visibility.Visible;
            StepsTitleProviderVisibility = Visibility.Collapsed;
            if (UserLocation == null)
                UserLocation = new ViewModel() { AttractionName = "My Location" };
            StaticVM = this;
            LoadPage();
        }

        async void LocateUser()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    Map = MapView.MapControl;
                    if (FastLoadGeoPosition != null)
                    {
                        if (geolocator == null)
                        {
                            geolocator = new Geolocator()
                            {
                                MovementThreshold = 1,
                                ReportInterval = 1,
                                DesiredAccuracyInMeters = 1
                            };
                            GeoLocate = geolocator;
                        }
                        else
                        {
                            UserLocation.Location = FastLoadGeoPosition;
                            Map.Center = FastLoadGeoPosition;
                            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.High, async delegate
                            {
                                await Map.TryZoomToAsync(16);
                            });
                        }
                        // Subscribe to the StatusChanged event to get updates of location status changes.
                        //geolocator.StatusChanged += Geolocator_StatusChanged;
                        // Carry out the operation.
                        GeoLocatorHelper.GetUserLocation();
                        GeoLocatorHelper.LocationFetched += GeoLocatorHelper_LocationFetched;
                        geolocator.PositionChanged += Geolocator_PositionChanged;
                        await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                        {
                            var savedplaces = SavedPlacesVM.GetSavedPlaces();
                            foreach (var item in savedplaces)
                            {
                                Map.MapElements.Add(new MapIcon()
                                {
                                    Location = new Geopoint(new BasicGeoposition() { Latitude = item.Latitude, Longitude = item.Longitude }),
                                    Title = item.PlaceName
                                });
                            }
                        });
                        await Task.Delay(150);
                        LocationFlagVisibility = Visibility.Visible;
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
                    LocationFlagVisibility = Visibility.Collapsed;
                    var msg = new MessageDialog(MultilingualHelpToolkit.GetString("StringLocationPrivacy", "Text"));
                    msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringOK", "Text"), async delegate
                    {
                        await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location", UriKind.RelativeOrAbsolute));
                    }));
                    msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringCancel", "Text"), delegate { }));
                    await msg.ShowAsync();
                    Window.Current.Activated += Current_Activated;
                }
            }
            catch { }
            try
            {
                ApplicationData.Current.LocalSettings.Values["WCReponse"].ToString();
            }
            catch 
            {
                var message = new MessageDialog("windowscentral is not a Microsoft News website that you are looking for. See our reply to WindowsCentral post about WinGo Maps.");
                message.Commands.Add(new UICommand("See our response on twitter.", async delegate
                {
                    await Launcher.LaunchUriAsync(new Uri("https://twitter.com/NGameAli/status/1028157663752978432"));
                }));
                await message.ShowAsync();
                ApplicationData.Current.LocalSettings.Values["WCReponse"] = "windowscentral is not a Microsoft News website that you are looking for";
            }

        }

        private async void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.CodeActivated)
            {
                Window.Current.Activated -= Current_Activated;
                if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
                {
                    LocateUser();
                }
            }
        }

        public async void LoadPage()
        {
            Compass = Compass.GetDefault();
            if (Compass != null)
                Compass.ReadingChanged += Compass_ReadingChanged;
            ActiveNavigationMode = false;
            geolocator = GeoLocate;
            Map = View.MapView.MapControl;
            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, LocateUser);
        }

        private async void GeoLocatorHelper_LocationFetched(object sender, Geoposition e)
        {
            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                var pos = e.Coordinate.Point;
                Geopoint snPoint = new Geopoint(new BasicGeoposition { Latitude = pos.Position.Latitude, Longitude = pos.Position.Longitude });
                await Task.Delay(10);
                //Map.MapElements.Add(UserLoction);
                if (Map == null)
                    Map = View.MapView.MapControl;
                if (Map != null)
                {
                    Map.Center = snPoint;
                    await Map.TryZoomToAsync(16);
                    UserLocation.Location = pos;
                }
            });
        }

        private async void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            await AppCore.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, delegate
            {
                try
                {
                    if (CompassEnabled)
                        if (args.Reading.HeadingTrueNorth.HasValue)
                            Map.Heading = args.Reading.HeadingTrueNorth.Value;
                }
                catch { }
            });
        }
        
        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    if (Map == null || UserLocation == null) return;
                    var r = args.Position.Coordinate.Point;
                    UserLocation.Location = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = r.Position.Latitude, Longitude = r.Position.Longitude }, AltitudeReferenceSystem.Terrain);// ;
                    if (ActiveNavigationMode == true)
                    {
                        Map.Center = args.Position.Coordinate.Point;
                        new DisplayRequest().RequestActive();
                    }
                });
            }
            catch { }
        }
    }
}
