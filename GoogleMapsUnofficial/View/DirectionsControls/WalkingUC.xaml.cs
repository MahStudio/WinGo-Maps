using GoogleMapsUnofficial.ViewModel.DirectionsControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class WalkingUC : UserControl
    {
        public WalkingUC()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DirectionsMainUserControl.Origin != null && DirectionsMainUserControl.Destination != null)
            {
                var Origin = DirectionsMainUserControl.Origin;
                var Destination = DirectionsMainUserControl.Destination;
                var r = await DirectionsHelper.GetDirections(Origin.Position, Destination.Position, DirectionsHelper.DirectionModes.walking);
                if (r == null || r.routes.Count() == 0)
                {
                    await new MessageDialog("No way to your destination!!!").ShowAsync();
                    return;
                }
                var route = ViewModel.DirectionsControls.DirectionsHelper.GetDirectionAsRoute(r);
                MapView.MapControl.MapElements.Add(route);
            }
            else
            {
                await new MessageDialog("You didn't select both origin and destination points").ShowAsync();
            }
        }
    }
}
