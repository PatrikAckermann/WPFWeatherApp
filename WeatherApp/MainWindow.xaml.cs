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
using static WeatherApp.WeatherAPI;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string apiKey = "b1c8a5cea60f17f305ee2d9e3305af25";
        // List of locations from last search. Gets identified by list index
        Dictionary<int, locationObj.Location> searchLocationList = new Dictionary<int, locationObj.Location>(); 

        // List of saved locations. Gets loaded on program start. 
        Dictionary<int, locationObj.Location> savedLocationList = new Dictionary<int, locationObj.Location>();
        public MainWindow()
        {
            InitializeComponent();

            CoordinateObj coordinates = getCoordinates();
            setCurrentWeather(coordinates.lat, coordinates.lon);
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchLocations(citySearch, countrySearch);
        }

        private void searchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchListBox.SelectedItem != null)
            {
                openWeatherButton.IsEnabled = true;
                saveButton.IsEnabled = true;
            } else
            {
                openWeatherButton.IsEnabled = false;
                saveButton.IsEnabled = false;
            }
        }

        private void openWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            setCurrentWeather(searchLocationList[searchListBox.SelectedIndex].lat, searchLocationList[searchListBox.SelectedIndex].lon);
            tabControl.SelectedIndex = 0;
        }

        private void searchLocations(TextBox city, TextBox country)
        {
            searchListBox.Items.Clear();
            searchLocationList.Clear();
            if (city.Text != "")
            {
                var locations = LocationAPI.GetLocations(city.Text, country.Text, apiKey);
                if (locations != null)
                {
                    foreach (var l in locations)
                    {
                        searchListBox.Items.Add($"{l.name}, {l.country}");
                        searchLocationList.Add(searchListBox.Items.Count - 1, l);
                    }
                }
            }
        }

        public void setCurrentWeather(double latitude, double longitude)
        {
            string response = WeatherAPI.requestWeather(latitude, longitude);

            OWMObject.Root currentWeather = JsonConvert.DeserializeObject<OWMObject.Root>(response);
            Trace.WriteLine(response);
            placeLabel.Content = currentWeather.name;
            weatherLabel.Content = $"{kelvinToCelsius(currentWeather.main.temp)}°C, {currentWeather.weather[0].description}";
            sunriseLabel.Content = timestampToTime(currentWeather.sys.sunrise + currentWeather.timezone);
            sunsetLabel.Content = timestampToTime(currentWeather.sys.sunset + currentWeather.timezone);
            visibilityLabel.Content = $"{currentWeather.visibility}m";
            windspeedLabel.Content = $"{currentWeather.wind.speed}m/s";
            lowestTempLabel.Content = $"{kelvinToCelsius(currentWeather.main.temp_min)}°C";
            highestTempLabel.Content = $"{kelvinToCelsius(currentWeather.main.temp_max)}°C";
            lastUpdateLabel.Content = timestampToTime(currentWeather.dt);

            setIcon(currentWeather.weather[0].icon);
        }

        private void setIcon(string iconId)
        {
            weatherIcon.Source = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{iconId}@4x.png"));
        }
    }

    public class CoordinateObj
    {
        public bool success;
        public double lat { get; set; }
        public double lon { get; set; }
    }

}
