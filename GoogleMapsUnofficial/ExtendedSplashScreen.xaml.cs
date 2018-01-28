﻿using GoogleMapsUnofficial.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
        public ExtendedSplashScreen(SplashScreen splash)
        {
            this.InitializeComponent();
            this.Loaded += ExtendedSplashScreen_Loaded;

        }

        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
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
            await Task.Delay(2000);
            Window.Current.Content = new MainPage();
            Window.Current.Activate();
        }
    }
}
