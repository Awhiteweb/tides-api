using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DailyTide
{
    public class S3Client 
    {
        private readonly string Bucket = Environment.GetEnvironmentVariable("Bucket");
        private readonly AmazonS3Client Client;

        public S3Client(AmazonS3Client client) 
        {
            Client = client;
        }

        public async Task PutObject(string key, Stream body)
        {
            var request = new PutObjectRequest {
                BucketName = Bucket,
                Key = key,
                InputStream = body,
		        CannedACL = S3CannedACL.PublicRead
            };
            await Client.PutObjectAsync(request);
        }
    }
}
