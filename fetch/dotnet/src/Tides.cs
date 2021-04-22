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

        public async Task<Stream> GetTideEvents(string locationId)
        {
            this.Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
            var response = await this.Client.GetAsync($"{this.StationsUrl}/{locationId}/TidalEvents?7");
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("error with api request");
            }
            return await response.Content.ReadAsStreamAsync();
        }
    }
}