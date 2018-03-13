using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;

namespace LocationBasedNotify
{
    public sealed class NotifyUser : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var def = taskInstance.GetDeferral();
                if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
                {
                    var geolocator = new Geolocator();
                    var Location = await geolocator.GetGeopositionAsync();
                    var ul = Location.Coordinate.Point.Position;
                    
                }
                def.Complete();
            }
            catch (Exception ex)
            {
            }
        }
        public static bool InternetConnection()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            if (connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                return true;
            else return false;
        }
    }
}

