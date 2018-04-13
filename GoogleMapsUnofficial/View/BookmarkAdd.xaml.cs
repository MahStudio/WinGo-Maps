using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial.View
{
    public class BookmarkAddNeedsClass
    {
        public string PlaceID { get; set; }
        public Geopoint Location { get; set; }
        public string PlaceName { get; set; }
    }
    public sealed partial class BookmarkAdd : ContentDialog
    {
        public BookmarkAdd(BookmarkAddNeedsClass Context)
        {
            this.InitializeComponent();
            DataContext = Context;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var context = DataContext as BookmarkAddNeedsClass;
            if (DefaultName.IsChecked.Value)
            {
                try
                {
                    SavedPlacesVM.AddNewPlace(new SavedPlacesVM.SavedPlaceClass() { PlaceName = context.PlaceName, Latitude = context.Location.Position.Latitude, Longitude = context.Location.Position.Longitude });
                    if (PinLiveTile.IsChecked.Value)
                    {
                        //var pd = await ViewModel.PlaceControls.PlaceDetailsHelper.GetPlaceDetails((DataContext as BookmarkAddNeedsClass).PlaceID);
                        Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-100.png");
                        SecondaryTile tile = new SecondaryTile(new Random(1).Next(Int32.MaxValue).ToString(), "Home", $"{context.Location.Position.Latitude},{context.Location.Position.Longitude}", square150x150Logo, TileSize.Square150x150);
                        tile.DisplayName = PlaceNameText.Text;
                        tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                        tile.VisualElements.ShowNameOnWide310x150Logo = true;
                        tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                        tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
                        tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTile.scale-200.png");
                        await tile.RequestCreateAsync();
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                    await this.ShowAsync();
                }
            }
            else
            {
                try
                {
                    SavedPlacesVM.AddNewPlace(new SavedPlacesVM.SavedPlaceClass() { PlaceName = PlaceNameText.Text, Latitude = context.Location.Position.Latitude, Longitude = context.Location.Position.Longitude });
                    if (PinLiveTile.IsChecked.Value)
                    {
                        Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-100.png");
                        SecondaryTile tile = new SecondaryTile(new Random(1).Next(Int32.MaxValue).ToString(), "Home", $"{context.Location.Position.Latitude},{context.Location.Position.Longitude}", square150x150Logo, TileSize.Square150x150);
                        tile.DisplayName = PlaceNameText.Text;
                        tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                        tile.VisualElements.ShowNameOnWide310x150Logo = true;
                        tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                        tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
                        tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTile.scale-200.png");
                        await tile.RequestCreateAsync();
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                    await this.ShowAsync();
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void RadioButtons_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomName.IsChecked.Value)
                {
                    PlaceNameText.IsEnabled = true;
                }
                else PlaceNameText.IsEnabled = false;
            }
            catch { }
        }
    }
}
