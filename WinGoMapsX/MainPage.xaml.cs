using RavinduL.LocalNotifications;
using RavinduL.LocalNotifications.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.View;
using WinGoMapsX.View.OfflineMapDownloader;
using WinGoMapsX.View.SettingsView;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinGoMapsX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        object para = null;
        public MainPage(object parameter = null)
        {
            this.InitializeComponent();
            para = parameter;
            OnNavigatedTo(null);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
                base.OnNavigatedTo(e);

            Fr.Navigate(typeof(MapView), para);
            HMenuTopLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringMapView", "Text"), Icon = "", Tag = "Map View" });
            HMenuTopLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringOfflineMaps", "Text"), Icon = "", Tag = "Offline Maps" });

            HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringSendFeedback", "Text"), Icon = "", Tag = "Send feedback" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringSettings", "Text"), Icon = "", Tag = "Settings" });

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            try
            {
                if (CurrentAppSimulator.LicenseInformation.IsTrial)
                {
                    var exp = CurrentAppSimulator.LicenseInformation.ExpirationDate;
                    if (exp.Subtract(DateTime.Now) <= new TimeSpan(4, 0, 0, 0))
                    {
                        await new MessageDialog("Application will be expired on " + exp.ToString()).ShowAsync();
                    }
                }
                //    HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringBuyFromIran", "Text"), Icon = "", Tag = "Buy from Iran" });
            }
            catch
            {
                //HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringBuyFromIran", "Text"), Icon = "", Tag = "Buy from Iran" });
            }
        }
        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (Fr.CanGoBack) Fr.GoBack();
                else
                {
                    LocalNotificationManager lnm = new LocalNotificationManager(MG);
                    lnm.Show(new SimpleNotification
                    {
                        TimeSpan = TimeSpan.FromSeconds(5),
                        Text = "Click/Tap Here to exit",
                        Glyph = "\uE7E8",
                        Background = (new SolidColorBrush((Color)Resources["SystemControlBackgroundAccentBrush"])),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Action = () => { App.Current.Exit(); },
                    }, LocalNotificationCollisionBehaviour.Wait);

                }
            }
            catch
            {

            }
        }

        private void HmenuBTN_Click(object sender, RoutedEventArgs e) => Split.IsPaneOpen = !Split.IsPaneOpen;

        private void Fr_Navigated(object sender, NavigationEventArgs e)
        {
            if (Split.DisplayMode == SplitViewDisplayMode.Overlay || Split.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                if (e.SourcePageType.Name == "MapView")
                    HmenuBTN.Visibility = Visibility.Visible;
                else
                    HmenuBTN.Visibility = Visibility.Collapsed;
            }
            if (Fr.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private async void MenuItem_Click(object sender, ItemClickEventArgs e)
        {
            switch ((e.ClickedItem as MenuClass).Tag)
            {
                case "Buy from Iran":
                    await Launcher.LaunchUriAsync(new Uri("http://winmap.winuser.ir/2018/03/20/%D8%B1%D9%88%D8%B4-%D8%AE%D8%B1%DB%8C%D8%AF-%D8%A8%D8%B1%D9%86%D8%A7%D9%85%D9%87-%D8%A8%D8%B1%D8%A7%DB%8C-%DA%A9%D8%A7%D8%B1%D8%A8%D8%B1%D8%A7%D9%86-%D8%A7%DB%8C%D8%B1%D8%A7%D9%86%DB%8C/", UriKind.RelativeOrAbsolute));
                    break;
                case "Map View":
                    if (Fr.Content.GetType() != typeof(MapView))
                        Fr.Navigate(typeof(MapView));
                    break;
                case "Settings":
                    if (Fr.Content.GetType() != typeof(SettingsMainView))
                        Fr.Navigate(typeof(SettingsMainView));
                    break;
                case "Offline Maps":
                    if (Fr.Content.GetType() != typeof(MapDownloaderView))
                        Fr.Navigate(typeof(MapDownloaderView));
                    break;
                case "Send feedback":
                    await Launcher.LaunchUriAsync(new Uri("mailto:ngame1390@live.com"));
                    break;
                default:
                    break;
            }
            if (Split.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                Split.IsPaneOpen = false;
            }
        }
        private class MenuClass
        {
            public string Tag { get; set; }
            public string Icon { get; set; }
            public string Text { get; set; }
        }


    }
}
