using GoogleMapsUnofficial.Common;

namespace GoogleMapsUnofficial.ViewModel.SettingsView
{
    public class PersonalizationViewModel : ObservableObject
    {
        private bool _isThemeDark;


        public PersonalizationViewModel()
        {
            _isThemeDark = SettingsSetters.GetLocalSetting<string>("SelectedTheme", null) == "Light" ? true : false;
        }

        
        public bool IsThemeDark
        {
            get => _isThemeDark;
            set
            {
                Set(ref _isThemeDark, value);
                SettingsSetters.SaveLocalSetting("SelectedTheme", _isThemeDark ? "Light" : "Dark");
                // SharedLogic.InitializeTheme();
            }
        }
        
    }
}
