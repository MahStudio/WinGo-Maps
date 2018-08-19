using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinGoMapsX.View.OnMapControls
{
    public sealed partial class ZoomUserControl : UserControl
    {
        private MapControl _map { get; set; }
        public MapControl Map { get => _map; set { _map = value; Map.HeadingChanged += Map_HeadingChanged; } }
        
        public ZoomUserControl() => this.InitializeComponent();

        private async void ZoomOut_Click(object sender, RoutedEventArgs e) => await Map.TryZoomOutAsync();

        private async void ZoomIn_Click(object sender, RoutedEventArgs e)=> await Map.TryZoomInAsync();

        private async void North_Click(object sender, RoutedEventArgs e)=> await Map.TryRotateToAsync(0);

        private void Map_HeadingChanged(MapControl sender, object args)
        {
            if (Map.Heading != 0) North.IsEnabled = true;
            else North.IsEnabled = false;
        }
    }
}
