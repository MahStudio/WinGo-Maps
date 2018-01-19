using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class ChangeViewUserControl : UserControl
    {
        private MapControl Map;
        public ChangeViewUserControl()
        {
            this.InitializeComponent();
        }

        private void Flyout_Opened(object sender, object e)
        {
            if(Map == null)
            {
                Map = MapView.MapControl;
                DefaultMapView.IsChecked = true;
                ShowTraffic.IsOn = true;
            }
        }

        private void DefaultMapView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = true;
            SatelliteMapView.IsChecked = false;
        }

        private void SatelliteMapView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = false;
            SatelliteMapView.IsChecked = true;
        }
        
        private void DefaultMapView_Checked(object sender, RoutedEventArgs e)
        {
            if(ShowTraffic.IsOn)
            {
                Map.TileSources.Clear();
                var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                dataSource.AllowCaching = true;
                Map.TileSources.Add(new MapTileSource(dataSource));
            }
            else
            {
                Map.TileSources.Clear();
                var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                dataSource.AllowCaching = true;
                Map.TileSources.Add(new MapTileSource(dataSource));
            }
        }

        private void SatelliteMapView_Checked(object sender, RoutedEventArgs e)
        {
            if (ShowTraffic.IsOn)
            {
                Map.TileSources.Clear();
                var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=s@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                dataSource.AllowCaching = true;
                Map.TileSources.Add(new MapTileSource(dataSource));
            }
            else
            {
                Map.TileSources.Clear();
                var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=s&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                dataSource.AllowCaching = true;
                Map.TileSources.Add(new MapTileSource(dataSource));
            }
        }
        
        private void ShowTraffic_Toggled(object sender, RoutedEventArgs e)
        {
            if (DefaultMapView.IsChecked.Value)
            {
                if (ShowTraffic.IsOn)
                {
                    Map.TileSources.Clear();
                    var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                    dataSource.AllowCaching = true;
                    Map.TileSources.Add(new MapTileSource(dataSource));
                }
                else
                {
                    Map.TileSources.Clear();
                    var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                    dataSource.AllowCaching = true;
                    Map.TileSources.Add(new MapTileSource(dataSource));
                }
            }
            else if (SatelliteMapView.IsChecked.Value)
            {
                if (ShowTraffic.IsOn)
                {
                    Map.TileSources.Clear();
                    var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=s@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                    dataSource.AllowCaching = true;
                    Map.TileSources.Add(new MapTileSource(dataSource));
                }
                else
                {
                    Map.TileSources.Clear();
                    var dataSource = new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=s&hl=x-local&z={zoomlevel}&x={x}&y={y}");
                    dataSource.AllowCaching = true;
                    Map.TileSources.Add(new MapTileSource(dataSource));
                }
            }
        }
    }
}
