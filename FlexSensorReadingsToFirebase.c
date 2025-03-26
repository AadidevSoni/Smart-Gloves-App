#include <WiFi.h>
#include <Firebase_ESP_Client.h>

// ✅ WiFi Credentials
#define WIFI_SSID "Aadidev's S23 FE"
#define WIFI_PASSWORD "seatscammer"

// ✅ Firebase Configuration
#define API_KEY "AIzaSyCVSVEaQyqqoqCOdZwOcwqwepnOtyQPXlc"
#define DATABASE_URL "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app"
#define USER_EMAIL "aadidevbahrain@gmail.com"
#define USER_PASSWORD "Aadidev@123"

// ✅ Firebase Objects
FirebaseData fbdo;
FirebaseAuth auth;
FirebaseConfig config;

// ✅ Flex Sensor Pins
#define FLEX_32 32
#define FLEX_33 33
#define FLEX_34 34
#define FLEX_35 35

// ✅ Thresholds for detection
#define THRESHOLD_32 404
#define THRESHOLD_33 530
#define THRESHOLD_34 685
#define THRESHOLD_35 2000

uint32_t lastUpdate = 0;
#define UPDATE_INTERVAL 1000 // ✅ Send data every 1 second

void setup() {
    Serial.begin(115200);
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    Serial.print("Connecting to WiFi");

    while (WiFi.status() != WL_CONNECTED) {
        Serial.print(".");
        delay(500);
    }
    Serial.println("\n✅ Connected to WiFi!");

    // ✅ Firebase Setup
    config.api_key = API_KEY;
    config.database_url = DATABASE_URL;
    auth.user.email = USER_EMAIL;
    auth.user.password = USER_PASSWORD;
    Firebase.begin(&config, &auth);
    Firebase.reconnectWiFi(true);

    Serial.println("✅ Ready to send sensor data to Firebase!");
}

void loop() {
    if (millis() - lastUpdate >= UPDATE_INTERVAL) {
        lastUpdate = millis();
        sendFlexSensorData();
    }
}

// ✅ Read Flex Sensor & Send to Firebase
void sendFlexSensorData() {
    int val32 = analogRead(FLEX_32);
    int val33 = analogRead(FLEX_33);
    int val34 = analogRead(FLEX_34);
    int val35 = analogRead(FLEX_35);

    int NeedWater = (val32 < THRESHOLD_32 && val32>0) ? 1 : 0;
    int NeedFood = (val33 < THRESHOLD_33 && val33>0) ? 1 : 0;
    int NeedWashroom = (val34 < THRESHOLD_34 && val34>0) ? 1 : 0;
    int NeedHelp = (val35 < THRESHOLD_35 && val35>0) ? 1 : 0;

    // ✅ Print values in Serial Monitor
    Serial.printf("NeedWater: %d | NeedFood: %d | NeedWashroom: %d | NeedHelp: %d\n", NeedWater, NeedFood, NeedWashroom, NeedHelp);

    // ✅ Create JSON data
    FirebaseJson json;
    json.set("/WearerStatus/NeedWater", NeedWater);
    json.set("/WearerStatus/NeedFood", NeedFood);
    json.set("/WearerStatus/NeedWashroom", NeedWashroom);
    json.set("/WearerStatus/NeedHelp", NeedHelp);

    // ✅ Send Data to Firebase (PATCH to update without deleting other data)
    if (Firebase.RTDB.updateNode(&fbdo, "/", &json)) {
        Serial.println("✅ Data Sent to Firebase!");
    } else {
        Serial.print("❌ Firebase Error: ");
        Serial.println(fbdo.errorReason());
    }
}
