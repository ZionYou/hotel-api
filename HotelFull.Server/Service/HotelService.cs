using System.Xml.Linq;



namespace HotelAPI.Service
{
    public class HotelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // Constructor with IHttpClientFactory for better HttpClient handling
        public HotelService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["GoogleApiKey"];  // 使用 IConfiguration 來讀取 API Key

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentNullException("GoogleApiKey", "Google API key is missing in the configuration.");
            }
        }

        public async Task<(decimal lat, decimal lng)?> GeocodeAsync(string address)
        {
            try
            {

                // Construct the request URL
                var requestUrl = $"https://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&key={_apiKey}";

                // Send the HTTP request
                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    // Log unsuccessful status code and return null
                    Console.WriteLine("Geocode request failed with status code: " + response.StatusCode);
                    return null;
                }

                // Parse the XML response
                var xmlContent = await response.Content.ReadAsStringAsync();
                var xmlDoc = XDocument.Parse(xmlContent);
                var status = xmlDoc.Root?.Element("status")?.Value;

                if (status == "OK")
                {
                    var locationElement = xmlDoc.Root?.Element("result")?.Element("geometry")?.Element("location");
                    if (locationElement != null)
                    {
                        var latElement = locationElement.Element("lat")?.Value;
                        var lngElement = locationElement.Element("lng")?.Value;

                        if (decimal.TryParse(latElement, out var lat) && decimal.TryParse(lngElement, out var lng))
                        {
                            return (lat, lng);  // Return the latitude and longitude
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Geocode request failed with status: {status}");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine("HTTP request error: " + httpEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching geocode data: " + ex.Message);
            }

            return null; // Return null if geocoding fails
        }

    }
}
