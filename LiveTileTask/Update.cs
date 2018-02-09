using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;
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
                        var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTile.png", CreationCollisionOption.OpenIfExists);
                        var str = await f.OpenAsync(FileAccessMode.ReadWrite);
                        await str.WriteAsync(res);
                        str.Dispose();
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
                    XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
                    tileXml.GetElementsByTagName("image")[0].Attributes[1].NodeValue = "ms-appdata:///local/LiveTile.png";
                    update.Update(new TileNotification(tileXml));
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

}
