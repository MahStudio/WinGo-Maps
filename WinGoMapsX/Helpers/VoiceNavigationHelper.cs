using GMapsUWP;
using GMapsUWP.Directions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Popups;
using WinGoMapsX.ViewModel.SettingsView;

namespace WinGoMapsX.Helpers
{
    class VoiceNavigationHelper : IDisposable
    {
        static VoiceNavigationHelper ActiveNavigation { get; set; }
        private DirectionsHelper.Leg _cleg;
        private DirectionsHelper.Step _cstep;
        DirectionsHelper.Route CurrentRoute { get; set; }
        DirectionsHelper.Leg CurrentLeg { get => _cleg; set { _cleg = value; CurrentLegChanged(); } }
        DirectionsHelper.Step CurrentStep { get => _cstep; set { _cstep = value; CurrentStepChanged(); } }
        static VoiceInformation SelectedVoice { get; set; }

        bool FirstAlert = false;
        bool SecAlert = false;
        bool voicestarted = false;

        CoreDispatcher Dispatcher { get; set; }

        public VoiceNavigationHelper(DirectionsHelper.Route Route)
        {
            if (ActiveNavigation != null)
            {
                ActiveNavigation.Dispose();
            }
            Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            GeoLocatorHelper.LocationChanged += GeoLocatorHelper_LocationChanged;
            CurrentRoute = Route;
            CurrentLeg = Route.Legs.FirstOrDefault();
            CurrentStep = CurrentLeg.Steps.FirstOrDefault();
            ActiveNavigation = this;
        }

        public void Dispose()
        {
            ActiveNavigation = null;
            GeoLocatorHelper.LocationChanged -= GeoLocatorHelper_LocationChanged;
            Dispatcher = null;
            CurrentRoute = null;
            CurrentLeg = null;
            CurrentStep = null;
            SecAlert = FirstAlert = false;
        }

        public void StopVoice()
        {
            Dispose();
        }

        private async void GeoLocatorHelper_LocationChanged(object sender, Windows.Devices.Geolocation.Geocoordinate e)
        {
            if (CurrentRoute == null || voicestarted) return;
            voicestarted = true;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
             {
                 var cp = e.Point.Position;
                 var ds = DistanceTo(CurrentStep.EndLocation.Latitude, CurrentStep.EndLocation.Longitude, cp.Latitude , cp.Longitude, 'K');
                 if (ds < 0.150)
                 {
                     if (!FirstAlert)
                     {
                         FirstAlert = true;
                         if (CurrentStep.Maneuver == null)
                         {
                             await ReadText(CurrentStep.HtmlInstructions.NoHTMLString());
                         }
                         else
                         {
                             await ReadText(CurrentStep.Maneuver + "\n" + CurrentStep.HtmlInstructions.NoHTMLString());
                         }
                         voicestarted = false;
                         return;
                     }
                     else if (!SecAlert && ds < 0.040)
                     {
                         SecAlert = true;
                         if (CurrentStep.Maneuver == null)
                         {
                             await ReadText(CurrentStep.HtmlInstructions.NoHTMLString());
                         }
                         else
                         {
                             await ReadText(CurrentStep.Maneuver + "\n" + CurrentStep.HtmlInstructions.NoHTMLString());
                         }
                         voicestarted = false;
                         return;
                     }
                     else if (ds < 0.010)
                     {
                        //Next Step/Leg
                        var slist = CurrentLeg.Steps.ToList();
                         var sind = slist.FindIndex(x => x == CurrentStep);
                         if (sind < (slist.Count - 1))
                             CurrentStep = slist[(++sind)];
                         else
                         {
                             var llist = CurrentRoute.Legs.ToList();
                             var lind = llist.FindIndex(x => x == CurrentLeg);
                             if (lind < (llist.Count - 1))
                                 CurrentLeg = llist[(++lind)];
                             else StopVoice(); // Navigation Complete 
                        }
                         CurrentLeg.Steps.Where(x => x == CurrentStep);
                     }
                 }
                 voicestarted = false;
             });
        }

        private void CurrentLegChanged()
        {
            FirstAlert = SecAlert = false;
        }

        private void CurrentStepChanged()
        {
            FirstAlert = SecAlert = false;
        }

        public static async Task ReadText(string Text)
        {
            try
            {
                if (AppCore.GoogleMapRequestsLanguage.StartsWith("fa"))
                {
                    var mediaplayer = new MediaPlayer() { AudioCategory = MediaPlayerAudioCategory.Speech };
                    mediaplayer.Source = MediaSource.CreateFromUri(new Uri("http://api.farsireader.com/ArianaCloudService/ReadTextGET?APIKey=" + AppCore.ArianaAPIKey + "&Text=" + Text + "&Speaker=Female1&Format=mp3%2F32%2Fm&GainLevel=0&PitchLevel=0&PunctuationLevel=0&SpeechSpeedLevel=0&ToneLevel=0", UriKind.Absolute));
                    //mediaplayer.Source = MediaSource.CreateFromStream(r, "mp3");
                    mediaplayer.Play();
                    mediaplayer.MediaEnded += (e, s) =>
                    {
                        e.Dispose();
                    };
                }
                else
                {
                    if(SelectedVoice == null)
                    {
                        var apilng = LanguageSettingsSetters.GetAPILanguage();
                        var lvoices = SpeechSynthesizer.AllVoices.Where(x => x.Language.ToLower() == apilng.ToLower());
                        if (!lvoices.Any())
                        {
                            await new MessageDialog("No voice for selected api language in Settings -> Language Settings").ShowAsync();
                            if (ActiveNavigation != null)
                            {
                                ActiveNavigation.Dispose();
                            }
                            return;
                        }
                        else if (lvoices.Count() > 1)
                        {
                            var voice = lvoices.First(gender => gender.Gender == VoiceGender.Female);
                            if (voice != null)
                                SelectedVoice = voice;
                            else voice = lvoices.FirstOrDefault();
                        }
                    }
                    var speech = new SpeechSynthesizer() { Voice = SelectedVoice };
                    //var speech = new SpeechSynthesizer() { Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female) };
                    var mediaplayer = new MediaPlayer() { AudioCategory = MediaPlayerAudioCategory.Speech };

                    var stream = await speech.SynthesizeTextToStreamAsync(Text);
                    mediaplayer.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
                    mediaplayer.Play();
                    mediaplayer.MediaEnded += (e, s) =>
                    {
                        e.Dispose();
                    };
                }
            }
            catch
            {

            }
        }

        static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
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

    }
}
