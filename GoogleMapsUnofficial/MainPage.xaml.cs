using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.View.OfflineMapDownloader;
using GoogleMapsUnofficial.View.OnMapControls;
using GoogleMapsUnofficial.View.SettingsView;
using System;
using System.Numerics;
using Windows.Foundation.Metadata;
using Windows.System;
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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Fr.Navigate(typeof(MapView));
            applyAcrylicAccent(MainGrid);
            HMenuTopLst.Items.Add(new MenuClass { Text = "Map View", Icon = "", Tag = "Map View" });
            HMenuTopLst.Items.Add(new MenuClass { Text = "Offline Maps", Icon = "", Tag = "Offline Maps" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = "Send feedback", Icon = "", Tag = "Send feedback" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = "Settings", Icon = "", Tag = "Settings" });
            return;
        }

        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (Fr.CanGoBack) Fr.GoBack();
            else
            {
                var msg = new MessageDialog("Are you sure you want to exit?");
                msg.Commands.Add(new UICommand("Yes", delegate
                {
                    App.Current.Exit();
                }));
                msg.Commands.Add(new UICommand("No", delegate { }));
                await msg.ShowAsync();
            }
        }
        
        private void Fr_Navigated(object sender, NavigationEventArgs e)
        {
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
                case "Map View":
                    MainPage.RootFrame.Navigate(typeof(MapView));
                    break;
                case "Settings":
                    MainPage.RootFrame.Navigate(typeof(SettingsMainView));
                    break;
                case "Offline Maps":
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
