// https://www.admiralty.co.uk/digital-services/data-solutions/uk-tidal-api

use reqwest::{Client, header};

pub fn bosham_location_id() -> String {
    std::env::var("BoshamId").expect("BoshamId should be available as an environment variable")
}
fn subscription_key() -> String {
    std::env::var("SubscriptionKey").expect("Subscription key should be available as an environment variable")
}

fn admiralty_stations_url() -> String {
    "https://admiraltyapi.azure-api.net/uktidalapi/api/V1/Stations".to_owned()
}

fn admiralty_tidal_events_url(station_id: String, duration: i32) -> String {
    format!("https://admiraltyapi.azure-api.net/uktidalapi/api/V1/Stations/{stationId}/TidalEvents?{duration}", 
        stationId = station_id, 
        duration = duration)
}

fn admiralty_headers() -> header::HeaderMap {
    let mut headers = header::HeaderMap::new();
    let mut auth_value = header::HeaderValue::from_str(&subscription_key()).unwrap();
    auth_value.set_sensitive(true);
    headers.insert("Ocp-Apim-Subscription-Key", auth_value);
    headers
}

pub async fn admiralty_locations() -> reqwest::Result<String> {
    let client = Client::builder().default_headers(admiralty_headers()).build()?;
    let response = client.get(&admiralty_stations_url()).send().await?;
    response.text().await
}

pub async fn admiralty_location(id: String) -> reqwest::Result<String> {
    let client = Client::builder().default_headers(admiralty_headers()).build()?;
    let url = format!("{domain}?{stationId}", domain = &admiralty_stations_url(), stationId = id);
    let response = client.get(url).send().await?;
    response.text().await
}

pub async fn admiralty_tidal_events(id: String, duration: i32) -> reqwest::Result<String> {
    let client = Client::builder().default_headers(admiralty_headers()).build()?;
    let response = client.get(&admiralty_tidal_events_url(id, duration)).send().await?;
    response.text().await
}