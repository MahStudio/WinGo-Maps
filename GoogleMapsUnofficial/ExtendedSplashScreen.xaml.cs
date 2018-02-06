using GoogleMapsUnofficial.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            DispatcherTime = new DispatcherTimer();
            DispatcherTime.Interval = new TimeSpan(0, 0, 10);
            DispatcherTime.Tick += DispatcherTime_Tick;
            DispatcherTime.Start();
        }

        private void DispatcherTime_Tick(object sender, object e)
        {
            RemoveExtendedSplash();
        }

        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                try
                {
                    var crashrep = ApplicationData.Current.LocalSettings.Values["CrashDump"].ToString();
                    var file = await KnownFolders.PicturesLibrary.CreateFileAsync("WinGoMapsCrash.txt", CreationCollisionOption.GenerateUniqueName);
                    await FileIO.WriteTextAsync(file, crashrep);
                    ApplicationData.Current.LocalSettings.Values["CrashDump"] = null;
                    await new MessageDialog("We detected a crash on your last session. It has been saved in" + file.Path).ShowAsync();
                }
                catch
                {

                }
                var geolocator = MapViewVM.GeoLocate;
                var accessStatus = await Geolocator.RequestAccessAsync();

                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    geolocator = new Geolocator();
                    geolocator.ReportInterval = 5000;
                    geolocator.DesiredAccuracyInMeters = 200;
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    MapViewVM.FastLoadGeoPosition = pos;
                    MapViewVM.GeoLocate = geolocator;
                }
                RemoveExtendedSplash();
            });
        }

        async void RemoveExtendedSplash()
        {
            DispatcherTime.Stop();
            DispatcherTime.Tick -= DispatcherTime_Tick;
            DispatcherTime = null;
            await Task.Delay(500);
            Window.Current.Content = new MainPage(para);
            Window.Current.Activate();
        }
    }
}
