using GoogleMapsUnofficial.Helpers;
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
        private int _rotationcontrolsVisible = -1;
        private int _zoomcontrolsVisible = -1;
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
                if (_zoomcontrolsVisible != -1)
                {
                    SettingsSetters.SetZoomControlsVisible(StringToEnumConverter(MapInteractionModeOptions[value]));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomControlsVisible"));
            }
        }
        public int RotationControlsVisible
        {
            get { return _rotationcontrolsVisible; }
            set
            {
                _rotationcontrolsVisible = value;
                if (_rotationcontrolsVisible != -1)
                {
                    SettingsSetters.SetRotationControlsVisible(StringToEnumConverter(MapInteractionModeOptions[value]));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RotationControlsVisible"));
            }
        }
        public List<string> MapInteractionModeOptions
        {
            get { return Enum.GetNames(typeof(MapInteractionMode)).ToList(); }
        }
        public static List<string> MapInteractionModeOptionsstatic
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
            ZoomControlsVisible = EnumToIndexConverter(SettingsSetters.GetZoomControlsVisible());
            RotationControlsVisible = EnumToIndexConverter(SettingsSetters.GetRotationControlsVisible());
        }

        public static int EnumToIndexConverter(MapInteractionMode Entry)
        {
            return MapInteractionModeOptionsstatic.FindIndex(x => x == Entry.ToString());
        }

        public static MapInteractionMode StringToEnumConverter(string option)
        {
            return (MapInteractionMode)Enum.Parse(typeof(MapInteractionMode), option);
            //switch (Index)
            //{
            //    case "Auto":
            //        return MapInteractionMode.Auto;
            //    case "ControlOnly":
            //        return MapInteractionMode.ControlOnly;
            //    case "Disabled":
            //        return MapInteractionMode.Disabled;
            //    case "GestureAndControl":
            //        return MapInteractionMode.GestureAndControl;
            //    case "GestureOnly":
            //        return MapInteractionMode.GestureOnly;
            //    case "PointerAndKeyboard":
            //        return MapInteractionMode.PointerAndKeyboard;
            //    case "PointerKeyboardAndControl":
            //        return MapInteractionMode.PointerKeyboardAndControl;
            //    case "PointerOnly":
            //        return MapInteractionMode.PointerOnly;
            //    default:
            //        return MapInteractionMode.Auto;
            //}
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
                var r = ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible2"].ToString();
                return SettingsMainVM.StringToEnumConverter(r);
                //return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible"];
            }
            catch
            {
                SetZoomControlsVisible(MapInteractionMode.GestureAndControl);
                return MapInteractionMode.GestureAndControl;
            }
        }

        public static void SetZoomControlsVisible(MapInteractionMode Value)
        {
            ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible2"] = Value.ToString();
        }

        public static MapInteractionMode GetRotationControlsVisible()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["RotationControlsVisible2"].ToString();
                return SettingsMainVM.StringToEnumConverter(r);
                //return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["RotationControlsVisible"];
            }
            catch
            {
                if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone)
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
            ApplicationData.Current.LocalSettings.Values["RotationControlsVisible2"] = Value.ToString();
        }

    }
}
