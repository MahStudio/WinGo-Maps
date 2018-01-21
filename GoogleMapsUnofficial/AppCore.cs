using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AppCore
{
    public static string GoogleMapRequestsLanguage { get; private set; }
    public static string GoogleMapAPIKey { get; private set; }
    public static string HttpUserAgent { get; private set; }
    static AppCore()
    {
        HttpUserAgent = "WindowsUniversalGoogleMapsV2ALPHA";
        GoogleMapAPIKey = "AIzaSyCS5gpejHZIpgK7StAfFCcTqZ8cQsuHVnw";
        GoogleMapRequestsLanguage = "en-US";
    }
}

