using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CoordinateObj coordinates = getCoordinates();
            WeatherAPI.setCurrentWeather(coordinates.lat, coordinates.lon, placeLabel, weatherLabel, sunriseLabel, sunsetLabel, visibilityLabel, windspeedLabel, lowestTempLabel, highestTempLabel, lastUpdateLabel, weatherIcon);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private CoordinateObj getCoordinates()
        {
            var client = new RestClient("http://ip-api.com/json/");
            var request = new RestRequest();
            var response = client.Get(request);
            if (response.IsSuccessful)
            {
                CoordinateObj coordinates = JsonConvert.DeserializeObject<CoordinateObj>(response.Content);
                coordinates.success = true;
                return coordinates;
            }
            CoordinateObj coordinateObj = new CoordinateObj();
            coordinateObj.success = false;
            return coordinateObj;
        }
    }

    public class CoordinateObj
    {
        public bool success;
        public double lat { get; set; }
        public double lon { get; set; }
    }

}
