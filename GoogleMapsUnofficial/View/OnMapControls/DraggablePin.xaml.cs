using GoogleMapsUnofficial.View.DirectionsControls;
using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class DraggablePin : UserControl
    {
        #region Private Properties
        private object ClassInitializer;
        private MapControl _map;
        #endregion

        #region Constructor

        public DraggablePin(MapControl map, object Sender)
        {
            this.InitializeComponent();
            _map = map;
            _map.CenterChanged += _map_CenterChanged;
            this.Tapped += DraggablePin_Tapped;
            ClassInitializer = Sender;
        }

        private async void DraggablePin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                if (ClassInitializer.GetType() == typeof(DirectionsMainUserControl))
                {
                    var Pointer = (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/InAppIcons/GMP.png")));
                    if (DirectionsMainUserControl.Origin == null)
                    {
                        DirectionsMainUserControl.Origin = _map.Center;
                        _map.MapElements.Add(new MapIcon()
                        {
                            Location = _map.Center,
                            NormalizedAnchorPoint = new Point(0.5, 1.0),
                            Title = "Origin",
                            Image = RandomAccessStreamReference.CreateFromFile(Pointer),
                        });
                        DirectionsMainUserControl.OriginAddress = await GeocodeHelper.GetAddress(_map.Center);
                    }
                    else if (DirectionsMainUserControl.Destination == null)
                    {
                        DirectionsMainUserControl.Destination = _map.Center;
                        _map.MapElements.Add(new MapIcon()
                        {
                            Location = _map.Center,
                            NormalizedAnchorPoint = new Point(0.5, 1.0),
                            Title = "Destination",
                            Image = RandomAccessStreamReference.CreateFromFile(Pointer)
                        });
                        DirectionsMainUserControl.DestinationAddress = await GeocodeHelper.GetAddress(_map.Center);
                    }
                }
                if (ClassInitializer.GetType() == typeof(SavedPlacesUserControl))
                {
                    if (SavedPlacesUserControl.PName == string.Empty)
                    {
                        await new MessageDialog("Specify a name for this place").ShowAsync();
                    }
                    else
                    {
                        try
                        {
                            SavedPlacesVM.AddNewPlace(new SavedPlacesVM.SavedPlaceClass()
                            {
                                Latitude = _map.Center.Position.Latitude,
                                Longitude = _map.Center.Position.Longitude,
                                PlaceName = SavedPlacesUserControl.PName
                            });
                        }
                        catch (Exception ex)
                        {
                            await new MessageDialog(ex.Message).ShowAsync();
                        }
                    }
                }
            });
        }

        ~DraggablePin()
        {
            ClassInitializer = null;
            _map.CenterChanged -= _map_CenterChanged;
            _map = null;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// A boolean indicating whether the pushpin can be dragged.
        /// </summary>
        public bool Draggable { get; set; }

        #endregion

        #region  Public Events

        /// <summary>
        /// Occurs when the pushpin is being dragged.
        /// </summary>
        public Action<Geopoint> Drag;

        /// <summary>
        /// Occurs when the pushpin starts being dragged.
        /// </summary>
        public Action<Geopoint> DragStart;

        /// <summary>
        /// Occurs when the pushpin stops being dragged.
        /// </summary>
        public Action<Geopoint> DragEnd;

        #endregion

        #region Private Methods

        private void _map_CenterChanged(MapControl sender, object args)
        {
            //Reset the map center to the stored center value.
            //This prevents the map from panning when we drag across it.
            MapControl.SetLocation(this, sender.Center);

        }
        #endregion
    }
}
