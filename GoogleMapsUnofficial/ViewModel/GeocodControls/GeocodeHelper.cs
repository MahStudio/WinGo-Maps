using System;
using System.Linq;
using Windows.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Devices.Geolocation;

namespace GoogleMapsUnofficial.ViewModel.GeocodControls
{
    public class GeocodeHelper
    {
        /// <summary>
        /// Get Address of the GeoPoint you provided 
        /// </summary>
        /// <param name="cn">GeoPoint of you want it's address</param>
        /// <returns>Address of Provided GeoPoint. throwing in Exception cause returning "Earth :D" as Address</returns>
        public static async Task<string> GetAddress(Geopoint cn)
        {
            try
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/geocode/json?latlng={cn.Position.Latitude},{cn.Position.Longitude}&sensor=false&language={AppCore.GoogleMapRequestsLanguage}&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute));
                var res = JsonConvert.DeserializeObject<Rootobject>(r);
                return res.results.FirstOrDefault().formatted_address;
            }
            catch { return "Earth :D"; }
        }

        public static async Task<Rootobject> GetInfo(string PlaceID)
        {
            try
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/geocode/json?place_id={PlaceID}&language={AppCore.GoogleMapRequestsLanguage}&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(r);
            }
            catch { return null; }
        }
        public static async Task<Rootobject> GetInfo(Geopoint cn)
        {
            try
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/geocode/json?latlng={cn.Position.Latitude},{cn.Position.Longitude}&language={AppCore.GoogleMapRequestsLanguage}&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(r);
            }
            catch { return null; }
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
