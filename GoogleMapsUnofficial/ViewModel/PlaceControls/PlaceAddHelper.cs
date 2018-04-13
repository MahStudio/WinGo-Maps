using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace GoogleMapsUnofficial.ViewModel.PlaceControls
{
    class PlaceAddHelper
    {
        /// <summary>
        /// Add a missing place to Google Maps 
        /// </summary>
        /// <param name="PlaceInfo">Information about the place you want to add</param>
        /// <returns>return status about the place you added</returns>
        public static async Task<Response> AddPlace(Rootobject PlaceInfo)
        {
            try
            {
                var http = AppCore.HttpClient;
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.PostAsync(new Uri($"https://maps.googleapis.com/maps/api/place/add/json?key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute), new HttpStringContent(JsonConvert.SerializeObject(PlaceInfo)));
                return JsonConvert.DeserializeObject<Response>((await r.Content.ReadAsStringAsync()));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class Rootobject
        {
            /// <summary>
            ///  (required) — The geographical location, specified as latitude and longitude values, of the place you want to add.
            /// </summary>
            public Location location { get; set; }
            /// <summary>
            ///  The accuracy of the location signal on which this request is based, expressed in meters.
            /// </summary>
            public int accuracy { get; set; }
            /// <summary>
            /// (required) — The full text name of the place. Limited to 255 characters.
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// phone_number (recommended, to improve chances of passing moderation) — The phone number associated with the place. If a place has a well-formatted phone number, it is more likely to pass the moderation process for inclusion in the Google Maps database. This number should be in local or international format:  Local format may differ by country.See the Wikipedia article.For example, the local phone number for Google's Sydney, Australia office is (02) 9374 4000. nternational format includes the country code, and is prefixed with a plus (+) sign.For example, the international phone number for Google's Sydney, Australia office is +61 2 9374 4000.
            /// </summary>
            public string phone_number { get; set; }
            /// <summary>
            /// (recommended, to improve chances of passing moderation) — The address of the place you wish to add. If a place has a well-formatted, human-readable address, it is more likely to pass the moderation process for inclusion in the Google Maps database. 
            /// </summary>
            public string address { get; set; }
            /// <summary>
            /// (required) — The category in which this place belongs. While types takes an array, only one type can currently be specified for a place. XML requests require a single <type> element. See the list of supported types for more information. If none of the supported types are a match for this place, you may specify other.
            /// </summary>
            public string[] types { get; set; }
            /// <summary>
            /// (recommended, to improve chances of passing moderation) — A URL pointing to the authoritative website for this Place, such as a business home page. If a Place has a well-formatted website address, it is more likely to pass the moderation process for inclusion in the Google Maps database.
            /// </summary>
            public string website { get; set; }
            /// <summary>
            /// The language in which the place's name is being reported. See the list of supported languages and their codes. Note that we often update supported languages so this list may not be exhaustive.
            /// </summary>
            public string language { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Response
        {
            public string status { get; set; }
            public string place_id { get; set; }
            public string scope { get; set; }
            public string reference { get; set; }
            public string id { get; set; }
        }

    }

    class PlaceDeleteHelper
    {/// <summary>
     /// Add a missing place to Google Maps 
     /// </summary>
     /// <param name="PlaceInfo">Information about the place you want to add</param>
     /// <returns>return status about the place you added</returns>
        public static async Task<Response> DeletePlace(Rootobject PlaceInfo)
        {
            try
            {
                var http = AppCore.HttpClient;
                var r = await http.PostAsync(new Uri($"https://maps.googleapis.com/maps/api/place/delete/json?key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute), new HttpStringContent(JsonConvert.SerializeObject(PlaceInfo)));
                return JsonConvert.DeserializeObject<Response>((await r.Content.ReadAsStringAsync()));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class Rootobject
        {
            public string place_id { get; set; }
        }

        public class Response
        {
            public string status { get; set; }
        }

    }

}
