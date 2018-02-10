using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Web.Http;

namespace GoogleMapsUnofficial.ViewModel.PlaceControls
{
    class PlaceAutoComplete
    {
        public static async Task<Rootobject> GetAutoCompleteResults(string input, int radius = 0, Geopoint location = null)
        {
            try
            {
                var http = new HttpClient();
                var para = $"input={input}&language={AppCore.GoogleMapRequestsLanguage}&key={AppCore.GoogleMapAPIKey}";
                if (radius != 0) para += $"&radius={radius}"; if (location != null) para += $"&location={location.Position.Latitude},{location.Position.Longitude}";
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/place/autocomplete/json?{para}"));

                return JsonConvert.DeserializeObject<Rootobject>(r);
            }
            catch 
            {
                return null;
            }
        }

        public class Rootobject
        {
            public Prediction[] predictions { get; set; }
            public string status { get; set; }
        }

        public class Prediction
        {
            public string description { get; set; }
            public string id { get; set; }
            public Matched_Substrings[] matched_substrings { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public Structured_Formatting structured_formatting { get; set; }
            public Term[] terms { get; set; }
            public string[] types { get; set; }
        }

        public class Structured_Formatting
        {
            public string main_text { get; set; }
            public Main_Text_Matched_Substrings[] main_text_matched_substrings { get; set; }
            public string secondary_text { get; set; }
        }

        public class Main_Text_Matched_Substrings
        {
            public int length { get; set; }
            public int offset { get; set; }
        }

        public class Matched_Substrings
        {
            public int length { get; set; }
            public int offset { get; set; }
        }

        public class Term
        {
            public int offset { get; set; }
            public string value { get; set; }
        }

    }
}
