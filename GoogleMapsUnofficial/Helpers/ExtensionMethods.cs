using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Devices.Geolocation;
using static GoogleMapsUnofficial.ViewModel.DirectionsControls.DirectionsHelper;

public static class ExtensionMethods
{
    public static string NoHTMLString(this string HTML)
    {
        return Regex.Replace(HTML, @"<[^>]+>|&nbsp;", "").Trim();
    }
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

    public static double DistanceTo(this Geopoint Origin, Geopoint Destination, char unit = 'K')
    {
        var lat1 = Origin.Position.Latitude;
        var lon1 = Origin.Position.Longitude;
        var lat2 = Destination.Position.Latitude;
        var lon2 = Destination.Position.Longitude;
        double rlat1 = Math.PI * lat1 / 180;
        double rlat2 = Math.PI * lat2 / 180;
        double theta = lon1 - lon2;
        double rtheta = Math.PI * theta / 180;
        double dist =
            Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
            Math.Cos(rlat2) * Math.Cos(rtheta);
        dist = Math.Acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;
        switch (unit)
        {
            case 'K': //Kilometers -> default
                return dist * 1.609344;
            case 'N': //Nautical Miles 
                return dist * 0.8684;
            case 'M': //Miles
                return dist;
        }
        return dist;
    }

    public static double DistanceFromRoute(this Geopoint UserLocation, Step CurrentStep)
    {
        //Y2,Y1 , X2,X1 points of the line, X0, Y0 user one
        //Latitude = Y, Longitude = X
        var Y0 = UserLocation.Position.Latitude;
        var X0 = UserLocation.Position.Longitude;
        var Y1 = CurrentStep.start_location.lat;
        var Y2 = CurrentStep.end_location.lat;
        var X1 = CurrentStep.start_location.lng;
        var X2 = CurrentStep.end_location.lng;
        var a = ((Y2 - Y1) * X0) - ((X2 - X1) * Y0) + (X2 * Y1) - (Y2 * X1);
        if (a < 0) a = -1 * a;
        var b = Math.Sqrt((Math.Pow((Y2 - Y1), 2) + Math.Pow((X2 - X1), 2)));
        var dist = ((a / b));
        dist = dist * 60 * 1.1515;
        return dist * 1.609344; // Return kilometer
    }
}

