using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace WeatherApp
{
    class WeatherAPI // API KEY: b1c8a5cea60f17f305ee2d9e3305af25
    { // http://api.openweathermap.org/geo/1.0/direct?q=
        private static string apiKey = "b1c8a5cea60f17f305ee2d9e3305af25";

        public static string requestWeather(double latitude, double longitude)
        {
            var client = new RestClient("http://api.openweathermap.org/data/2.5/weather");
            var request = new RestRequest($"?appid={apiKey}&lat={latitude}&lon={longitude}");
            var response = client.Get(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }

        public static double kelvinToCelsius(double tempInCelsius)
        {
            return Math.Round(tempInCelsius - 273.15);
        }

        public static string timestampToTime(int timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timestamp);
            return $"{dateTime.Hour}:{dateTime.Minute}";
        }
    }
}

namespace OWMObject // OWM = Open Weather Map, made with https://json2csharp.com/
{
    public class Clouds
    {
        public int all { get; set; }
    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
    }

    public class Rain
    {
        [JsonProperty("1h")]
        public double _1h { get; set; }
    }

    public class Root
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
        public double gust { get; set; }
    }
}