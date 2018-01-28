using GoogleMapsUnofficial.ViewModel.DirectionsControls;
using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class WalkingUC : UserControl
    {
        public WalkingUC()
        {
            this.InitializeComponent();
            OriginTxt.DataContext = new ACSuggestionProviderVM();
            DestTxt.DataContext = new ACSuggestionProviderVM();
            OriginTxt.SetBinding(AutoSuggestBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SearchResults"), Mode = BindingMode.OneWay });
            DestTxt.SetBinding(AutoSuggestBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SearchResults"), Mode = BindingMode.OneWay });
            OriginTxt.TextChanged += OriginTxt_TextChanged;
            DestTxt.TextChanged += OriginTxt_TextChanged;
            OriginTxt.SuggestionChosen += OriginTxt_SuggestionChosen;
            DestTxt.SuggestionChosen += OriginTxt_SuggestionChosen;
            DirectionsMainUserControl.DestinationAddressChanged += DirectionsMainUserControl_DestinationAddressChanged;
            DirectionsMainUserControl.OriginAddressChanged += DirectionsMainUserControl_OriginAddressChanged;
        }

        private async void OriginTxt_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var pre = args.SelectedItem as PlaceAutoComplete.Prediction;
            if (pre == null) return;
            sender.Text = pre.description;

            if (pre.description == "MyLocation")
            {
                if (sender.Name == "OriginTxt")
                {
                    DirectionsMainUserControl.Origin = (await ViewModel.MapViewVM.GeoLocate.GetGeopositionAsync()).Coordinate.Point;
                }
                if (sender.Name == "DestTxt")
                {
                    DirectionsMainUserControl.Destination = (await ViewModel.MapViewVM.GeoLocate.GetGeopositionAsync()).Coordinate.Point;
                }
            }
            else if (pre.description.StartsWith("Saved:"))
            {
                var savedplaces = SavedPlacesVM.GetSavedPlaces();
                var res = savedplaces.Where(x => x.PlaceName == pre.description.Replace("Saved:", string.Empty)).FirstOrDefault();
                if (sender.Name == "OriginTxt")
                {
                    DirectionsMainUserControl.Origin = new Geopoint(new BasicGeoposition() { Latitude = res.Latitude, Longitude = res.Longitude });
                }
                if (sender.Name == "DestTxt")
                {
                    DirectionsMainUserControl.Destination = new Geopoint(new BasicGeoposition() { Latitude = res.Latitude, Longitude = res.Longitude });
                }
            }
            else
            {
                var res = await GeocodeHelper.GetInfo(pre.place_id);
                if (res == null) return;
                var ploc = res.results.FirstOrDefault().geometry.location;
                if (sender.Name == "OriginTxt")
                {
                    DirectionsMainUserControl.Origin = new Geopoint(new BasicGeoposition() { Latitude = ploc.lat, Longitude = ploc.lng });
                }
                if (sender.Name == "DestTxt")
                {
                    DirectionsMainUserControl.Destination = new Geopoint(new BasicGeoposition() { Latitude = ploc.lat, Longitude = ploc.lng });
                }
            }
        }

        private void OriginTxt_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
            (sender.DataContext as ACSuggestionProviderVM).SuggestForSearch(sender.Text);
        }

        private void DirectionsMainUserControl_OriginAddressChanged(object sender, string e)
        {
            OriginTxt.Text = e;
        }

        private void DirectionsMainUserControl_DestinationAddressChanged(object sender, string e)
        {
            DestTxt.Text = e;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                if (DirectionsMainUserControl.Origin != null && DirectionsMainUserControl.Destination != null)
                {
                    var Origin = DirectionsMainUserControl.Origin;
                    var Destination = DirectionsMainUserControl.Destination;
                    DirectionsHelper.Rootobject r = null;
                    if(DirectionsMainUserControl.WayPoints == null)
                        r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                    else
                    {
                        var lst = new List<BasicGeoposition>();
                        foreach (var item in DirectionsMainUserControl.WayPoints)
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
                    var route = DirectionsHelper.GetDirectionAsRoute(r.routes.FirstOrDefault(), Colors.Purple);
                    MapView.MapControl.MapElements.Add(route);
                    var es = DirectionsHelper.GetTotalEstimatedTime(r.routes.FirstOrDefault());
                    var di = DirectionsHelper.GetDistance(r.routes.FirstOrDefault());
                    await new MessageDialog($"we calculate that the route is about {di} and takes about {es}").ShowAsync();
                    MapView.MapControl.ZoomLevel = 18;
                    MapView.MapControl.Center = DirectionsMainUserControl.Origin;
                }
                else
                {
                    await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                }
            });
        }
        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
