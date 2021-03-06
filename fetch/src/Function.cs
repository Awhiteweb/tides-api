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
        /// A simple function that takes an input from a scheduled event and stores the result in S3
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(InputRequest input, ILambdaContext context)
        {
            var app = new App(this.ApiClient, this.S3);
            if(input.LocationId == "all")
            {
                await app.GetLocations();
                return;
            }
            await app.Run(input.LocationId);
        }

        /// <summary>
        /// An async function handler that is triggered from Api Gateway where the client is requesting a new set of tide events for a specific location
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Stream> ResponseFunctionHandlerAsync(InputRequest input, ILambdaContext context)
        {
            return await new Tides( this.ApiClient ).GetTideEvents( input.LocationId );
        }
    }
}
