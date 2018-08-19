using GMapsUWP.Directions;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using WinGoMapsX.Helpers;
using WinGoMapsX.ViewModel.SettingsView;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinGoMapsX.View.DirectionsControls
{
    public sealed partial class NewDirections : UserControl
    {
        public Geopoint Origin { get; set; }
        public Geopoint Destination { get; set; }
        public List<BasicGeoposition> Waypoints { get; set; }

        DirectionsHelper.DirectionModes Mode = DirectionsHelper.DirectionModes.walking;

        public MapControl Map { get; set; }
        public StepsTitleProvider StepsTitleProvider { get; set; }
        public NewDirections()
        {
            this.InitializeComponent();
            this.Loaded += NewDirections_Loaded;
            Waypoints = new List<BasicGeoposition>();
        }

        private void NewDirections_Loaded(object sender, RoutedEventArgs e)
        {
            var c = (Color)Resources["SystemControlBackgroundAccentBrush"];
            WalkBTN.Foreground = new SolidColorBrush(c);
            Mode = DirectionsHelper.DirectionModes.walking;
        }

        private async void NavMode_Click(object sender, RoutedEventArgs e)
        {
            #region Setting background / foreground / mode
            SolidColorBrush bg;
            if (Mode != DirectionsHelper.DirectionModes.walking)
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
                    Mode = DirectionsHelper.DirectionModes.walking;
                    break;
                case "DriveBTN":
                    Mode = DirectionsHelper.DirectionModes.driving;
                    break;
                case "TransitBTN":
                    Mode = DirectionsHelper.DirectionModes.transit;
                    break;
                default:
                    break;
            }
            #endregion

            if (Destination == null || Origin == null) return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, DirectionFinder);
        }
        public async void DirectionFinder()
        {
            MapPolyline CurrentDrawed = null;
            try
            {
                foreach (var item in Map.MapElements)
                {
                    if (item.GetType() == typeof(MapPolyline))
                        CurrentDrawed = (MapPolyline)item;
                }
            }
            catch { }

            if (Destination == null) return;
            await VoiceNavigationHelper.ReadText("calculating route");
            var r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, Mode, Waypoints);

            if (r == null) { await new MessageDialog(MultilingualHelpToolkit.GetString("StringNoWayToDestination", "Text")).ShowAsync(); return; }
            if (r.Status == "OVER_QUERY_LIMIT") { await new MessageDialog("OVER_QUERY_LIMIT").ShowAsync(); return; }

            if (CurrentDrawed != null)
                Map.MapElements.Remove(CurrentDrawed);
            
            var polyline = DirectionsHelper.GetDirectionAsRoute(r.Routes.FirstOrDefault(), (Color)Resources["SystemControlBackgroundAccentBrush"]);
            Map.MapElements.Add(polyline);

            new VoiceNavigationHelper(r.Routes.FirstOrDefault());
            StepsTitleProvider.SetRoute(r.Routes.FirstOrDefault());

            var distance = GetDistance(r.Routes.FirstOrDefault());
            var estime = DirectionsHelper.GetTotalEstimatedTime(r.Routes.FirstOrDefault());
            await new MessageDialog($"distance : {distance} estimated time : {estime}").ShowAsync();
        }

        public static string GetDistance(DirectionsHelper.Route Route)
        {
            var Distance = 0;
            foreach (var item in Route.Legs)
            {
                Distance += item.Distance.Value;
            }
            if (SettingsSetters.GetLengthUnit() == 0)
            {
                //Metric
                if (Distance <= 2000)
                    return $"{Distance} {MultilingualHelpToolkit.GetString("StringMeters", "Text")}";
                else return $"{Distance / 1000f} {MultilingualHelpToolkit.GetString("StringKiloMeters", "Text")}";
            }
            else if (SettingsSetters.GetLengthUnit() == 1)
            {
                //Imperial
                if (Distance <= 2000)
                    return $"{string.Format("{0:0.00}", Distance * 1.093613f)} {MultilingualHelpToolkit.GetString("StringYards", "Text")}";
                else return $"{string.Format("{0:0.00}", Distance * 0.000621371f)} {MultilingualHelpToolkit.GetString("StringMiles", "Text")}";
            }
            else
            {
                //US 
                if (Distance <= 2000)
                    return $"{string.Format("{0:0.00}", Distance * 3.28084f)} {MultilingualHelpToolkit.GetString("StringFeet", "Text")}";
                else return $"{string.Format("{0:0.00}", Distance * 0.000621371f)} {MultilingualHelpToolkit.GetString("StringMiles", "Text")}";
            }
        }
    }
}
