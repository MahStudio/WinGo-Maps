using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace GoogleMapsUnofficial.ViewModel.PlaceControls
{
    class PlaceDetailsHelper
    {
        /// <summary>
        /// Get a place details using place id
        /// </summary>
        /// <param name="PlaceID">Google Maps place id</param>
        /// <returns>Details of a place including phone number, address and etc.</returns>
        public static async Task<Rootobject> GetPlaceDetails(string PlaceID)
        {
            try
            {
                var http = AppCore.HttpClient;
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var res = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/place/details/json?placeid={PlaceID}&key={AppCore.GoogleMapAPIKey}&language={AppCore.GoogleMapRequestsLanguage}", UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(res);
            }
            catch
            {
                return null;
            }
        }
        public static async Task<Rootobject> GetPlaceDetailsbyReference(string ReferenceID)
        {
            try
            {
                var http = AppCore.HttpClient;
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var res = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/place/details/json?reference={ReferenceID}&key={AppCore.GoogleMapAPIKey}&language={AppCore.GoogleMapRequestsLanguage}", UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(res);
            }
            catch
            {
                return null;
            }
        }
        public class Rootobject
        {
            public object[] html_attributions { get; set; }
            public Result result { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Address_Components[] address_components { get; set; }
            public string adr_address { get; set; }
            public string formatted_address { get; set; }
            public string formatted_phone_number { get; set; }
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string international_phone_number { get; set; }
            public string name { get; set; }
            public Opening_Hours opening_hours { get; set; }
            public Photo[] photos { get; set; }
            public string place_id { get; set; }
            public float rating { get; set; }
            public string reference { get; set; }
            public Review[] reviews { get; set; }
            public string scope { get; set; }
            public string[] types { get; set; }
            public string url { get; set; }
            public int utc_offset { get; set; }
            public string vicinity { get; set; }
            public string website { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public Viewport viewport { get; set; }
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

        public class Opening_Hours
        {
            public bool open_now { get; set; }
            public Period[] periods { get; set; }
            public string[] weekday_text { get; set; }
        }

        public class Period
        {
            public Close close { get; set; }
            public Open open { get; set; }
        }

        public class Close
        {
            public int day { get; set; }
            public string time { get; set; }
        }

        public class Open
        {
            public int day { get; set; }
            public string time { get; set; }
        }

        public class Address_Components
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

        public class Photo
        {
            public int height { get; set; }
            public string[] html_attributions { get; set; }
            public string photo_reference { get; set; }
            public int width { get; set; }
            public Uri PhotoThumbnail { get { return PhotoControls.PhotosHelper.GetPhotoUri(photo_reference, 350,350); } }
        }

        public class Review
        {
            public string author_name { get; set; }
            public string author_url { get; set; }
            public string language { get; set; }
            public string profile_photo_url { get; set; }
            public int rating { get; set; }
            public string relative_time_description { get; set; }
            public string text { get; set; }
            public int time { get; set; }
        }

    }
}
