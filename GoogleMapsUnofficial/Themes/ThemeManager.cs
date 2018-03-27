using GoogleMapsUnofficial.Common;
using GoogleMapsUnofficial.Core;
using GoogleMapsUnofficial.Dispatcher;
using GoogleMapsUnofficial.Extensions;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace GoogleMapsUnofficial.Themes
{
    public class ThemeManager : ObservableObject
    {
        private static readonly string[] BrushKeys = {
            //wp
            "PhoneAccentBrush",
            // windows

                   "SystemControlDisabledAccentBrush" ,
                   "SystemControlForegroundAccentBrush" ,
                   "SystemControlHighlightAccentBrush" ,
                   "SystemControlHighlightAltAccentBrush",
                   "SystemControlHighlightAltListAccentHighBrush" ,
                   "SystemControlHighlightAltListAccentLowBrush" ,
                   "SystemControlHighlightAltListAccentMediumBrush",
                   "SystemControlHighlightListAccentHighBrush" ,
                   "SystemControlHighlightListAccentLowBrush" ,
                   "SystemControlHighlightListAccentMediumBrush",
                   "SystemControlHyperlinkTextBrush" ,
                   "ContentDialogBorderThemeBrush" ,
                   "JumpListDefaultEnabledBackground" ,
                   "HoverBrush" ,
                   "SystemAccentColor1",
                   "SystemControlBackgroundAccentBrush",
                   "PlaybarBrush"
        };

        public static async void SetThemeColor(string albumartPath)
        {
            await BreadDispatcher.InvokeAsync(() =>
            {

                if (SettingsSetters.GetLocalSetting<string>("SelectedTheme", "Light") == "Light")
                {
                    //ThemeChanged?.Invoke(null, new Events.ThemeChangedEventArgs(oldColor, color));
                }
                else
                {
                    ChangeColor(GetAccentColor());
                }
            });
        }

        private static void ChangeColor(Color color)
        {
            ChangeTitleBarColor(color);
            var oldColor = GetThemeResource<SolidColorBrush>("PlaybarBrush").Color;
            if (oldColor == color)
            {
                return;
            }

            AdjustForeground(color);
            foreach (var brushKey in BrushKeys)
            {
                if (Application.Current.Resources.ContainsKey(brushKey))
                {
                    ((SolidColorBrush)Application.Current.Resources[brushKey]).Color = color;
                }
            }
        }

        private static void ChangeTitleBarColor(Color color)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = color;
                statusBar.BackgroundOpacity = 1;
            }
            else
            {
                ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = color;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = Colors.White;
            }
        }

        private static Color GetAccentColor()
        {
            return ((Color)Application.Current.Resources["SystemAccentColor"]);
        }

        private static T GetThemeResource<T>(string key)
        {
            return ((T)Application.Current.Resources[key]);
        }

        private static void AdjustForeground(Color accentColor)
        {
            GetThemeResource<SolidColorBrush>("TextBrush").Color = accentColor.ToForeground(); //.AnimateBrush(foregroundColor.Color, foreg, "(SolidColorBrush.Color)");
            GetThemeResource<SolidColorBrush>("AccentHoverBrush").Color = accentColor.ToHoverColor();
        }
    }
}