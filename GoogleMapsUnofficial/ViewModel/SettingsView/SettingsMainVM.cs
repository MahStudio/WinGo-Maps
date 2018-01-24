using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoogleMapsUnofficial.ViewModel.SettingsView
{
    class SettingsMainVM : INotifyPropertyChanged
    {
        private bool _fadeanimationEnabled;
        private bool _allowOverstretch;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool AllowOverstretch
        {
            get { return _allowOverstretch; }
            set { _allowOverstretch = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowOverstretch")); }
        }
        public bool FadeAnimationEnabled
        {
            get { return _fadeanimationEnabled; }
            set { _fadeanimationEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FadeAnimationEnabled")); }
        }

        public SettingsMainVM()
        {
            AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
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
    }
}
