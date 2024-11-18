using System.Threading.Tasks;
using System.Net.Http;
using Amazon.S3;
using System;

namespace DailyTide
{
    public class App
    {
        private readonly string DefaultLocationId = Environment.GetEnvironmentVariable("DefaultLocationId");
        private readonly Tides TidesApi;
        private readonly S3Client StorageApi;

        public App(HttpClient apiClient, AmazonS3Client s3)
        {
            this.TidesApi = new Tides( apiClient );
            this.StorageApi = new S3Client( s3 );
        }

        public async Task Run(string input)
        {
            var locationId = string.IsNullOrEmpty(input) ? this.DefaultLocationId : input;
            var today = DateTime.UtcNow;
            var s = await this.TidesApi.GetTideEvents(locationId);
            var key = $"{today.ToString("yyyy")}/{today.ToString("MM")}/{today.ToString("dd")}/{locationId}";
            await this.StorageApi.PutObject(key, s);
        }

        public async Task GetLocations()
        {
            var today = DateTime.UtcNow;
            var s = await this.TidesApi.GetLocations();
            var key = $"{today.ToString("yyyy")}/locations";
            await this.StorageApi.PutObject(key, s);
        }
    }
}