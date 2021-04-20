use rusoto_s3::{S3, PutObjectRequest, PutObjectOutput};

pub async fn put_object(client: impl S3, key: String, body: String) -> PutObjectOutput {
    let mut input: PutObjectRequest = Default::default();
    input.key = key;
    input.bucket = std::env::var("Bucket").unwrap();
    input.body = Some(body.into_bytes().into());
    client.put_object(input).await.unwrap()
}