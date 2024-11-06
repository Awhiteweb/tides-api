using Amazon;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using DailyTide;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var app = new App(new HttpClient(), new AmazonS3Client(RegionEndpoint.EUWest1), new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUWest1));

await app.Run("0068");
