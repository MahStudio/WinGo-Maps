using GoogleMapsUnofficial.ViewModel;
using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;

class GeoLocatorHelper
{
    public static event EventHandler<Geoposition> LocationFetched;
    public static Geoposition UserPosition { get; set; }
    public static bool IsLocationBusy { get; set; }
    static GeoLocatorHelper()
    {
        IsLocationBusy = false;
    }
    public static void GetUserLocation()
    {
        if (IsLocationBusy) return;
        var res = MapViewVM.GeoLocate.GetGeopositionAsync();
        MapViewVM.GeoLocate.StatusChanged += GeoLocate_StatusChanged;
        IsLocationBusy = true;
        res.Completed += new AsyncOperationCompletedHandler<Geoposition>(LocationGetComplete);
    }

    private static void GeoLocate_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
    {
        if (args.Status == PositionStatus.Disabled || args.Status == PositionStatus.NoData || args.Status == PositionStatus.NotAvailable)
            GetUserLocation();
    }

    private static void LocationGetComplete(IAsyncOperation<Geoposition> asyncInfo, AsyncStatus asyncStatus)
    {
        var res = asyncInfo.GetResults();
        IsLocationBusy = false;
        LocationFetched(null, res);
    }

}
