using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Web.Http;

namespace GoogleMapsUnofficial.ViewModel.SearchControls
{
    class SearchHelper
    {
        public enum SearchPriceEnum
        {
            MostAffordable = 0,
            Affordable = 1,
            Normal = 2,
            Expensive = 3,
            MostExpensive = 4,
            NonSpecified = 5
        }

        public static async Task<Rootobject> NearbySearch(BasicGeoposition Location, int Radius, string Keyword = "", SearchPriceEnum MinPrice = SearchPriceEnum.NonSpecified, SearchPriceEnum MaxPrice = SearchPriceEnum.NonSpecified)
        {
            try
            {
                if (Radius > 50000)
                {
                    throw new IndexOutOfRangeException("Radious Value is out of expected range.");
                }
                string para = "";
                para += $"location={Location.Latitude},{Location.Longitude}&radius={Radius}";
                if (Keyword != "") para += $"&keyword={Keyword}"; if (MinPrice != SearchPriceEnum.NonSpecified) para += $"&minprice={(int)MinPrice}"; if (MaxPrice != SearchPriceEnum.NonSpecified) para += $"&maxprice={(int)MaxPrice}";
                para += $"&key={AppCore.GoogleMapAPIKey}";
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var st = await http.GetStringAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json?" + para, UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(st);
            }
            catch
            {
                return null;
            }
        }

        public class Rootobject
        {
            public object[] html_attributions { get; set; }
            public string next_page_token { get; set; }
            public Result[] results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public Photo[] photos { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public string scope { get; set; }
            public string[] types { get; set; }
            public string vicinity { get; set; }
            public Opening_Hours opening_hours { get; set; }
            public float rating { get; set; }
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
            public object[] weekday_text { get; set; }
        }

        public class Photo
        {
            public int height { get; set; }
            public string[] html_attributions { get; set; }
            public string photo_reference { get; set; }
            public int width { get; set; }
        }

    }
}
