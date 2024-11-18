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
            this.Client = client;
        }

        public async Task<string> GetTideEvents(string locationId)
        {
            this.Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
            var request = $"{this.StationsUrl}/{locationId}/TidalEvents?7";
            Console.WriteLine($"making request to {request}");
            var response = await this.Client.GetAsync(request);
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
            this.Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
            var request = $"{this.StationsUrl}";
            Console.WriteLine($"making request to {request}");
            var response = await this.Client.GetAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine( response.StatusCode );
                throw new Exception("error with api request");
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}