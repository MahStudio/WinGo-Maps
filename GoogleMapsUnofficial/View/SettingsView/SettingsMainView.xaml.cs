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
        public static Panel TPanel { get; set; }
        public SettingsMainView()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
            TPanel = ThemePanel;
        }

        private void LanguageSets_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsLanguageView));
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();

            if (selectedTheme != null)
            {
                SettingsSetters.RootTheme = SettingsSetters.GetEnum<ElementTheme>(selectedTheme);
            }
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = SettingsSetters.RootTheme.ToString();
            (ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;
        }

        
    }
}
