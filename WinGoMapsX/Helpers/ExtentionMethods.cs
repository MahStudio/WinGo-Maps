using GMapsUWP.Directions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WinGoMapsX
{
    public static class ExtentionMethods
    {
        public static Dictionary<string, string> DecodeQueryParameters(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (uri.Query.Length == 0)
                return new Dictionary<string, string>();

            return uri.Query.TrimStart('?')
                            .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                            .GroupBy(parts => parts[0],
                                     parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                            .ToDictionary(grouping => grouping.Key,
                                          grouping => string.Join(",", grouping));
        }

        public static double DistanceFromRoute(this Geopoint UserLocation, DirectionsHelper.Step CurrentStep)
        {
            //Y2,Y1 , X2,X1 points of the line, X0, Y0 user one
            //Latitude = Y, Longitude = X
            var Y0 = UserLocation.Position.Latitude;
            var X0 = UserLocation.Position.Longitude;
            var Y1 = CurrentStep.StartLocation.Latitude;
            var Y2 = CurrentStep.EndLocation.Latitude;
            var X1 = CurrentStep.StartLocation.Longitude;
            var X2 = CurrentStep.EndLocation.Longitude;
            var a = ((Y2 - Y1) * X0) - ((X2 - X1) * Y0) + (X2 * Y1) - (Y2 * X1);
            if (a < 0) a = -1 * a;
            var b = Math.Sqrt((Math.Pow((Y2 - Y1), 2) + Math.Pow((X2 - X1), 2)));
            var dist = ((a / b));
            dist = dist * 60 * 1.1515;
            return dist * 1.609344; // Return kilometer
        }

    }
}
