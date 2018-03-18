using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class NewZoomControl : UserControl
    {
        public NewZoomControl()
        {
            this.InitializeComponent();
        }

        private async void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            await MapView.MapControl.TryZoomOutAsync();
        }

        private async void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            await MapView.MapControl.TryZoomInAsync();
        }

        private async void ZoomIn_Holding(object sender, HoldingRoutedEventArgs e)
        {
            await MapView.MapControl.TryZoomInAsync();
        }

        private async void ZoomOut_Holding(object sender, HoldingRoutedEventArgs e)
        {
            await MapView.MapControl.TryZoomInAsync();
        }

        private void ZoomControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //var storyboard = ZoomControl.Resources["ZoomInControl"] as Storyboard;
            ZoomInControl.Begin();
        }

        private void ZoomControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //var storyboard = ZoomControl.Resources["ZoomOutControl"] as Storyboard;
            ZoomOutControl.Begin();
        }

        private void ZoomControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //var storyboard = ZoomControl.Resources["ZoomInControl"] as Storyboard;
            ZoomInControl.Begin();
        }
    }
}
