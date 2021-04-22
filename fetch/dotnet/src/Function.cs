using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DailyTide
{
    public class Function
    {
        private readonly HttpClient ApiClient;
        private readonly AmazonS3Client S3;

        public Function()
        {
            this.ApiClient = new HttpClient();
            this.S3 = new AmazonS3Client();
        }
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            return await new App(this.ApiClient, this.S3).Run(input);
        }
    }

    public class App
    {
        private readonly string DefaultLocationId = Environment.GetEnvironmentVariable("DefaultLocationId");
        private readonly HttpClient ApiClient;
        private readonly AmazonS3Client S3;

        public App(HttpClient apiClient, AmazonS3Client s3)
        {
            this.ApiClient = apiClient;
            this.S3 = s3;
        }

        public async Task<string> Run(string input)
        {
            var locationId = string.IsNullOrEmpty(input) ? this.DefaultLocationId : input;
            var api = new Tides(this.ApiClient);
            var s3 = new S3Client(this.S3);
            var today = DateTime.UtcNow;
            using( var s = await api.GetTideEvents(locationId) )
            {
                var key = $"{today.ToString("yyyy")}/{today.ToString("MM")}/{today.ToString("dd")}/{locationId}";
                await s3.PutObject(key, s);
                if(string.IsNullOrEmpty(input)) {
                    return "tides saved";
                }
                using( var reader = new StreamReader(s, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
