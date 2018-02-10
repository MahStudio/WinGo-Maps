using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace GoogleMapsUnofficial.ViewModel.GeocodControls
{
    class ReverseGeoCode
    {/// <summary>
     /// Get Location Latitude and Longitude from Address
     /// </summary>
     /// <param name="Address">The address for reverse geocoding</param>
     /// <returns>returns a geopoint contains address Latitude and Longitude</returns>
        public static async Task<Geopoint> GetLocation(string Address)
        {
            try
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.GetStringAsync(new Uri($"http://maps.googleapis.com/maps/api/geocode/json?address={Address}&sensor=false", UriKind.RelativeOrAbsolute));
                var res = JsonConvert.DeserializeObject<Rootobject>(r).results.FirstOrDefault().geometry.location;
                return new Geopoint(new BasicGeoposition() { Latitude = res.lat, Longitude = res.lng });
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
            public bool partial_match { get; set; }
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
