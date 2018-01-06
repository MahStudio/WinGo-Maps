using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AppCore
{
    public static string HttpUserAgent { get; private set; }
    static AppCore()
    {
        HttpUserAgent = "WindowsUniversalGoogleMapsV1Beta";
    }
}

