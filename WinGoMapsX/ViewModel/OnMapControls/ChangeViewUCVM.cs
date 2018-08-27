using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using WinGoMapsX.ViewModel.SettingsView;

namespace WinGoMapsX.ViewModel.OnMapControls
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
                UseGoogleMaps(CurrentMapMode, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
                Update("ShowTraffic");
            }
        }
        public bool IsDefaultMapViewOn
        {
            get => _isDefaultMapViewOn; set
            {
                _isDefaultMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Standard, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsDefaultMapViewOn");
            }
        }
        public bool IsSatelliteMapViewOn
        {
            get => _isSatelliteMapViewOn; set
            {
                _isSatelliteMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Satellite, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsSatelliteMapViewOn");
            }
        }
        public bool IsHybridMapViewOn
        {
            get => _isHybridMapViewOn; set
            {
                _isHybridMapViewOn = value;
                if (value) UseGoogleMaps(MapMode.Hybrid, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsHybridMapViewOn");
            }
        }
        public bool IsRoadsOnlyViewOn
        {
            get => _isRoadsOnlyViewOn; set
            {
                _isRoadsOnlyViewOn = value;
                if (value) UseGoogleMaps(MapMode.RoadsOnly, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
                Update("IsRoadsOnlyViewOn");
            }
        }


        public MapMode CurrentMapMode;
        public bool AllowOverstretch { get => SettingsSetters.GetAllowOverstretch(); }
        public bool FadeAnimationEnabled { get => SettingsSetters.GetFadeAnimationEnabled(); }
        private MapControl _map { get; set; }
        public MapControl Map
        {
            get => _map; set
            {
                _map = value;
                UseGoogleMaps(CurrentMapMode, ShowTraffic, true, AllowOverstretch, FadeAnimationEnabled);
            }
        }
        public ChangeViewUCVM()
        {
            if (App.Current.RequestedTheme == ApplicationTheme.Light)
                CurrentMapMode = MapMode.Standard;
            else CurrentMapMode = MapMode.RoadsOnly;
            IsDefaultMapViewOn = true;
            ShowTraffic = SettingsSetters.GetShowTrafficOnLaunch();
        }

        public void UseGoogleMaps(MapMode MapMode, bool showtraffic, bool AllowCaching, bool AllowOverstretch, bool IsFadingEnabled)
        {
            if (Map == null) return;
            GMapsUWP.Map.MapControlHelper.MapMode md = GMapsUWP.Map.MapControlHelper.MapMode.Standard;
            switch (MapMode)
            {
                case MapMode.Standard:
                    IsHybridMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    IsSatelliteMapViewOn = false;
                    md = GMapsUWP.Map.MapControlHelper.MapMode.Standard;
                    CurrentMapMode = MapMode.Standard;
                    break;
                case MapMode.RoadsOnly:
                    IsHybridMapViewOn = false;
                    IsDefaultMapViewOn = false;
                    IsSatelliteMapViewOn = false;
                    md = GMapsUWP.Map.MapControlHelper.MapMode.RoadsOnly;
                    CurrentMapMode = MapMode.RoadsOnly;
                    break;
                case MapMode.Satellite:
                    IsHybridMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    IsDefaultMapViewOn = false;
                    md = GMapsUWP.Map.MapControlHelper.MapMode.Satellite;
                    CurrentMapMode = MapMode.Satellite;
                    break;
                case MapMode.Hybrid:
                    IsDefaultMapViewOn = false;
                    IsRoadsOnlyViewOn = false;
                    IsSatelliteMapViewOn = false;
                    md = GMapsUWP.Map.MapControlHelper.MapMode.Hybrid;
                    CurrentMapMode = MapMode.Hybrid;
                    break;
                default:
                    break;
            }
            GMapsUWP.Map.MapControlHelper.UseGoogleMaps(Map, md, ShowTraffic, AllowCaching, AllowOverstretch, IsFadingEnabled);
        }
    }
}

