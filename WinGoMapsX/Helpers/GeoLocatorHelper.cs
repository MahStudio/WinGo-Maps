using System;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

class GeoLocatorHelper
{
    public static event EventHandler<Geoposition> LocationFetched;
    public static event EventHandler<Geocoordinate> LocationChanged;
    public static bool IsLocationBusy { get; set; }
    private static ExtendedExecutionSession session;
    public static Geolocator Geolocator = new Geolocator() { DesiredAccuracy = PositionAccuracy.High, ReportInterval = 500 };

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
        GetUserLocation();
    }

    public static async void GetUserLocation()
    {
        try
        {
            if (IsLocationBusy) return;
            IsLocationBusy = true;
            var access = await Geolocator.RequestAccessAsync();
            if(access != GeolocationAccessStatus.Allowed)
            {
                var msg = new MessageDialog(MultilingualHelpToolkit.GetString("StringLocationPrivacy", "Text"));
                msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringOK", "Text"), async delegate
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location", UriKind.RelativeOrAbsolute));
                    Window.Current.Activated += Current_Activated;
                }));
                msg.Commands.Add(new UICommand(MultilingualHelpToolkit.GetString("StringCancel", "Text"), delegate { }));
                var a = await msg.ShowAsync();
            }
            Geolocator.PositionChanged += Geolocator_PositionChanged;
            Geolocator.StatusChanged += GeoLocate_StatusChanged;
            var res = Geolocator.GetGeopositionAsync();
            if (res != null)
                res.Completed += new AsyncOperationCompletedHandler<Geoposition>(LocationGetComplete);
            IsLocationBusy = false;
        }
        catch { }
    }

    private static void Current_Activated(object sender, WindowActivatedEventArgs e)
    {
        if (e.WindowActivationState == CoreWindowActivationState.CodeActivated)
        {
            Window.Current.Activated -= Current_Activated;

            GetUserLocation();
        }
    }

    private static void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
    {
        try
        {
            LocationChanged?.Invoke(null, args.Position.Coordinate);
        }
        catch (Exception ex) { }
    }

    private static void GeoLocate_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
    {
        if (args.Status == PositionStatus.NoData || args.Status == PositionStatus.NotAvailable)
        {
            GetUserLocation();
        }
    }

    private static void LocationGetComplete(IAsyncOperation<Geoposition> asyncInfo, AsyncStatus asyncStatus)
    {
        try
        {
            var res = asyncInfo.GetResults();
            LocationFetched?.Invoke(null, res);
            LocationChanged?.Invoke(null, res.Coordinate);
            IsLocationBusy = false;
        }
        catch
        {
        }
    }

}
