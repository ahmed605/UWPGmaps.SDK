﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace GMapsUWP.Place
{
    class PlaceSearchHelper
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
        /// <summary>
        /// Search nearby places in the mentioned Radius
        /// </summary>
        /// <param name="Location"> The latitude/longitude around which to retrieve place information. This must be specified as latitude,longitude</param>
        /// <param name="Radius">Defines the distance (in meters) within which to return place results. The maximum allowed radius is 50 000 meters. </param>
        /// <param name="Keyword">A term to be matched against all content that Google has indexed for this place, including but not limited to name, type, and address, as well as customer reviews and other third-party content.</param>
        /// <param name="MinPrice">Restricts results to only those places within the specified range. Valid values range between 0 (most affordable) to 4 (most expensive), inclusive.</param>
        /// <param name="MaxPrice">Restricts results to only those places within the specified range. Valid values range between 0 (most affordable) to 4 (most expensive), inclusive.</param>
        /// <returns>Search Result. LOL :D</returns>
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
                para += $"&key={Initializer.GoogleMapAPIKey}&language={Initializer.GoogleMapRequestsLanguage}";
                var http = new HttpClient();
                var st = await http.GetStringAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json?" + para, UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(st);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Rootobject> TextSearch(string query, Geopoint Location = null, int Radius = 0, string Region = "", string NextPageToken = "", SearchPriceEnum MinPrice = SearchPriceEnum.NonSpecified, SearchPriceEnum MaxPrice = SearchPriceEnum.NonSpecified)
        {
            try
            {
                if (Radius > 50000)
                {
                    throw new IndexOutOfRangeException("Radious Value is out of expected range.");
                }
                if (Location != null && Radius == 0) { throw new Exception("Location and radius values must having values"); }
                string para = "";
                para += $"query={query.Replace(" ", "+")}";
                if (Location != null) para += $"location={Location.Position.Latitude},{Location.Position.Longitude}&radius={Radius}";
                if (Region != "") para += $"&region={Region}"; if (MinPrice != SearchPriceEnum.NonSpecified) para += $"&minprice={(int)MinPrice}"; if (MaxPrice != SearchPriceEnum.NonSpecified) para += $"&maxprice={(int)MaxPrice}";
                para += $"&key={Initializer.GoogleMapAPIKey}&language={Initializer.GoogleMapRequestsLanguage}";
                var http = new HttpClient();
                var st = await http.GetStringAsync(new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json?" + para, UriKind.RelativeOrAbsolute));
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

    class DetailsHelper
    {
        /// <summary>
        /// Get a place details using place id
        /// </summary>
        /// <param name="PlaceID">Google Maps place id</param>
        /// <returns>Details of a place including phone number, address and etc.</returns>
        public static async Task<Rootobject> GetPlaceDetails(string PlaceID)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
            var res = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/place/details/json?placeid={PlaceID}&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute));
            return JsonConvert.DeserializeObject<Rootobject>(res);
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

    class Add
    {
        public static async Task<Response> AddPlace(Rootobject PlaceInfo)
        {
            try
            {
                var http = new HttpClient();
                var r = await http.PostAsync($"https://maps.googleapis.com/maps/api/place/add/json?key={Initializer.GoogleMapAPIKey}", new StringContent(JsonConvert.SerializeObject(PlaceInfo)));
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
}
