using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;

namespace GoogleMapsUnofficial.ViewModel.SettingsView
{
    class SettingsMainVM : INotifyPropertyChanged
    {
        private int _rotationcontrolsVisible;
        private int _zoomcontrolsVisible;
        private bool _fadeanimationEnabled;
        private bool _allowOverstretch;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool AllowOverstretch
        {
            get { return _allowOverstretch; }
            set
            {
                _allowOverstretch = value;
                SettingsSetters.SetAllowOverstretch(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowOverstretch"));
            }
        }
        public bool FadeAnimationEnabled
        {
            get { return _fadeanimationEnabled; }
            set
            {
                _fadeanimationEnabled = value;
                SettingsSetters.SetFadeAnimationEnabled(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FadeAnimationEnabled"));
            }
        }
        public int ZoomControlsVisible
        {
            get { return _zoomcontrolsVisible; }
            set
            {
                _zoomcontrolsVisible = value;
                SettingsSetters.SetZoomControlsVisible((MapInteractionMode)value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomControlsVisible"));
            }
        }
        public int RotationControlsVisible
        {
            get { return _rotationcontrolsVisible; }
            set
            {
                _rotationcontrolsVisible = value;
                SettingsSetters.SetRotationControlsVisible((MapInteractionMode)value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RotationControlsVisible"));
            }
        }
        public List<string> MapInteractionModeOptions
        {
            get { return Enum.GetNames(typeof(MapInteractionMode)).ToList(); }
        }
        public string ApplicationVersion
        {
            get { var ver = Package.Current.Id.Version; return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}"; }
        }
        public SettingsMainVM()
        {
            AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
            ZoomControlsVisible = (int)SettingsSetters.GetZoomControlsVisible();
            RotationControlsVisible = (int)SettingsSetters.GetRotationControlsVisible();
        }
    }

    class SettingsSetters
    {
        public static bool GetFadeAnimationEnabled()
        {
            try
            {
                return (bool)ApplicationData.Current.LocalSettings.Values["FadeAnimationEnabled"];
            }
            catch
            {
                SetFadeAnimationEnabled(true);
                return true;
            }
        }

        public static void SetFadeAnimationEnabled(bool Value)
        {
            ApplicationData.Current.LocalSettings.Values["FadeAnimationEnabled"] = Value;
        }

        public static bool GetAllowOverstretch()
        {
            try
            {
                return (bool)ApplicationData.Current.LocalSettings.Values["AllowOverstretch"];
            }
            catch
            {
                SetAllowOverstretch(true);
                return true;
            }
        }

        public static void SetAllowOverstretch(bool Value)
        {
            ApplicationData.Current.LocalSettings.Values["AllowOverstretch"] = Value;
        }

        public static MapInteractionMode GetZoomControlsVisible()
        {
            try
            {
                return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible"];
            }
            catch
            {
                SetZoomControlsVisible(MapInteractionMode.GestureAndControl);
                return MapInteractionMode.GestureAndControl;
            }
        }

        public static void SetZoomControlsVisible(MapInteractionMode Value)
        {
            ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible"] = (int)Value;
        }

        public static MapInteractionMode GetRotationControlsVisible()
        {
            try
            {
                return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["RotationControlsVisible"];
            }
            catch
            {
                if(ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone)
                {
                    SetRotationControlsVisible(MapInteractionMode.GestureOnly);
                    return MapInteractionMode.GestureAndControl;
                }
                else
                {
                    SetRotationControlsVisible(MapInteractionMode.GestureAndControl);
                    return MapInteractionMode.GestureAndControl;
                }
            }
        }

        public static void SetRotationControlsVisible(MapInteractionMode Value)
        {
            ApplicationData.Current.LocalSettings.Values["RotationControlsVisible"] = (int)Value;
        }
    }
}
