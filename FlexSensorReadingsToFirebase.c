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
#define FLEX_26 26
#define FLEX_33 33
#define FLEX_34 34
#define FLEX_35 35

// ✅ Thresholds
#define THRESHOLD_26 1880
#define THRESHOLD_33 200
#define THRESHOLD_34 1890
#define THRESHOLD_35 1600

// ✅ Button Pin
#define BUTTON_PIN 4
bool lastButtonState = HIGH;
bool emergencyState = false; // false = no emergency

// ✅ Timing
uint32_t lastUpdate = 0;
#define UPDATE_INTERVAL 100

bool waiting = false;
String waitingKey = "";

void setup() {
  Serial.begin(115200);
  pinMode(BUTTON_PIN, INPUT);  // Using external pull-up resistor

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

  // ✅ Set default Emergency to 0
  Firebase.RTDB.setInt(&fbdo, "/WearerStatus/Emergency", 0);

  Serial.println("✅ Ready to send sensor data to Firebase!");
}

void loop() {
  handleButton();  // 🔘 Check button

  if (!waiting && millis() - lastUpdate >= UPDATE_INTERVAL) {
    lastUpdate = millis();
    sendFlexSensorData();
    delay(500);
  }

  if (waiting) {
    checkIfReset();
    delay(1000);
  }
}

// ✅ Track Actual Button State (Pressed = 1, Released = 0)
void handleButton() {
  bool currentButtonState = digitalRead(BUTTON_PIN);

  // Only update Firebase when state changes
  if (currentButtonState != lastButtonState) {
    lastButtonState = currentButtonState;

    emergencyState = (currentButtonState == LOW);  // Pressed = LOW → true
    Firebase.RTDB.setInt(&fbdo, "/WearerStatus/Emergency", emergencyState ? 1 : 0);

    if (emergencyState) {
      Serial.println("🚨 Emergency Button Pressed!");
    } else {
      Serial.println("✅ Emergency Button Released!");
    }

    delay(50); // debounce
  }
}


// ✅ Send Flex Sensor Data
void sendFlexSensorData() {
  int val26 = analogRead(FLEX_26);
  int val33 = analogRead(FLEX_33);
  int val34 = analogRead(FLEX_34);
  int val35 = analogRead(FLEX_35);

  int NeedWater = (val26 < THRESHOLD_26 && val26 > 0) ? 1 : 0;
  int NeedFood = (val33 < THRESHOLD_33 && val33 > 0) ? 1 : 0;
  int NeedWashroom = (val34 < THRESHOLD_34 && val34 > 0) ? 1 : 0;
  int NeedHelp = (val35 < THRESHOLD_35 && val35 > 0) ? 1 : 0;

  Serial.printf("NeedWater: %d | NeedFood: %d | NeedWashroom: %d | NeedHelp: %d\n",
                NeedWater, NeedFood, NeedWashroom, NeedHelp);

  FirebaseJson json;
  json.set("NeedWater", NeedWater);
  json.set("NeedFood", NeedFood);
  json.set("NeedWashroom", NeedWashroom);
  json.set("NeedHelp", NeedHelp);

  // ✅ Only update inside /WearerStatus
  if (Firebase.RTDB.updateNode(&fbdo, "/WearerStatus", &json)) {
    Serial.println("✅ Sensor Data Sent!");

    if (NeedWater == 1) {
      waiting = true;
      waitingKey = "/WearerStatus/NeedWater";
    } else if (NeedFood == 1) {
      waiting = true;
      waitingKey = "/WearerStatus/NeedFood";
    } else if (NeedWashroom == 1) {
      waiting = true;
      waitingKey = "/WearerStatus/NeedWashroom";
    } else if (NeedHelp == 1) {
      waiting = true;
      waitingKey = "/WearerStatus/NeedHelp";
    }

    if (waiting) {
      Serial.printf("⏸️ Paused reading. Waiting for %s to become 0...\n", waitingKey.c_str());
    }

  } else {
    Serial.print("❌ Firebase Error: ");
    Serial.println(fbdo.errorReason());
  }
}

// ✅ Check for Reset
void checkIfReset() {
  if (Firebase.RTDB.getInt(&fbdo, waitingKey)) {
    int val = fbdo.intData();
    Serial.printf("🔁 Checking %s: %d\n", waitingKey.c_str(), val);
    if (val == 0) {
      Serial.println("✅ Flag cleared. Resuming readings...");
      waiting = false;
      waitingKey = "";
    }
  } else {
    Serial.print("❌ Firebase Check Error: ");
    Serial.println(fbdo.errorReason());
  }
}
