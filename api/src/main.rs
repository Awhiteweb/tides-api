// https://adevait.com/rust/deploying-rust-functions-on-aws-lambda

use rusoto_core::Region;
use rusoto_s3::{S3, S3Client};
use lambda_runtime::{error::HandlerError, lambda, Context};
use serde::{Deserialize, Serialize};
use std::error::Error;
use futures::executor;

mod s3;
use s3::GetObject;

#[derive(Deserialize, Serialize, Clone)]
#[serde(rename_all = "camelCase")]
struct LambdaRequest {
    key: String,
}

#[derive(Deserialize, Serialize, Clone)]
#[serde(rename_all = "camelCase")]
struct LambdaResponse {
    data: String,
}

fn main() -> Result<(), Box<dyn Error>> {
    lambda!(lambda_handler);
    Ok(())
}

fn lambda_handler(e: LambdaRequest, _c: Context) -> Result<LambdaResponse, HandlerError> {
    let client = S3Client::new(Region::EuWest1);
    let item_result: GetObject = executor::block_on( get_item(client, e.key) );
    match std::str::from_utf8(&item_result.buffer) {
        Ok(data) => Ok(LambdaResponse { data: data.to_owned() }),
        Err(e) => panic!("{}", e)
    }
}

async fn get_item(client: impl S3, key: String) -> GetObject {
    s3::get_object(client, key).await
}