using System;
using System.Collections.Generic;
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
                        if (text.Equals("I"))
                        {
                            RespondTouser("Hi, How are your doing ?");
                        }
                        else
                        {
                            //Here you call up any api's to create a meaning full response to user/call // services like Bot/Luis etc... to create a meaningful response.  
                            RespondTouser("You Said ," + text);
                        }
                    }
                }
                catch (Exception ex) { }
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
                destinationTile.Title = $"Your location";
                var http = new HttpClient();
                var httpres = await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={point.Position.Latitude},{point.Position.Longitude}&zoom=16&size=280x140&markers=Red|label:G|{point.Position.Latitude},{point.Position.Longitude}", UriKind.RelativeOrAbsolute));
                var buf = await httpres.Content.ReadAsBufferAsync();
                var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("CortanaResp.png", CreationCollisionOption.OpenIfExists);
                var fread = await f.OpenAsync(FileAccessMode.ReadWrite);
                await fread.WriteAsync(buf);
                fread.Dispose();
                destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/CortanaResp.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                destinationTile.Title = "Mashad, Iran";
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
}