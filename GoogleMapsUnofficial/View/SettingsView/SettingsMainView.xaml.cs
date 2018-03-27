using GoogleMapsUnofficial.ViewModel.SettingsView;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial.View.SettingsView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsMainView : Page
    {
        public PersonalizationViewModel PersonalizationVM { get; set; }

        public static Panel TPanel { get; set; }
        public static RadioButton rbutton { get; set; }
        public SettingsMainView()
        {
            this.InitializeComponent();
            PersonalizationVM = new PersonalizationViewModel();
        }

        private void LanguageSets_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsLanguageView));
        }
    }
}
