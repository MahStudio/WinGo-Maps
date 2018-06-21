using GoogleMapsUnofficial.ViewModel.OnMapControls;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class ChangeViewUserControl : UserControl
    {
        public ChangeViewUserControl()
        {
            this.InitializeComponent();
        }

        public void UseGoogleMaps()
        {
            ChangeViewUCVM.UseGoogleMaps(ChangeViewUCVM.CurrentMapMode, ChangeViewUCVM.ShowTraffic, true, ChangeViewUCVM.AllowOverstretch, ChangeViewUCVM.FadeAnimationEnabled);
        }
        public void UseGoogleMaps(MapMode MapMode = MapMode.Standard, bool showtraffic = false, bool AllowCaching = true, bool AllowOverstretch = false, bool IsFadingEnabled = true)
        {
            ChangeViewUCVM.UseGoogleMaps(MapMode, showtraffic, AllowCaching, AllowOverstretch,IsFadingEnabled);
        }
    }
}
