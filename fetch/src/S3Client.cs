using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;

namespace DailyTide
{
    public class S3Client 
    {
        private readonly string Bucket = Environment.GetEnvironmentVariable("Bucket");
        private readonly AmazonS3Client Client;

        public S3Client(AmazonS3Client client) 
        {
            this.Client = client;
        }

        public async Task PutObject(string key, Stream body)
        {
            var request = new PutObjectRequest {
                BucketName = this.Bucket,
                Key = key,
                InputStream = body,
		        CannedAcl = S3CannedACL.PublicRead
            };
            await this.Client.PutObjectAsync(request);
        }
    }
}
