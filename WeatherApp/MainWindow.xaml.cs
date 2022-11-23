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
using System.Runtime.InteropServices;
using System.IO;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string apiKey = "b1c8a5cea60f17f305ee2d9e3305af25";
        // List of locations from last search. Gets identified by list index
        List<locationObj.Location> searchLocationList = new List<locationObj.Location>(); 

        // List of saved locations. Gets loaded on program start. 
        List<locationObj.Location> savedLocationList = new List<locationObj.Location>();
        public MainWindow()
        {
            InitializeComponent();

            CoordinateObj coordinates = getCoordinates();
            setCurrentWeather(coordinates.lat, coordinates.lon);
            loadList();
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

        private void saveToList()
        {
            if (!savedLocationList.Contains(searchLocationList[searchListBox.SelectedIndex]))
            {
                savedLocationList.Add(searchLocationList[searchListBox.SelectedIndex]);
                saveList();
                loadList();
            }
        }

        private void deleteFromList()
        {
            if (savedListBox.SelectedIndex != -1)
            {
                savedLocationList.RemoveAt(savedListBox.SelectedIndex);
                saveList();
                loadList();
            }
        }

        private void saveList()
        {
            string output = JsonConvert.SerializeObject(savedLocationList);
            Trace.WriteLine(output);
            File.WriteAllText("savedCities.json", output);
        }

        private void loadList()
        {
            if (File.Exists("savedCities.json"))
            {
                savedListBox.Items.Clear();
                string output = File.ReadAllText("savedCities.json");
                savedLocationList = JsonConvert.DeserializeObject<List<locationObj.Location>>(output);
                foreach (var l in savedLocationList)
                {
                    Trace.WriteLine($"{l.lat}, {l.lon}, {l.name}");
                    addToSavedListBox(l.name, l.country);
                }

            }
        }

        private void addToSavedListBox(string city, string country)
        {
            savedListBox.Items.Add($"{city}, {country}");
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
                deleteButton.IsEnabled = false;
                savedListBox.UnselectAll();
            }
        }

        private void savedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (savedListBox.SelectedItem != null)
            {
                openWeatherButton.IsEnabled = true;
                saveButton.IsEnabled = false;
                deleteButton.IsEnabled = true;
                searchListBox.UnselectAll();
            }
        }

        private void openWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            double lat = 0;
            double lon = 0;
            if (searchListBox.SelectedIndex != -1)
            {
                lat = searchLocationList[searchListBox.SelectedIndex].lat;
                lon = searchLocationList[searchListBox.SelectedIndex].lon;
            } else if (savedListBox.SelectedIndex != -1)
            {
                lat = savedLocationList[savedListBox.SelectedIndex].lat;
                lon = savedLocationList[savedListBox.SelectedIndex].lon;
            }
            setCurrentWeather(lat, lon);
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
                        searchLocationList.Add(l);
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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveToList();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            deleteFromList();
        }
    }

    public class CoordinateObj
    {
        public bool success;
        public double lat { get; set; }
        public double lon { get; set; }
    }

}
