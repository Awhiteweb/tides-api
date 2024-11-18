using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DailyTide
{
    public class Tides
    {
        private readonly string SubscriptionKey = Environment.GetEnvironmentVariable("SubscriptionKey");
        private readonly string StationsUrl = "https://admiraltyapi.azure-api.net/uktidalapi/api/V1/Stations";

        private readonly HttpClient Client;

        public Tides(HttpClient client) 
        {
            Client = client;
        }

        public async Task<string> GetTideEvents(string locationId)
        {
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            var request = $"{StationsUrl}/{locationId}/TidalEvents?7";
            Console.WriteLine($"making request to {request}");
            var response = await Client.GetAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine( response.StatusCode );
                Console.Error.WriteLine( await response.Content.ReadAsStringAsync() );
                throw new Exception("error with api request");
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetLocations()
        {
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            var request = $"{StationsUrl}";
            Console.WriteLine($"making request to {request}");
            var response = await Client.GetAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine( response.StatusCode );
                throw new Exception("error with api request");
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}