#include <ArduinoJson.h>
#include <DHT.h>
#include <pm2008_i2c.h>

//DFRobot_DHT11 DHT;
//#define AD1_HUM 11

int AD1_IR_out = 5;
int AD1_LED = 10;
#define DHTPIN 9     // Digital pin connected to the DHT sensor
#define DHTTYPE DHT22   // DHT 22 (AM2302), AM2321

DHT dht(DHTPIN, DHTTYPE);
PM2008_I2C pm2008_i2c;

DynamicJsonDocument doc(128);

void setup() {
  // 온습도
  Serial.begin(9600);
  Serial.println(F("DHTxx test!"));
  dht.begin();

  // 센서
#ifdef PM2008N
  // wait for PM2008N to be changed to I2C mode
  delay(10000);
#endif
  pm2008_i2c.begin();
  pm2008_i2c.command();
  delay(1000);

  pinMode(AD1_LED, OUTPUT);  // LED_BUILTIN
  pinMode(AD1_IR_out, INPUT);
}

void loop() {
  // Wait a few seconds between measurements.
  // 온습도
  delay(2000);

  // Reading temperature or humidity takes about 250 milliseconds!
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  float h = dht.readHumidity();
  // Read temperature as Celsius (the default)
  float t = dht.readTemperature();
  // Read temperature as Fahrenheit (isFahrenheit = true)
  float f = dht.readTemperature(true);

  // Check if any reads failed and exit early (to try again).
  if (isnan(h) || isnan(t) || isnan(f)) {
    return;
  }

  // Compute heat index in Fahrenheit (the default)
  float hif = dht.computeHeatIndex(f, h);
  // Compute heat index in Celsius (isFahreheit = false)
  float hic = dht.computeHeatIndex(t, h, false);

  // 센서
  uint8_t ret = pm2008_i2c.read();
  if (ret == 0) {
  }

  int state = digitalRead(AD1_IR_out);

  //DHT.read(AD1_HUM);

  if (state == LOW) {
    digitalWrite(AD1_IR_out, HIGH);
    digitalWrite(AD1_LED, LOW); // LED를 켭니다.
  } else {
    digitalWrite(AD1_IR_out, LOW);
    digitalWrite(AD1_LED, HIGH); // LED를 끕니다.
  }

  doc["IR_Sensor"] = state;
  doc["Temperature"] = t;
  doc["Humidity"] = h;
  doc["PM_1.0_GRIMM"] = pm2008_i2c.pm1p0_grimm;
  doc["PM_2.5_GRIMM"] = pm2008_i2c.pm2p5_grimm;
  doc["PM_10_GRIMM"] = pm2008_i2c.pm10_grimm;
  doc["PM_1.0_TSI"] = pm2008_i2c.pm1p0_tsi;
  doc["PM_2.5_TSI"] = pm2008_i2c.pm2p5_tsi;
  doc["PM_10_TSI"] = pm2008_i2c.pm10_tsi;
  doc["Number_of_0.3_um"] = pm2008_i2c.number_of_0p3_um;
  doc["Number_of_0.5_um"] = pm2008_i2c.number_of_0p5_um;
  doc["Number_of_1_um"] = pm2008_i2c.number_of_1_um;
  doc["Number_of_2.5_um"] = pm2008_i2c.number_of_2p5_um;
  doc["Number_of_5_um"] = pm2008_i2c.number_of_5_um;
  doc["Number_of_10_um"] = pm2008_i2c.number_of_10_um;

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);

  delay(10000);
}
