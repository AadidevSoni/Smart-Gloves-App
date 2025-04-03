#include <WiFi.h>
#include <Firebase_ESP_Client.h>
#include <Wire.h>
#include "MAX30100_PulseOximeter.h"

// ✅ WiFi Credentials
#define WIFI_SSID "Aadidev's S23 FE"
#define WIFI_PASSWORD "seatscammer"

// ✅ Firebase Configuration
#define API_KEY "AIzaSyCVSVEaQyqqoqCOdZwOcwqwepnOtyQPXlc"
#define DATABASE_URL "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app"
#define USER_EMAIL "aadidevbahrain@gmail.com"
#define USER_PASSWORD "Aadidev@123"

// ✅ Firebase & Sensor Objects
FirebaseData fbdo;
FirebaseAuth auth;
FirebaseConfig config;
PulseOximeter pox;

// ✅ Flex Sensor Pins
#define FLEX_32 32
#define FLEX_33 33
#define FLEX_34 34
#define FLEX_35 35
#define FLEX_25 25  

// ✅ Thresholds for Flex Sensor Detection
#define THRESHOLD_32 404
#define THRESHOLD_33 530
#define THRESHOLD_34 685
#define THRESHOLD_35 2000
#define THRESHOLD_25 2300  

// ✅ Timers
#define UPDATE_INTERVAL 1000  // Send data every 1 second
uint32_t lastUpdate = 0;
uint32_t tsLastReport = 0;

void onBeatDetected() {
    Serial.println("💓 Beat Detected!");
}

void setup() {
    Serial.begin(115200);
    delay(5000);  // ✅ Allow time for sensor to stabilize

    // ✅ Connect to WiFi
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

    // ✅ Initialize MAX30100 Sensor
    Wire.begin(21, 22, 100000);
    Serial.print("Initializing Pulse Oximeter...");
    if (!pox.begin()) {
        Serial.println("❌ FAILED");
        while (1);
    }
    Serial.println("✅ SUCCESS");
    pox.setOnBeatDetectedCallback(onBeatDetected);
}

void loop() {
    pox.update();  // ✅ Update sensor readings

    if (millis() - lastUpdate >= UPDATE_INTERVAL) {
        lastUpdate = millis();
        sendFlexSensorData();
    }

    if (millis() - tsLastReport >= UPDATE_INTERVAL) {
        tsLastReport = millis();
        sendPulseOximeterData();
    }
}

// ✅ Function to read and send Flex Sensor data
void sendFlexSensorData() {
    int val32 = analogRead(FLEX_32);
    int val33 = analogRead(FLEX_33);
    int val34 = analogRead(FLEX_34);
    int val35 = analogRead(FLEX_35);
    int val25 = analogRead(FLEX_25);  

    int NeedWater = (val32 < THRESHOLD_32 && val32 > 0) ? 1 : 0;
    int NeedFood = (val33 < THRESHOLD_33 && val33 > 0) ? 1 : 0;
    int NeedWashroom = (val34 < THRESHOLD_34 && val34 > 0) ? 1 : 0;
    int NeedHelp = (val35 < THRESHOLD_35 && val35 > 0) ? 1 : 0;
    int Emergency = (val25 < THRESHOLD_25 && val25 > 0) ? 1 : 0;  

    Serial.printf("NeedWater: %d | NeedFood: %d | NeedWashroom: %d | NeedHelp: %d | Emergency: %d\n", NeedWater, NeedFood, NeedWashroom, NeedHelp, Emergency);

    FirebaseJson json;
    json.set("/WearerStatus/NeedWater", NeedWater);
    json.set("/WearerStatus/NeedFood", NeedFood);
    json.set("/WearerStatus/NeedWashroom", NeedWashroom);
    json.set("/WearerStatus/NeedHelp", NeedHelp);
    json.set("/WearerStatus/Emergency", Emergency);  

    if (Firebase.RTDB.updateNode(&fbdo, "/", &json)) {
        Serial.println("✅ Flex Sensor Data Sent to Firebase!");
    } else {
        Serial.print("❌ Firebase Error: ");
        Serial.println(fbdo.errorReason());
    }
}

// ✅ Function to read and send Pulse Oximeter data
void sendPulseOximeterData() {
    int heartRate = (int)pox.getHeartRate();
    int bloodO2 = (int)pox.getSpO2();

    if (heartRate > 30 && bloodO2 > 70) {
        Serial.printf("💓 HR: %d BPM | O₂: %d%%\n", heartRate, bloodO2);
        FirebaseJson json;
        json.set("/heart/rate", heartRate);
        json.set("/blood/O2", bloodO2);

        if (Firebase.RTDB.updateNode(&fbdo, "/", &json)) {
            Serial.println("✅ Pulse Oximeter Data Updated in Firebase!");
        } else {
            Serial.print("❌ Firebase Error: ");
            Serial.println(fbdo.errorReason());
        }
    } else {
        Serial.println("❌ Invalid Readings. Skipping Firebase Update.");
    }
}