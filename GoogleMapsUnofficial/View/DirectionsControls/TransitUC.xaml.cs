using GoogleMapsUnofficial.ViewModel.DirectionsControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class TransitUC : UserControl
    {
        public TransitUC()
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {

                if (DirectionsMainUserControl.Origin != null && DirectionsMainUserControl.Destination != null)
                {
                    var Origin = DirectionsMainUserControl.Origin;
                    var Destination = DirectionsMainUserControl.Destination;
                    var r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.transit);
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
                }
                else
                {
                    await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
                }
            });
        }
    }
}
