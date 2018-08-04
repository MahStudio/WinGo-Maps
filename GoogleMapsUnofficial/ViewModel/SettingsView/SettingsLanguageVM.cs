using GoogleMapsUnofficial.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Globalization;
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
        private int _applicationlangindex;

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
        public int ApplicationLanguageIndex
        {
            get { return _applicationlangindex; }
            set
            {
                _applicationlangindex = value;
                LanguageSettingsSetters.SetApplicationLanguage(ApplicationLanguages[value].Value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplicationLanguageIndex"));
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
        public List<KeyValuePair<string, string>> ApplicationLanguages
        {
            get
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Arabic","Ar"),
                    new KeyValuePair<string, string>("Belarusian","Be-By"),
                    new KeyValuePair<string, string>("Czech","cs-cz"),
                    new KeyValuePair<string, string>("English","En-Us"),
                    new KeyValuePair<string, string>("French","Fr-Fr"),
                    new KeyValuePair<string, string>("German","DE-DE"),
                    new KeyValuePair<string, string>("Italian","it"),
                    new KeyValuePair<string, string>("Japanese","ja-JP"),
                    new KeyValuePair<string, string>("Persian","Fa-Ir"),
                    new KeyValuePair<string, string>("Portuguese (Brazil)","Pt-Br"),
                    new KeyValuePair<string, string>("Turkish","tr-tr"),
                    new KeyValuePair<string, string>("Spanish","ES")
                };
            }
        }
        public SettingsLanguageVM()
        {
            OnMapLanguageIndex = SupportedMapLanguage.FindIndex(x => x.Value.ToLower() == LanguageSettingsSetters.GetOnMapLanguage());
            APILanguageIndex = SupportedLanguages.FindIndex(x => x.Value.ToLower() == LanguageSettingsSetters.GetAPILanguage());
            ApplicationLanguageIndex = ApplicationLanguages.FindIndex(x => x.Value.ToLower() == LanguageSettingsSetters.GetApplicationLanguage());
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
        public static void SetApplicationLanguage(string LanguageCode)
        {
            ApplicationData.Current.LocalSettings.Values["ApplicationLanguage"] = LanguageCode.ToLower();
            ApplicationLanguages.PrimaryLanguageOverride = LanguageCode;
            var English = new System.Globalization.CultureInfo("en-us");
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = English.NumberFormat.NumberDecimalSeparator;
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = English.NumberFormat.CurrencyDecimalSeparator;
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator = English.NumberFormat.PercentDecimalSeparator;
        }

        public static string GetApplicationLanguage()
        {
            try
            {
                return ApplicationData.Current.LocalSettings.Values["ApplicationLanguage"].ToString();
            }
            catch
            {
                SetApplicationLanguage("En-Us");
                return "En-Us";
            }
        }
    }
}
