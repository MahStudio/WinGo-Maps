using GoogleMapsUnofficial.ViewModel.DirectionsControls;
using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class DrivingUC : UserControl
    {
        public DrivingUC()
        {
            this.InitializeComponent();
            DirectionsMainUserControl.DestinationAddressChanged += DirectionsMainUserControl_DestinationAddressChanged;
            DirectionsMainUserControl.OriginAddressChanged += DirectionsMainUserControl_OriginAddressChanged;
        }

        private void DirectionsMainUserControl_OriginAddressChanged(object sender, string e)
        {
            OriginTxt.Text = e;
        }
        private void DirectionsMainUserControl_DestinationAddressChanged(object sender, string e)
        {
            DestTxt.Text = e;
        }

        ~DrivingUC()
        {
            DirectionsMainUserControl.DestinationAddressChanged -= DirectionsMainUserControl_DestinationAddressChanged;
            DirectionsMainUserControl.OriginAddressChanged -= DirectionsMainUserControl_OriginAddressChanged;
            OriginTxt.Text = "";
            DestTxt.Text = "";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                if (DirectionsMainUserControl.Origin != null && DirectionsMainUserControl.Destination != null)
                {
                    var Origin = DirectionsMainUserControl.Origin;
                    var Destination = DirectionsMainUserControl.Destination;
                    var r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.driving);
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
    }
}
