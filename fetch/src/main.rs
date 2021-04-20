// https://adevait.com/rust/deploying-rust-functions-on-aws-lambda

use rusoto_core::Region;
use rusoto_s3::{S3, S3Client};
use lambda_runtime::{error::HandlerError, lambda, Context};
use std::error::Error;
use serde::{Deserialize, Serialize};
use futures::executor;
use chrono::{Utc, Datelike};

mod s3;
mod tides;

struct LambdaRequest;
struct LambdaResponse;

fn main() -> Result<(), Box<dyn Error>> {
    lambda!(lambda_handler);
    Ok(())
}

fn lambda_handler(_e: LambdaRequest, _c: Context) -> Result<LambdaResponse, HandlerError> {
    let client = S3Client::new(Region::EuWest1);
    executor::block_on( process_work(client) );
    Ok(LambdaResponse {})
}

async fn process_work(client: impl S3) {
    // let locations = tides::admiralty_locations().await.unwrap();
    // put_item(client, "tides/locations.json".to_owned(), locations).await;
    let tides = tides::admiralty_tidal_events(tides::bosham_location_id(), 7).await.expect("To have had some tide data returned");
    let now = Utc::now();
    let key = format!("tides/{year}/{month}/{day}", year = now.year(), month = now.month0(), day = now.day0());
    s3::put_object(client, key, tides).await;
}