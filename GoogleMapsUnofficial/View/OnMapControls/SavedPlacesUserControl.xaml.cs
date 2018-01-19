using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

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
            MapView.MapControl.Children.Remove(OrDesSelector);
        }
        
        private async void AddPlace_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
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
            });
        }

        private void PNameHolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            PName = PNameHolder.Text;
        }
    }
}
