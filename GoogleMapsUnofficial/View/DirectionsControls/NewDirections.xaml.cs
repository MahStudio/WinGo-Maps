using GoogleMapsUnofficial.ViewModel;
using GoogleMapsUnofficial.ViewModel.DirectionsControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class NewDirections : UserControl
    {
        public Geopoint Origin { get; set; }
        public Geopoint Destination { get; set; }
        public List<Geopoint> Waypoints { get; set; }
        enum DirectionMode
        {
            walking,
            driving,
            transit
        }
        DirectionMode Mode = DirectionMode.walking;
        public NewDirections()
        {
            this.InitializeComponent();
            this.Loaded += NewDirections_Loaded;
            Waypoints = new List<Geopoint>();
        }

        private void NewDirections_Loaded(object sender, RoutedEventArgs e)
        {
            var c = (Color)Resources["SystemControlBackgroundAccentBrush"];
            WalkBTN.Foreground = new SolidColorBrush(c);
        }

        private void NavMode_Click(object sender, RoutedEventArgs e)
        {
            #region Setting background / foreground / mode
            SolidColorBrush bg;
            if (Mode != DirectionMode.walking)
            {
                bg = WalkBTN.Foreground as SolidColorBrush;
            }
            else
            {
                bg = DriveBTN.Foreground as SolidColorBrush;
            }
            WalkBTN.Foreground = bg;
            DriveBTN.Foreground = bg;
            TransitBTN.Foreground = bg;
            var c = (Color)Resources["SystemControlBackgroundAccentBrush"];
            (sender as Button).Foreground = new SolidColorBrush(c);
            switch ((sender as Button).Name)
            {
                case "WalkBTN":
                    Mode = DirectionMode.walking;
                    break;
                case "DriveBTN":
                    Mode = DirectionMode.driving;
                    break;
                case "TransitBTN":
                    Mode = DirectionMode.transit;
                    break;
                default:
                    break;
            }
            #endregion

            foreach (var item in MapView.MapControl.MapElements)
            {
                if (item.GetType() == typeof(MapPolyline))
                {
                    MapView.MapControl.MapElements.Remove(item);
                    DirectionFinder();
                    return;
                }
            }
        }

        public async void DirectionFinder()
        {
            MapPolyline CurrentDrawed = null;
            try
            {
                foreach (var item in MapView.MapControl.MapElements)
                {
                    if (item.GetType() == typeof(MapPolyline))
                        CurrentDrawed = (MapPolyline)item;
                }
            }
            catch { }
            if (Mode == DirectionMode.walking)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        if (Waypoints == null)
                            r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                        else
                        {
                            var lst = new List<BasicGeoposition>();
                            foreach (var item in Waypoints)
                            {
                                if (item != null)
                                    lst.Add(new BasicGeoposition() { Latitude = item.Position.Latitude, Longitude = item.Position.Longitude });
                            }
                            if (lst.Count > 0)
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking, lst);
                            else
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                        }
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                            return;
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                         var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
            else if (Mode == DirectionMode.driving)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        if (Waypoints == null)
                            r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving);
                        else
                        {
                            var lst = new List<BasicGeoposition>();
                            foreach (var item in Waypoints)
                            {
                                if (item != null)
                                    lst.Add(new BasicGeoposition() { Latitude = item.Position.Latitude, Longitude = item.Position.Longitude });
                            }
                            if (lst.Count > 0)
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving, lst);
                            else
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving);
                        }
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                            return;
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                        var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.transit);
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                            return;
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                        var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
        }


        public async Task DirectionFinderAsync()
        {
            MapPolyline CurrentDrawed = null;
            try
            {
                foreach (var item in MapView.MapControl.MapElements)
                {
                    if (item.GetType() == typeof(MapPolyline))
                        CurrentDrawed = (MapPolyline)item;
                }
            }
            catch { }
            if (Mode == DirectionMode.walking)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        if (Waypoints == null)
                            r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                        else
                        {
                            var lst = new List<BasicGeoposition>();
                            foreach (var item in Waypoints)
                            {
                                if (item != null)
                                    lst.Add(new BasicGeoposition() { Latitude = item.Position.Latitude, Longitude = item.Position.Longitude });
                            }
                            if (lst.Count > 0)
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking, lst);
                            else
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                        }
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                        var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
            else if (Mode == DirectionMode.driving)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        if (Waypoints == null)
                            r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving);
                        else
                        {
                            var lst = new List<BasicGeoposition>();
                            foreach (var item in Waypoints)
                            {
                                if (item != null)
                                    lst.Add(new BasicGeoposition() { Latitude = item.Position.Latitude, Longitude = item.Position.Longitude });
                            }
                            if (lst.Count > 0)
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving, lst);
                            else
                                r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving);
                        }
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                            return;
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                        var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    if (Origin != null && Destination != null)
                    {
                        DirectionsHelper.Rootobject r = null;
                        r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.transit);
                        if (r == null || r.routes.Count() == 0)
                        {
                            await new MessageDialog("No way to your destination!!!").ShowAsync();
                            return;
                        }
                        MapView.MapControl.MapElements.Remove(CurrentDrawed);
                        var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
                        MapView.MapControl.MapElements.Add(route);
                        var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                        var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                        await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                        MapView.MapControl.ZoomLevel = 18;
                        MapView.MapControl.Center = Origin;
                        MapView.MapControl.DesiredPitch = 45;
                        MapViewVM.ActiveNavigationMode = true;
                        new DisplayRequest().RequestActive();
                    }
                    else
                    {
                        await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                    }
                });
            }
        }
    }
}
