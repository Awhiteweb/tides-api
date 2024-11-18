using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Threading.Tasks;

namespace DailyTide
{
    public class SnsClient 
    {
        private readonly string TopicArn = Environment.GetEnvironmentVariable("TopicArn");
        private readonly AmazonSimpleNotificationServiceClient Client;

        public SnsClient(AmazonSimpleNotificationServiceClient client)
        {
            Client = client;
        }

        public async Task SendMessage(string message)
        {
            await Client.PublishAsync(new PublishRequest()
            {
                TopicArn = TopicArn,
                Message = message
            });
        }
    }
}