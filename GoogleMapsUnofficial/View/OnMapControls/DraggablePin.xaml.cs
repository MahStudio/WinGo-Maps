using GoogleMapsUnofficial.View.DirectionsControls;
using GoogleMapsUnofficial.ViewModel.GeocodControls;
using GoogleMapsUnofficial.ViewModel.PlaceControls;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public class TestPin
    {
        public static async Task<MapElement> Run(MapControl map)
        {
            var imgContainer = new DraggablePin(map, null);
            map.Children.Add(imgContainer);
            await Task.Delay(50);
            //render symbolicon to bmp
            RenderTargetBitmap renderbmp = new RenderTargetBitmap();
            await renderbmp.RenderAsync(imgContainer);
            map.Children.Remove(imgContainer);
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                //create a bitmap encoder
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                //write pixels into this encoder
                var pixels = await renderbmp.GetPixelsAsync();
                var reader = DataReader.FromBuffer(pixels);
                byte[] bytes = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(bytes);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                    (uint)renderbmp.PixelWidth, (uint)renderbmp.PixelHeight, 0, 0, bytes);
                await encoder.FlushAsync();
                var mapIconStreamReference = RandomAccessStreamReference.CreateFromStream(stream);

                //create mapIcon
                var mapIcon = new MapIcon();
                mapIcon.Image = mapIconStreamReference;
                mapIcon.Location = map.Center;
                mapIcon.Title = "Some label".ToString();
                return mapIcon;
            }
        }
    }
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
