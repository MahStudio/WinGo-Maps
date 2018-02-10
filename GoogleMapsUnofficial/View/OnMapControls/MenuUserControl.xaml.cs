using GoogleMapsUnofficial.View.OfflineMapDownloader;
using GoogleMapsUnofficial.View.SettingsView;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class MenuUserControl : UserControl
    {
        private class MenuClass
        {
            public string Tag { get; set; }
            public string Icon { get; set; }
            public string Text { get; set; }
        }
        public MenuUserControl()
        {
            this.InitializeComponent();
            var menu = new List<MenuClass>()
            {
                new MenuClass(){ Icon = "", Text = "Offline Maps", Tag = "Offline Maps"},
                new MenuClass(){ Icon = "", Text = "Directions", Tag= "Directions"},
                new MenuClass(){ Icon = "", Text = "Saved Locations", Tag = "Saved Locations"},
                new MenuClass(){ Icon = "", Text = "Settings", Tag = "Settings" }
            };
            DataContext = menu;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clk = e.ClickedItem as MenuClass;
            var Gr = MainPage.Grid;
            switch (clk.Tag)
            {
                case "Settings":
                    MainPage.RootFrame.Navigate(typeof(SettingsMainView));
                    break;
                case "Offline Maps":
                    MainPage.RootFrame.Navigate(typeof(MapDownloaderView));
                    break;
                case "Saved Locations":
                    var FavPlaces = Gr.FindName("FavPlaces") as SavedPlacesUserControl;
                    if (FavPlaces == null)
                    {
                        if (MapView.MapControl.FindName("OrDesSelector") as DraggablePin == null)
                        {
                            Gr.Children.Add(new SavedPlacesUserControl() { Name = "FavPlaces", Width = 180, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0,0,0,80) });
                        }
                    }
                    else
                    {
                        Gr.Children.Remove(FavPlaces);
                    }
                    break;
                case "Directions":
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
                    break;
                default:
                    break;
            }
            FL.Hide();
        }
    }
}
