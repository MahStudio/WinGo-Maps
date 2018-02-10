using GoogleMapsUnofficial.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Storage;

namespace GoogleMapsUnofficial.ViewModel.SettingsView
{
    class SettingsLanguageVM : INotifyPropertyChanged
    {
        private CountryCodesHelper CCHelper = new CountryCodesHelper();

        private string _onMapLanguage;
        private string _apiLanguage;
        private int _onmaplangindex;
        private int _apilangindex;

        public event PropertyChangedEventHandler PropertyChanged;

        public int APILanguageIndex
        {
            get { return _apilangindex; }
            set
            {
                _apilangindex = value;
                LanguageSettingsSetters.SetAPILanguage(SupportedLanguages[value].Value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("APILanguageIndex"));
            }
        }
        public int OnMapLanguageIndex
        {
            get { return _onmaplangindex; }
            set
            {
                _onmaplangindex = value;
                LanguageSettingsSetters.SetOnMapLanguage(SupportedMapLanguage[value].Value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnMapLanguageIndex"));
            }
        }
        public string OnMapLanguage
        {
            get { return _onMapLanguage; }
            set
            {
                _onMapLanguage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnMapLanguage"));
            }
        }
        public string APILanguage { get { return _apiLanguage; } set { _apiLanguage = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("APILanguage")); } }

        public List<KeyValuePair<string, string>> SupportedLanguages
        {
            get { return CCHelper.CountriesAndLanguageCodes.ToList(); }
        }
        public List<KeyValuePair<string, string>> SupportedMapLanguage
        {
            get
            {
                var lst = new List<KeyValuePair<string, string>>();
                lst.Add(new KeyValuePair<string, string>("Local Languages", "x-local"));
                lst.AddRange(CCHelper.CountriesAndLanguageCodes);
                return lst;
            }
        }

        public SettingsLanguageVM()
        {
            OnMapLanguageIndex = SupportedMapLanguage.FindIndex(x => x.Value.ToLower() == LanguageSettingsSetters.GetOnMapLanguage());
            APILanguageIndex = SupportedLanguages.FindIndex(x=>x.Value.ToLower() == LanguageSettingsSetters.GetAPILanguage());
        }

    }

    class LanguageSettingsSetters
    {
        public static void SetAPILanguage(string LanguageCode)
        {
            ApplicationData.Current.LocalSettings.Values["APILanguage"] = LanguageCode.ToLower();
            AppCore.GoogleMapRequestsLanguage = LanguageCode.ToLower();
        }

        public static string GetAPILanguage()
        {
            try
            {
                return ApplicationData.Current.LocalSettings.Values["APILanguage"].ToString();
            }
            catch
            {
                SetAPILanguage("en-us");
                return "en-us";
            }
        }
        public static void SetOnMapLanguage(string LanguageCode)
        {
            ApplicationData.Current.LocalSettings.Values["OnMapLanguage"] = LanguageCode.ToLower();
        }

        public static string GetOnMapLanguage()
        {
            try
            {
                return ApplicationData.Current.LocalSettings.Values["OnMapLanguage"].ToString();
            }
            catch
            {
                SetOnMapLanguage("x-local");
                return "x-local";
            }
        }
    }
}
