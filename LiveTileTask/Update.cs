using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
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
                    var http = new HttpClient();
                    http.DefaultRequestHeaders.UserAgent.ParseAdd("MahStudioWinGoMaps");
                    var res = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=200x200", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                    var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTile.png", CreationCollisionOption.OpenIfExists);
                    var str = await f.OpenAsync(FileAccessMode.ReadWrite);
                    await str.WriteAsync(res);
                    str.Dispose();
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
    }
}
