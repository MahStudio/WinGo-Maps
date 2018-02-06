using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.View.OfflineMapDownloader;
using GoogleMapsUnofficial.View.OnMapControls;
using GoogleMapsUnofficial.View.SettingsView;
using GoogleMapsUnofficial.ViewModel.OfflineMapDownloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
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
                ac.TintOpacity = 0.8;
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
            Fr.Navigate(typeof(View.MapView), para);
            applyAcrylicAccent(MainGrid);
            HMenuTopLst.Items.Add(new MenuClass { Text = "Map View", Icon = "", Tag = "Map View" });
            HMenuTopLst.Items.Add(new MenuClass { Text = "Offline Maps", Icon = "", Tag = "Offline Maps" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = "Send feedback", Icon = "", Tag = "Send feedback" });
            HMenuBottomLst.Items.Add(new MenuClass { Text = "Settings", Icon = "", Tag = "Settings" });

            return;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (Fr.CanGoBack) Fr.GoBack();
            else App.Current.Exit();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fr.Navigate(typeof(View.OfflineMapDownloader.MapDownloaderView));
        }

        private async void DirBtn_Click(object sender, RoutedEventArgs e)
        {
            var dir = Gr.FindName("DirectionUC") as View.DirectionsControls.DirectionsMainUserControl;
            if (dir == null)
            {
                if (MapView.MapControl.FindName("OrDesSelector") as DraggablePin == null)
                {
                    Gr.Children.Add(new View.DirectionsControls.DirectionsMainUserControl() { Name = "DirectionUC", VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
                    await new MessageDialog("Navigate the point added on your screen to the Origin point and click on it. Then move it to Destination point and click on it again. Select navigation mode from top left menu and hit the navigate button.").ShowAsync();
                }
            }
            else
                Gr.Children.Remove(dir);
        }

        private void Fr_Navigated(object sender, NavigationEventArgs e)
        {
            if (Fr.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

        }

        private void FavPlaces_Click(object sender, RoutedEventArgs e)
        {
            var FavPlaces = Gr.FindName("FavPlaces") as SavedPlacesUserControl;
            if (FavPlaces == null)
            {
                if (MapView.MapControl.FindName("OrDesSelector") as DraggablePin == null)
                {
                    Gr.Children.Add(new SavedPlacesUserControl() { Name = "FavPlaces", Width = 180, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left });
                }
            }
            else
            {
                Gr.Children.Remove(FavPlaces);
            }
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
            if(Split.DisplayMode == SplitViewDisplayMode.Overlay)
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
