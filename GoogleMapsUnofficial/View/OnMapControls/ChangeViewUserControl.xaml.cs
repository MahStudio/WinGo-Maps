using GoogleMapsUnofficial.ViewModel.SettingsView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class ChangeViewUserControl : UserControl
    {
        private MapControl Map;
        public bool AllowOverstretch;
        public bool FadeAnimationEnabled;
        public bool ShowTrafficIsOn;
        public enum MapMode
        {
            Standard,
            RoadsOnly,
            Satellite,
            Hybrid
        }
        public MapMode CurrentMapMode;
        public ChangeViewUserControl()
        {
            CurrentMapMode = MapMode.Standard;
            ShowTrafficIsOn = false;
            AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
            this.InitializeComponent();
            if (!InternalHelper.InternetConnection())
                CVBTN.IsEnabled = false;
            Map = MapView.MapControl;
        }
        public void UseGoogleMaps(MapMode MapMode = MapMode.Standard, bool ShowTraffic = false, bool AllowCaching = true, bool AllowOverstretch = false, bool IsFadingEnabled = true)
        {
            if (Map == null && MapView.MapControl != null) Map = MapView.MapControl;
            if (Map == null) return;
            Map.Style = MapStyle.None;
            Map.TileSources.Clear();
            CurrentMapMode = MapMode;
            ShowTrafficIsOn = ShowTraffic;
            string mapuri = "";
            switch (MapMode)
            {
                case MapMode.Standard:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=r&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    break;
                case MapMode.RoadsOnly:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=h&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=h@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    break;
                case MapMode.Satellite:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=s&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=s@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    break;
                case MapMode.Hybrid:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=y&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=y@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    break;
                default:
                    break;
            }
            Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource(mapuri)
            { AllowCaching = AllowCaching })
            { AllowOverstretch = AllowOverstretch, IsFadingEnabled = IsFadingEnabled });
        }

        private void Flyout_Opened(object sender, object e)
        {
            if (Map == null)
            {
                Map = MapView.MapControl;
                DefaultMapView.IsChecked = true;
                ShowTraffic.IsOn = false;
            }
        }

        private void DefaultMapView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = true;
            SatelliteMapView.IsChecked = false;
            HybridMapView.IsChecked = false;
            RoadsOnlyView.IsChecked = false;
        }

        private void SatelliteMapView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = false;
            SatelliteMapView.IsChecked = true;
            HybridMapView.IsChecked = false;
            RoadsOnlyView.IsChecked = false;
        }

        private void HybridMapView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = false;
            SatelliteMapView.IsChecked = false;
            HybridMapView.IsChecked = true;
            RoadsOnlyView.IsChecked = false;
        }

        private void RoadsOnlyView_Click(object sender, RoutedEventArgs e)
        {
            DefaultMapView.IsChecked = false;
            SatelliteMapView.IsChecked = false;
            HybridMapView.IsChecked = false;
            RoadsOnlyView.IsChecked = true;
        }

        private void DefaultMapView_Checked(object sender, RoutedEventArgs e)
        {
            UseGoogleMaps(MapMode.Standard, ShowTraffic.IsOn, true, AllowOverstretch, FadeAnimationEnabled);
        }

        private void SatelliteMapView_Checked(object sender, RoutedEventArgs e)
        {
            UseGoogleMaps(MapMode.Satellite, ShowTraffic.IsOn, true, AllowOverstretch, FadeAnimationEnabled);
        }

        private void ShowTraffic_Toggled(object sender, RoutedEventArgs e)
        {
            UseGoogleMaps(CurrentMapMode, ShowTraffic.IsOn, true, AllowOverstretch, FadeAnimationEnabled);
        }

        private void HybridMapView_Checked(object sender, RoutedEventArgs e)
        {
            UseGoogleMaps(MapMode.Hybrid, ShowTraffic.IsOn, true, AllowOverstretch, FadeAnimationEnabled);
        }

        private void RoadsOnlyView_Checked(object sender, RoutedEventArgs e)
        {
            UseGoogleMaps(MapMode.RoadsOnly, ShowTraffic.IsOn, true, AllowOverstretch, FadeAnimationEnabled);
        }
    }
}
