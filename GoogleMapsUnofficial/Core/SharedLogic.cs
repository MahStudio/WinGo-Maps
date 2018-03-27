using GoogleMapsUnofficial.ViewModel.SettingsView;
using GoogleMapsUnofficial.Services;

namespace GoogleMapsUnofficial.Core
{
    public class SharedLogic
    {
        public SharedLogic()
        { 

        }
        public SettingsViewModel SettingsVm => GSingleton<SettingsViewModel>.Instance.Singleton;

        public static SharedLogic Instance => GSingleton<SharedLogic>.Instance.Singleton;
    }
}