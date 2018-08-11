using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;

namespace GoogleMapsUnofficial.ViewModel.OnMapControls
{
    public enum MapMode
    {
        Standard,
        RoadsOnly,
        Satellite,
        Hybrid
    }
    class ChangeViewUCVM : INotifyPropertyChanged
    {
        private bool _showtraffic;
        private bool _isDefaultMapViewOn;
        private bool _isSatelliteMapViewOn;
        private bool _isHybridMapViewOn;
        private bool _isRoadsOnlyViewOn;
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }
        public bool ShowTraffic
        {
            get => _showtraffic; set
            {
                _showtraffic = value;
                if (value) UseGoogleMaps(CurrentMapMode, value, true, AllowOverstretch, FadeAnimationEnabled);
                Update("ShowTraffic");
            }
        }
        public bool IsDefaultMapViewOn
        {
            get => _isDefaultMapViewOn; set
            {
                _isDefaultMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Standard, value, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsDefaultMapViewOn");
            }
        }
        public bool IsSatelliteMapViewOn
        {
            get => _isSatelliteMapViewOn; set
            {
                _isSatelliteMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Satellite, value, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsSatelliteMapViewOn");
            }
        }
        public bool IsHybridMapViewOn
        {
            get => _isHybridMapViewOn; set
            {
                _isHybridMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Hybrid, value, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsHybridMapViewOn");
            }
        }
        public bool IsRoadsOnlyViewOn
        {
            get => _isRoadsOnlyViewOn; set
            {
                _isRoadsOnlyViewOn = value;
                if (value) UseGoogleMaps(MapMode.RoadsOnly, value, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsRoadsOnlyViewOn");
            }
        }


        public MapMode CurrentMapMode;
        public bool AllowOverstretch;
        public bool FadeAnimationEnabled;
        public MapControl Map { get; set; }
        public ChangeViewUCVM()
        {
            CurrentMapMode = MapMode.Standard;
            IsDefaultMapViewOn = true;
            ShowTraffic = SettingsSetters.GetShowTrafficOnLaunch();
            AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
            Map = MapView.MapControl;
        }
        public void UseGoogleMaps(MapMode MapMode = MapMode.Standard, bool showtraffic = false, bool AllowCaching = true, bool AllowOverstretch = false, bool IsFadingEnabled = true)
        {
            Map = MapView.MapControl;
            if (Map == null) return;
            Map.Style = MapStyle.None;
            CurrentMapMode = MapMode;
            if (!InternalHelper.InternetConnection()) return;
            Map.TileSources.Clear();
            string mapuri = "";
            switch (MapMode)
            {
                case MapMode.Standard:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=r&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=r@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    IsHybridMapViewOn = false;
                    IsSatelliteMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    break;
                case MapMode.RoadsOnly:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=h&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=h@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    IsHybridMapViewOn = false;
                    IsSatelliteMapViewOn = false;
                    IsDefaultMapViewOn = false;
                    break;
                case MapMode.Satellite:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=s&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=s@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    IsHybridMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    IsDefaultMapViewOn = false;
                    break;
                case MapMode.Hybrid:
                    if (!ShowTraffic)
                        mapuri = "http://mt1.google.com/vt/lyrs=y&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    else
                        mapuri = "http://mt1.google.com/vt/lyrs=y@221097413,traffic&hl=x-local&z={zoomlevel}&x={x}&y={y}";
                    IsSatelliteMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    IsDefaultMapViewOn = false;
                    break;
                default:
                    break;
            }
            Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource(mapuri)
            { AllowCaching = AllowCaching })
            { AllowOverstretch = AllowOverstretch, IsFadingEnabled = IsFadingEnabled });
        }
    }
}
