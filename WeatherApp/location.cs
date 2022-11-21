using Newtonsoft.Json;
using OWMObject;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locationObj;
using System.Windows.Controls;
using System.Diagnostics;

namespace WeatherApp
{
    internal class LocationAPI
    {
        public static List<Location> GetLocations(string city, string country, string apiKey)
        {
            var client = new RestClient("http://api.openweathermap.org/geo/1.0/direct");
            string requestStr = $"?appid={apiKey}&q={city}&limit=100";
            if (country != "")
            {
                requestStr += $",{country}";
            }
            var request = new RestRequest(requestStr);
            var response = client.Get(request);

            if(response.IsSuccessful)
            {
                var locations = JsonConvert.DeserializeObject<List<Location>>(response.Content);
                return locations;
            } else {
                // unsuccessful
                return null;
            }
        }

        public static void saveToList(string name, string state, string country, double lat, double lon)
        {

        }
        
        public static void deleteFromList(string id)
        {

        }
    }
}

namespace locationObj
{
    public class LocalNames
    {
        public string de { get; set; }
        public string en { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public LocalNames local_names { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string country { get; set; }
        public string state { get; set; }
    }

    public class LocationList
    {
        public List<Location> locations { get; set; }
    }
}