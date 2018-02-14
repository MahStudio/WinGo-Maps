using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Web.Http;

namespace WinGoService
{
    public sealed class Cortana : IBackgroundTask
    {
        BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            // This should match the uap:AppService and VoiceCommandService references from the  
            // package manifest and VCD files, respectively. Make sure we've been launched by  
            // a Cortana Voice Command.  
            if (triggerDetails != null && triggerDetails.Name == "Cortana")
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;
                    // GetVoiceCommandAsync establishes initial connection to Cortana, and must // be called prior to any  
                    // messages sent to Cortana. Attempting to use // ReportSuccessAsync, ReportProgressAsync, etc  
                    // prior to calling this will produce undefined behavior.  
                    var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();
                    var text = voiceCommand.SpeechRecognitionResult.Text;
                    var loc = await Geolocator.RequestAccessAsync();
                    if (loc == GeolocationAccessStatus.Allowed)
                    {
                        Geolocator geolocator = new Geolocator();
                        var myloc = await geolocator.GetGeopositionAsync();
                        RespondTouser("Here is your location", myloc.Coordinate.Point);
                    }
                    else
                    {
                        RespondTouser("I can't access to your location! Where are you?");
                    }
                }
                catch { }
            }
        }
        private async void RespondTouser(string text, Geopoint point = null)
        {
            var userMessage = new VoiceCommandUserMessage();
            string keepingTripToDestination = text; //How can i Help you?  
            userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;
            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            var destinationTile = new VoiceCommandContentTile();
            destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWith280x140IconAndText;
            if (point != null)
            {
                destinationTile.Title = await GeocodeHelper.GetAddress(point);
                try
                {
                    var http = new HttpClient();
                    var httpres = await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={point.Position.Latitude},{point.Position.Longitude}&zoom=16&size=280x140&markers=Red|label:G|{point.Position.Latitude},{point.Position.Longitude}", UriKind.RelativeOrAbsolute));
                    var buf = await httpres.Content.ReadAsBufferAsync();
                    var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("CortanaResp.png", CreationCollisionOption.OpenIfExists);
                    var fread = await f.OpenAsync(FileAccessMode.ReadWrite);
                    await fread.WriteAsync(buf);
                    fread.Dispose();
                    destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/CortanaResp.png", UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    destinationTile.Title = text;
                    destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SplashScreen.scale-200.png", UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                destinationTile.Title = text;
                destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SplashScreen.scale-200.png", UriKind.RelativeOrAbsolute));
            }
            destinationsContentTiles.Add(destinationTile);

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }
    }
    class GeocodeHelper
    {
        public static async Task<string> GetAddress(Geopoint cn)
        {
            try
            {
                var http = new HttpClient();
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/geocode/json?latlng={cn.Position.Latitude},{cn.Position.Longitude}&sensor=false&language=en-us&key=AIzaSyCS5gpejHZIpgK7StAfFCcTqZ8cQsuHVnw", UriKind.RelativeOrAbsolute));
                var res = JsonConvert.DeserializeObject<Rootobject>(r);
                return res.results.FirstOrDefault().formatted_address;
            }
            catch { return "Earth :D"; }
        }
        public class Rootobject
        {
            public Result[] results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Address_Components[] address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string place_id { get; set; }
            public string[] types { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
            public Bounds bounds { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Northeast
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Bounds
        {
            public Northeast1 northeast { get; set; }
            public Southwest1 southwest { get; set; }
        }

        public class Northeast1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Address_Components
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

    }
}