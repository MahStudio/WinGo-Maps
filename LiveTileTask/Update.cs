using System;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Http;

namespace LiveTileTask
{
    public sealed class Update : IBackgroundTask
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
                    if(InternetConnection())
                    {
                        var http = new HttpClient();
                        http.DefaultRequestHeaders.UserAgent.ParseAdd("MahStudioWinGoMaps");
                        var res = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=200x200", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                        var reswide = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=310x150", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                        var resLarge = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=310x310", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                        var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTile.png", CreationCollisionOption.OpenIfExists);
                        var fWide = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTileWide.png", CreationCollisionOption.OpenIfExists);
                        var fLarge = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTileLarge.png", CreationCollisionOption.OpenIfExists);
                        var str = await f.OpenAsync(FileAccessMode.ReadWrite);
                        var strwide = await fWide.OpenAsync(FileAccessMode.ReadWrite);
                        var strLarge = await fLarge.OpenAsync(FileAccessMode.ReadWrite);
                        await str.WriteAsync(res);
                        await strwide.WriteAsync(reswide);
                        await strLarge.WriteAsync(resLarge);
                        str.Dispose();
                        strwide.Dispose();
                        strLarge.Dispose();
                    }
                    else
                    {
                        try
                        {
                            var tc = new TileCoordinate(ul.Latitude, ul.Longitude, 16);
                            var x = tc.x;
                            //mah_x_{x}-y_{y}-z_{zoomlevel}.jpeg
                            var f = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///local/MahMaps/mah_x_{tc.x}-y_{tc.y}-z_16.jpeg"));
                            await f.CopyAsync(ApplicationData.Current.LocalFolder, "LiveTile.png");
                        }
                        catch
                        {

                        }
                    }
                    var update = TileUpdateManager.CreateTileUpdaterForApplication();
                    string xml = "<tile>\n";
                    xml += "<visual version=\"4\">\n";
                    xml += "  <binding template=\"TileSquare150x150Image\" fallback=\"TileSquareImage\">\n";
                    xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTile.png\"/>\n";
                    xml += "  </binding>\n";
                    xml += "  <binding template=\"TileWide310x150Image\" fallback=\"TileWideImage\">\n";
                    xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTileWide.png\"/>\n";
                    xml += "  </binding>\n";
                    xml += "  <binding template=\"TileSquare310x310Image\" fallback=\"TileSquareImageLarge\">\n";
                    xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTileLarge.png\"/>\n";
                    xml += "  </binding>\n";
                    xml += "</visual>\n";
                    xml += "</tile>";

                    XmlDocument txml = new XmlDocument();
                    txml.LoadXml(xml);
                    TileNotification tNotification = new TileNotification(txml);

                    update.Clear();
                    update.Update(tNotification);
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
    class TileCoordinate
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
        public bool LocationCoord()
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

}
