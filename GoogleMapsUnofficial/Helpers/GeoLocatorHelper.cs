using GoogleMapsUnofficial.ViewModel;
using System;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;

class GeoLocatorHelper
{
    public static event EventHandler<Geoposition> LocationFetched;
    public static bool IsLocationBusy { get; set; }
    private static ExtendedExecutionSession session;

    private static async void StartLocationExtensionSession()
    {
        try
        {
            session = new ExtendedExecutionSession();
            session.Description = "Location Tracker";
            session.Reason = ExtendedExecutionReason.LocationTracking;
            var result = await session.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Denied)
            {
                //TODO: handle denied
                session.Revoked += new TypedEventHandler<object, ExtendedExecutionRevokedEventArgs>(ExtendedExecutionSession_Revoked);
            }
        }
        catch { }
    }

    private static void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
    {
        try
        {
            if (session != null)
            {
                session.Dispose();
                session = null;
            }
        }
        catch { }
    }

    static GeoLocatorHelper()
    {
        IsLocationBusy = false;
        StartLocationExtensionSession();
    }
    public static void GetUserLocation()
    {
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
        try
        {
            var res = asyncInfo.GetResults();
            IsLocationBusy = false;
            LocationFetched(null, res);
        }
        catch
        {
        }
    }

}
