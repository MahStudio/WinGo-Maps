using System;
using Windows.Devices.Geolocation;

public class TileCoordinate
{
    public TileCoordinate(double lat, double lon, int zoom)
    {
        this.lat = lat;
        this.lon = lon;
        this.zoom = zoom;
    }
    public double y;
    public double x;
    public double lat;
    public double lon;
    public int zoom;
    public bool locationCoord()
    {
        if (Math.Abs(this.lat) > 85.0511287798066)
            return false;
        double sin_phi = Math.Sin(this.lat * Math.PI / 180);
        double norm_x = this.lon / 180;
        double norm_y = (0.5 * Math.Log((1 + sin_phi) / (1 - sin_phi))) / Math.PI;
        this.y = Math.Pow(2, this.zoom) * ((1 - norm_y) / 2);
        this.x = Math.Pow(2, this.zoom) * ((norm_x + 1) / 2);
        return true;
    }

    public static BasicGeoposition ReverseGeoPoint(double x, double y, double z)
    {
        var Lng = 180 * ((2 * x) / (Math.Pow(2, z)) - 1);
        var Lat = (180 / Math.PI) *
                Math.Asin((Math.Pow(Math.E, (2 * Math.PI * (1 - ((2 * y) / Math.Pow(2, z))))) - 1)
                    / (1 + Math.Pow(Math.E, (2 * Math.PI * (1 - ((2 * y) / Math.Pow(2, z)))))));
        return new BasicGeoposition() { Latitude = Lat, Longitude = Lng };

    }

}
