using Windows.Networking.Connectivity;

class InternalHelper
{
    public static bool InternetConnection()
    {
        ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
        if (connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
            return true;
        else return false;
    }
    
}

