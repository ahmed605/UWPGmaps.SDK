﻿using GMapsUWP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace GMapsUWP.TileAPI
{
    public class TileApiHelper
    {
        /// <summary>
        /// An array of values representing the layer type(s) to be added to the map. 
        /// </summary>
        public enum LayerTypes
        {
            /// <summary>
            /// Required if terrain is specified as the mapType. Can also be optionally overlaid on the satellite mapType. Has no effect on roadmap tiles.
            /// </summary>
            layerRoadmap,
            /// <summary>
            /// Shows Street View-enabled streets and locations using blue outlines on the map.
            /// </summary>
            layerStreetview,
            /// <summary>
            /// Displays current traffic conditions.
            /// </summary>
            layerTraffic
        }
        public enum Scale
        {
            scaleFactor1x,
            scaleFactor2x,
            scaleFactor4x
        }
        public enum mapType
        {
            roadmap,
            satellite,
            terrain,
            streetview
        }

        public static async Task<ResponseClass> GetSessionToken(SessionTokenRequest Request)
        {
            RequestClass req = new RequestClass() { mapType = Request.MapType.ToString(), region = Request.Region, language = Initializer.GoogleMapRequestsLanguage };
            req.highDpi = Request.HighDpi;
            if (Request.LayerTypes != null && Request.LayerTypes.Length > 0)
            {
                foreach (var item in Request.LayerTypes)
                {
                    req.layerTypes.Add(item.ToString());
                }
            }
            req.overlay = Request.Overlay;
            if (Request.Scale != null) req.scale = Request.Scale.Value.ToString();
            var http = Initializer.httpclient;
            using (var resp = await http.PostAsync(new Uri($"https://www.googleapis.com/tile/v1/createSession?key={Initializer.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute),
                new HttpStringContent(JsonConvert.SerializeObject(req))))
            {
                var res = JsonConvert.DeserializeObject<InternalResponse>(await resp.Content.ReadAsStringAsync());
                var t = DateTime.Now.AddSeconds(res.expiry);
                return new ResponseClass()
                {
                    Expiry = t,
                    ImageFormat = res.imageFormat,
                    Session = res.session,
                    TileHeight = res.tileHeight,
                    TileWidth = res.tileWidth
                };
            }
        }

        public static Uri GetMapUri(string SessionToken)
        {
            return new Uri("https://www.googleapis.com/tile/v1/tiles/{x}/{y}/{zoomlevel}?session=" + SessionToken + "&key=" + Initializer.GoogleMapAPIKey, UriKind.RelativeOrAbsolute);
        }

        public class SessionTokenRequest
        {
            /// <summary>
            /// Reuired : The type of base map.
            /// </summary>
            [JsonProperty(PropertyName = "mapType")]
            public mapType MapType { get; set; }
            /// <summary>
            /// Reuired : A CLDR region identifier representing the physical location of the user to whom we are showing these tiles. e.g. fr.
            /// </summary>
            [JsonProperty(PropertyName = "region")]
            public string Region { get; set; }
            /// <summary>
            ///  Scales up the size of map elements, such as road labels, while retaining the tile size and coverage area of the default tile. Increasing the scale will also reduce the number of labels on the map, to reduce clutter.
            /// </summary>
            [JsonProperty(PropertyName = "Scale")]
            public Scale? Scale { get; set; }
            /// <summary>
            /// An array of values representing the layer type(s) to be added to the map.
            /// </summary>
            [JsonProperty(PropertyName = "layerTypes")]
            public LayerTypes[] LayerTypes { get; set; }
            /// <summary>
            /// A boolean value defining whether specified layerTypes should be rendered as a separate overlay, or combined with the base imagery. When true, the base map is not displayed.
            /// </summary>
            [JsonProperty(PropertyName = "overlay")]
            public bool Overlay { get; set; }
            /// <summary>
            /// Specifies whether to return high DPI tiles. If true, the number of pixels in each of the x and y dimensions are multiplied by the scale factor value (ie. 2x or 4x), while the coverage area of the tile is unchanged. This parameter only works with scale values of 2x or 4x; it has no effect on 1x scale tiles.
            /// </summary>
            [JsonProperty(PropertyName = "highDpi")]
            public bool HighDpi { get; set; }
        }
        private class RequestClass
        {
            public string mapType { get; set; }
            public string language { get; set; }
            public string region { get; set; }
            public List<string> layerTypes { get; set; }
            public bool overlay { get; set; }
            public bool highDpi { get; set; }
            public string scale { get; set; }
        }
        private class InternalResponse
        {
            public string session { get; set; }
            public int expiry { get; set; }
            public int tileWidth { get; set; }
            public int tileHeight { get; set; }
            public string imageFormat { get; set; }
        }
        public class ResponseClass
        {
            public string Session { get; set; }
            public DateTime Expiry { get; set; }
            public int TileWidth { get; set; }
            public int TileHeight { get; set; }
            public string ImageFormat { get; set; }
        }
    }
}
