using GoogleMapsUnofficial.ViewModel;
using System;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;

class GeoLocatorHelper
{
    public static event EventHandler<Geoposition> LocationFetched;
    public static Geoposition UserPosition { get; set; }
    public static bool IsLocationBusy { get; set; }
    private static ExtendedExecutionSession session;

    private static async void StartLocationExtensionSession()
    {
        session = new ExtendedExecutionSession();
        session.Description = "Location Tracker";
        session.Reason = ExtendedExecutionReason.LocationTracking;
        session.Revoked += new TypedEventHandler<object, ExtendedExecutionRevokedEventArgs>(ExtendedExecutionSession_Revoked);
        var result = await session.RequestExtensionAsync();
        if (result == ExtendedExecutionResult.Denied)
        {
            //TODO: handle denied
        }
    }

    private static void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
    {
        if (session != null)
        {
            session.Dispose();
            session = null;
        }
    }

    static GeoLocatorHelper()
    {
        IsLocationBusy = false;
        StartLocationExtensionSession();
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
