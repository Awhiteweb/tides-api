use rusoto_s3::{S3, GetObjectRequest, StreamingBody};
use std::io::Read;

pub async fn get_object(client: impl S3, key: String) -> GetObject {
    let mut input: GetObjectRequest = Default::default();
    input.bucket = std::env::var("Bucket").unwrap();
    input.key = key;
    let get_object_result = client
        .get_object(input)
        .await
        .expect("couldn't get object from S3");
    let stream: StreamingBody = get_object_result.body.unwrap();
    let mut buf: Vec<u8> = Vec::new();
    let total_read = stream
        .into_blocking_read()
        .read_to_end(&mut buf)
        .unwrap();
    GetObject {
        buffer: buf, 
        size: total_read
    }
}

pub struct GetObject {
    pub buffer: Vec<u8>,
    pub size: usize,
}
