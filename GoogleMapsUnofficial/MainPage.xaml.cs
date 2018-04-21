using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.View.OfflineMapDownloader;
using GoogleMapsUnofficial.View.OnMapControls;
using GoogleMapsUnofficial.View.SettingsView;
using RavinduL.LocalNotifications.Notifications;
using System;
using System.Numerics;
using Windows.ApplicationModel.Store;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleMapsUnofficial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        object para = null;
        public static Frame RootFrame { get; set; }
        public static Grid Grid { get; set; }
        public MainPage(object parameter = null)
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            this.Loaded += MainPage_Loaded;
            RootFrame = Fr;
            Grid = Gr;
            para = parameter;
        }
        private void applyAcrylicAccent(Panel panel)
        {
            if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone) return;
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
            {
                var ac = new AcrylicBrush();
                var brush = Resources["SystemControlChromeMediumLowAcrylicElementMediumBrush"] as AcrylicBrush;
                ac = brush;
                ac.TintOpacity = 0.7;
                ac.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                Split.PaneBackground = ac;
                return;
            }
            if (ApiInformation.IsMethodPresent("Windows.UI.Xaml.Hosting.ElementCompositionPreview", "SetElementChildVisual"))
            {
                _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2((float)panel.ActualWidth, (float)panel.ActualHeight);

                ElementCompositionPreview.SetElementChildVisual(panel, _hostSprite);
                var b = _compositor.CreateHostBackdropBrush();
                _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
            }
        }

        Compositor _compositor;
        SpriteVisual _hostSprite;
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hostSprite != null)
                _hostSprite.Size = e.NewSize.ToVector2();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Fr.Navigate(typeof(MapView), para);
            applyAcrylicAccent(MainGrid);
            HMenuTopLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringMapView", "Text"), Icon = "", Tag = "Map View" });
            HMenuTopLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringOfflineMaps", "Text"), Icon = "", Tag = "Offline Maps" });
            try
            {
                if (CurrentAppSimulator.LicenseInformation.IsTrial)
                {
                    var exp = CurrentAppSimulator.LicenseInformation.ExpirationDate;
                    if(exp.Subtract(DateTime.Now) <= new TimeSpan(4,0,0,0))
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
            HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringSendFeedback", "Text"), Icon = "", Tag = "Send feedback" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = MultilingualHelpToolkit.GetString("StringSettings", "Text"), Icon = "", Tag = "Settings" });
            if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone)
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            return;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (Fr.CanGoBack) Fr.GoBack();
                else
                {
                    RavinduL.LocalNotifications.LocalNotificationManager lnm = new RavinduL.LocalNotifications.LocalNotificationManager(MG);
                    lnm.Show(new SimpleNotification
                    {
                        TimeSpan = TimeSpan.FromSeconds(5),
                        Text = "Click/Tap Here to exit",
                        Glyph = "\uE7E8",
                        Background = (new SolidColorBrush((Color)Resources["SystemControlBackgroundAccentBrush"])),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Action = () => { App.Current.Exit(); },
                    }, RavinduL.LocalNotifications.LocalNotificationCollisionBehaviour.Wait);
                    //var msg = new MessageDialog(MultilingualHelpToolkit.GetString("stringExit", "Text"));
                    //msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringYes", "Text"), delegate
                    //{
                    //    App.Current.Exit();
                    //}));
                    //msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringNo", "Text"), delegate { }));
                    //await msg.ShowAsync();
                }
            }
            catch
            {

            }
        }

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

        private void HmenuBTN_Click(object sender, RoutedEventArgs e)
        {
            Split.IsPaneOpen = !Split.IsPaneOpen;
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
                        MainPage.RootFrame.Navigate(typeof(MapView));
                    break;
                case "Settings":
                    if (Fr.Content.GetType() != typeof(SettingsMainView))
                        MainPage.RootFrame.Navigate(typeof(SettingsMainView));
                    break;
                case "Offline Maps":
                    if (Fr.Content.GetType() != typeof(MapDownloaderView))
                        MainPage.RootFrame.Navigate(typeof(MapDownloaderView));
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
