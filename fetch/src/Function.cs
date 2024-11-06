using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DailyTide
{
    public class Function
    {
        private readonly HttpClient ApiClient;
        private readonly AmazonS3Client S3;
        private readonly AmazonSimpleNotificationServiceClient SNS;

        public Function()
        {
            ApiClient = new HttpClient();
            S3 = new AmazonS3Client();
            SNS = new AmazonSimpleNotificationServiceClient();
        }
        
        /// <summary>
        /// A simple function that takes an input from a scheduled event and stores the result in S3
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(InputRequest input, ILambdaContext context)
        {
            var app = new App(ApiClient, S3, SNS);
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
            return await new Tides( ApiClient ).GetTideEvents( input.LocationId );
        }
    }
}
