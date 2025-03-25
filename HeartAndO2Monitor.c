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

#define REPORTING_PERIOD_MS 1000
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

    // ✅ Set I²C Speed (for stability)
    Wire.begin(21, 22, 100000); // ✅ Use 100kHz speed

    // ✅ Initialize MAX30100 Sensor
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

    if (millis() - tsLastReport > REPORTING_PERIOD_MS) {
        sendSensorData();
        tsLastReport = millis();
    }
}

// ✅ Function to read and send sensor data
void sendSensorData() {
    int heartRate = (int)pox.getHeartRate();
    int bloodO2 = (int)pox.getSpO2();

    if (heartRate > 30 && bloodO2 > 70) {
        Serial.printf("💓 HR: %d BPM | O₂: %d%%\n", heartRate, bloodO2);
        sendDataToFirebase(heartRate, bloodO2);
    } else {
        Serial.println("❌ Invalid Readings. Skipping Firebase Update.");
    }
}

// ✅ Function to send data to Firebase
void sendDataToFirebase(int heartRate, int bloodO2) {
    FirebaseJson json;
    json.set("/heart/rate", heartRate);
    json.set("/blood/O2", bloodO2);

    if (Firebase.RTDB.updateNode(&fbdo, "/", &json)) {
        Serial.println("✅ Data Updated in Firebase!");
    } else {
        Serial.print("❌ Firebase Error: ");
        Serial.println(fbdo.errorReason());
    }
}