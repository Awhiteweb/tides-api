using Amazon.S3;
using Amazon.SimpleNotificationService;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DailyTide
{
    public class App
    {
        private readonly string DefaultLocationId = Environment.GetEnvironmentVariable("DefaultLocationId");
        private readonly Tides TidesApi;
        private readonly S3Client StorageApi;

        private readonly SnsClient SnsClient;

        public App(HttpClient apiClient, AmazonS3Client s3, AmazonSimpleNotificationServiceClient sns)
        {
            TidesApi = new Tides( apiClient );
            StorageApi = new S3Client( s3 );
            SnsClient = new SnsClient( sns );
        }

        public async Task Run(string input)
        {
            var locationId = string.IsNullOrEmpty(input) ? DefaultLocationId : input;
            var today = DateTime.UtcNow;
            using var s = await TidesApi.GetTideEvents(locationId);
            var key = $"{today:yyyy}/{today:MM}/{today:dd}/{locationId}";
            await StorageApi.PutObject(key, s);
            
            if( DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday )
            {
                var tides = await JsonSerializer.DeserializeAsync<AdmiraltyTide[]>(s);
                var suitableTide = tides.FirstOrDefault(t => t.EventType.Equals("HighWater", StringComparison.InvariantCulture) 
                                                          && t.DateTime.DayOfWeek > DayOfWeek.Monday
                                                          && t.DateTime.DayOfWeek < DayOfWeek.Saturday
                                                          && t.DateTime.Hour > 18 
                                                          && t.DateTime.Hour < 22);
                if(suitableTide != null)
                {
                    // notify that there is a suitable tide after today
                    var message = $"Tide height on {suitableTide.DateTime.DayOfWeek} {GetDaySuffix(suitableTide.DateTime.Day)} is {double.Round(suitableTide.Height,2)}";
                    await SnsClient.SendMessage(message);
                }
            }
        }

        public async Task GetLocations()
        {
            var today = DateTime.UtcNow;
            using var s = await TidesApi.GetLocations();
            var key = $"{today:yyyy}/locations";
            await StorageApi.PutObject(key, s);
        }

        private static string GetDaySuffix(int day) => day switch
        {
            1 or 21 or 31 => $"{day}st",
            2 or 22 => $"{day}nd",
            3 or 23 => $"{day}rd",
            _ => $"{day}th",
        };
    }
}