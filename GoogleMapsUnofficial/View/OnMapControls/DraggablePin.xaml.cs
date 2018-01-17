using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class DraggablePin : UserControl
    {
        #region Private Properties

        private MapControl _map;
        private bool isDragging = false;
        Geopoint _center;

        #endregion

        #region Constructor

        public DraggablePin(MapControl map)
        {
            this.InitializeComponent();
            _map = map;
            _map.CenterChanged += _map_CenterChanged;
        }
        ~DraggablePin()
        {
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
