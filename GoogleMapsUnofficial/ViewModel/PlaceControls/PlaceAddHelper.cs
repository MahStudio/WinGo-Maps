using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsUnofficial.ViewModel.PlaceControls
{
    class PlaceAddHelper
    {/// <summary>
     /// Add a missing place to Google Maps 
     /// </summary>
     /// <param name="PlaceInfo">Information about the place you want to add</param>
     /// <returns>return status about the place you added</returns>
        public static async Task<Response> AddPlace(Rootobject PlaceInfo)
        {
            try
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var r = await http.PostAsync($"https://maps.googleapis.com/maps/api/place/add/json?key={AppCore.GoogleMapAPIKey}", new StringContent(JsonConvert.SerializeObject(PlaceInfo)));
                return JsonConvert.DeserializeObject<Response>((await r.Content.ReadAsStringAsync()));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class Rootobject
        {
            public Location location { get; set; }
            public int accuracy { get; set; }
            public string name { get; set; }
            public string phone_number { get; set; }
            public string address { get; set; }
            public string[] types { get; set; }
            public string website { get; set; }
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
                var http = new HttpClient();
                var r = await http.PostAsync($"https://maps.googleapis.com/maps/api/place/delete/json?key={AppCore.GoogleMapAPIKey}", new StringContent(JsonConvert.SerializeObject(PlaceInfo)));
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
