using GMapsUWP;
using GMapsUWP.Directions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinGoMapsX.View.DirectionsControls
{
    public sealed partial class StepsTitleProvider : UserControl
    {
        DirectionsHelper.Route CurrentRoute { get; set; }
        DirectionsHelper.Leg CurrentLeg { get; set; }
        DirectionsHelper.Step CurrentStep { get; set; }
        public bool IsBusy { get; set; }
        public MapView MapView { get; set; }

        public StepsTitleProvider()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        public void SetRoute(DirectionsHelper.Route Route)
        {
            GeoLocatorHelper.LocationChanged += GeoLocatorHelper_LocationChanged;
            CurrentRoute = Route;
            CurrentLeg = Route.Legs.FirstOrDefault();
            CurrentStep = CurrentLeg.Steps.FirstOrDefault();
            this.Visibility = Visibility.Visible;
            UpdateStep();
            FullList_Click(null, null);
        }

        private async void GeoLocatorHelper_LocationChanged(object sender, Geocoordinate e)
        {
            if (CurrentRoute == null || IsBusy) return;
            IsBusy = true;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                var cp = e.Point.Position;
                var ds = DistanceTo(CurrentStep.EndLocation.Latitude, CurrentStep.EndLocation.Longitude, cp.Latitude, cp.Longitude, 'K');
                if (ds < 0.010)
                {
                    //Next Step/Leg
                    var slist = CurrentLeg.Steps.ToList();
                    var sind = slist.FindIndex(x => x == CurrentStep);
                    if (sind < (slist.Count - 1))
                        CurrentStep = slist[(++sind)];
                    else
                    {
                        var llist = CurrentRoute.Legs.ToList();
                        var lind = llist.FindIndex(x => x == CurrentLeg);
                        if (lind < (llist.Count - 1))
                            CurrentLeg = llist[(++lind)];
                        else this.Visibility = Visibility.Visible;//end of navigation
                    }
                    CurrentLeg.Steps.Where(x => x == CurrentStep);
                }
                IsBusy = false;
            });
        }

        async void UpdateStep()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                ComandInstructions.Text = CurrentStep.HtmlInstructions.NoHTMLString();
                ComandImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/DirectionsIcons/" + CurrentStep.Maneuver + ".png", UriKind.RelativeOrAbsolute));
            });
        }

        double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }
            return dist;
        }

        private void FullList_Click(object sender, RoutedEventArgs e)
        {
            (MapView.FindName("RightPaneGrid") as Grid).Children.Clear();
            (MapView.FindName("RightPaneGrid") as Grid).Children.Add(new FullStepsProvider(CurrentRoute) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch });
            (MapView.DataContext as MapViewVM).RightPaneOpen = true;
        }
    }
}
