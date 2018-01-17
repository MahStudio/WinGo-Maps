using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class SavedPlacesUserControl : UserControl
    {
        public static string PName = "";

        public SavedPlacesUserControl()
        {
            this.InitializeComponent();
            DraggablePin draggablePin = new DraggablePin(MapView.MapControl, this) { Name = "OrDesSelector" };
            draggablePin.Draggable = true;
            MapControl.SetLocation(draggablePin, MapView.MapControl.Center);
            MapView.MapControl.Children.Add(draggablePin);
            this.Unloaded += SavedPlacesUserControl_Unloaded;
        }

        private void SavedPlacesUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            PName = "";
            var OrDesSelector = MapView.MapControl.FindName("OrDesSelector") as DraggablePin;
            if (OrDesSelector != null)
                MapView.MapControl.Children.Remove(OrDesSelector);
        }
        
        private async void AddPlace_Click(object sender, RoutedEventArgs e)
        {
            if (PName == string.Empty)
            {
                await new MessageDialog("Specify a name for this place").ShowAsync();
            }
            else
            {
                try
                {
                    SavedPlacesVM.AddNewPlace(new SavedPlacesVM.SavedPlaceClass()
                    {
                        Latitude = MapView.MapControl.Center.Position.Latitude,
                        Longitude = MapView.MapControl.Center.Position.Longitude,
                        PlaceName = PNameHolder.Text
                    });
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
        }

        private void PNameHolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            PName = PNameHolder.Text;
        }
    }
}
