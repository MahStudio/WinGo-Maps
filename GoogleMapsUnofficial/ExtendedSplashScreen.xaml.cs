using GoogleMapsUnofficial.ViewModel;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        DispatcherTimer DispatcherTime;
        object para = null;
        public ExtendedSplashScreen(SplashScreen splash, object parameter = null)
        {
            this.InitializeComponent();
            this.Loaded += ExtendedSplashScreen_Loaded;
            para = parameter;
            DispatcherTime = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 20)
            };
            DispatcherTime.Tick += DispatcherTime_Tick;
            DispatcherTime.Start();
        }

        private void ApplyAcrylicAccent(Grid Grid1)
        {
            if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone) return;
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
            {
                var ac = new AcrylicBrush();
                var brush = Resources["SystemControlAccentAcrylicWindowAccentMediumHighBrush"] as AcrylicBrush;
                //brush.TintColor = (Color)Resources["SystemControlBackgroundAccentBrush"];
                ac = brush;
                //ac.TintOpacity = 0.7;
                //ac.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                Grid1.Background = ac;
                return;
            }
            if (ApiInformation.IsMethodPresent("Windows.UI.Xaml.Hosting.ElementCompositionPreview", "SetElementChildVisual"))
            {
                _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2((float)Grid1.ActualWidth, (float)Grid1.ActualHeight);

                ElementCompositionPreview.SetElementChildVisual(Grid1, _hostSprite);
                var b = _compositor.CreateHostBackdropBrush();
                _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
            }
        }

        Compositor _compositor;
        SpriteVisual _hostSprite;
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hostSprite != null)
                _hostSprite.Size = e.NewSize.ToVector2();
        }

        private async void DispatcherTime_Tick(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                //try
                //{
                //    if (Geolocator.DefaultGeoposition.HasValue && Geolocator.IsDefaultGeopositionRecommended)
                //    {
                //        MapViewVM.FastLoadGeoPosition = new Geopoint(Geolocator.DefaultGeoposition.Value);
                //    }
                //}
                //catch { }
                RemoveExtendedSplash();
            });
        }
        public async void InstallVCD()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["VCDV10"].ToString();
            }
            catch (Exception)
            {
                var vcdStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommands.xml", UriKind.RelativeOrAbsolute));
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                ApplicationData.Current.LocalSettings.Values["VCDV10"] = "";
            }
        }
        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                ApplyAcrylicAccent(Grid1);
                InstallVCD();
                try
                {
                    var res = ApplicationData.Current.LocalSettings.Values["LastUserLocation"].ToString();
                    var latitude = res.Split(',')[0];
                    var longitude = res.Split(',')[1];
                    if (MapViewVM.UserLocation == null) MapViewVM.UserLocation = new ViewModel.ViewModel() { AttractionName = "My Location" };
                    MapViewVM.UserLocation.Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(latitude), Longitude = Convert.ToDouble(longitude) });
                    MapViewVM.FastLoadGeoPosition = MapViewVM.UserLocation.Location;
                    RemoveExtendedSplash();
                    return;
                }
                catch { }
                //try
                //{
                //    var crashrep = ApplicationData.Current.LocalSettings.Values["CrashDump"].ToString();
                //    var file = await KnownFolders.PicturesLibrary.CreateFileAsync("WinGoMapsCrash.txt", CreationCollisionOption.GenerateUniqueName);
                //    await FileIO.WriteTextAsync(file, crashrep);
                //    ApplicationData.Current.LocalSettings.Values["CrashDump"] = null;
                //    await new MessageDialog("We detected a crash on your last session. It has been saved in" + file.Path).ShowAsync();
                //}
                //catch
                //{

                //}
                var geolocator = MapViewVM.GeoLocate;
                var accessStatus = await Geolocator.RequestAccessAsync();

                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    geolocator = new Geolocator
                    {
                        MovementThreshold = 1,
                        ReportInterval = 1,
                        DesiredAccuracyInMeters = 1
                    };
                    MapViewVM.GeoLocate = geolocator;
                    GeoLocatorHelper.GetUserLocation();
                    GeoLocatorHelper.LocationFetched += GeoLocatorHelper_LocationFetched;
                }
                RemoveExtendedSplash();
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            GeoLocatorHelper.LocationFetched -= GeoLocatorHelper_LocationFetched;
        }

        private void GeoLocatorHelper_LocationFetched(object sender, Geoposition e)
        {
            MapViewVM.FastLoadGeoPosition = e.Coordinate.Point;
        }

        async void RemoveExtendedSplash()
        {
            try
            {
                DispatcherTime.Stop();
                DispatcherTime.Tick -= DispatcherTime_Tick;
                DispatcherTime = null;
                await Task.Delay(500);
                Window.Current.Content = new MainPage(para);
                Window.Current.Activate();
            }
            catch { }
        }
    }
}
