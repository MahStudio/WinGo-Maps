using GoogleMapsUnofficial.View.OnMapControls;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class DirectionsMainUserControl : UserControl
    {
        public enum DirectionsMode
        {
            Driving = 1,
            Transit = 2,
            Walking = 3
        }
        private static string _orgadd { get; set; }
        private static string _desadd { get; set; }
        public static string OriginAddress { get { return _orgadd; } set { _orgadd = value; OriginAddressChanged?.Invoke("", value); } }
        public static string DestinationAddress { get { return _desadd; } set { _desadd = value; DestinationAddressChanged?.Invoke("", value); } }
        public static event EventHandler<string> OriginAddressChanged;
        public static event EventHandler<string> DestinationAddressChanged;
        public static Geopoint Origin { get; set; }
        public static List<Geopoint> WayPoints { get; set; }
        public static Geopoint Destination { get; set; }
        public static DirectionsMode SelectedDirectionMode;
        public DirectionsMainUserControl()
        {
            this.InitializeComponent();
            WayPoints = null;
            this.Loaded += DirectionsMainUserControl_Loaded;
            this.Unloaded += DirectionsMainUserControl_Unloaded;
        }

        private void DirectionsMainUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var gr = MapView.MapControl.FindName("OrDesSelector") as DraggablePin;
            MapView.MapControl.Children.Remove(gr);
            Origin = null;
            WayPoints = null;
            Destination = null;
            MainPage.Grid.Children.Remove(this);
        }

        private void DirectionsMainUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DraggablePin pin = new DraggablePin(MapView.MapControl, this);
            pin.Name = "OrDesSelector";
            MapControl.SetLocation(pin, MapView.MapControl.Center);

            //Set the pin as draggable.
            pin.Draggable = true;

            //Add the pin to the map.
            MapView.MapControl.Children.Add(pin);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Piv.SelectedIndex == 0)
            {
                SelectedDirectionMode = DirectionsMode.Walking;
            }
            else if (Piv.SelectedIndex == 1)
            {
                SelectedDirectionMode = DirectionsMode.Transit;
            }
            else if (Piv.SelectedIndex == 2)
            {
                SelectedDirectionMode = DirectionsMode.Driving;
            }
        }
        public static async void AddPointer(Geopoint ploc, string Title)
        {
            var Pointer = (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/InAppIcons/GMP.png")));
            MapView.MapControl.MapElements.Add(new MapIcon()
            {
                Location = ploc,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = Title,
                Image = RandomAccessStreamReference.CreateFromFile(Pointer),
            });
        }
    }
}
