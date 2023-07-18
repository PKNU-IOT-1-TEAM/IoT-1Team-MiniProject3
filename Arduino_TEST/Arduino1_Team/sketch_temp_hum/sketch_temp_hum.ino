//#include <DFRobot_DHT11.h>
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
    Serial.println(F("Failed to read from DHT sensor!"));
    return;
  }

  // Compute heat index in Fahrenheit (the default)
  float hif = dht.computeHeatIndex(f, h);
  // Compute heat index in Celsius (isFahreheit = false)
  float hic = dht.computeHeatIndex(t, h, false);

  Serial.print(F("Humidity: "));
  Serial.print(h);
  Serial.print(F("%  Temperature: "));
  Serial.print(t);
  Serial.print(F("°C "));
  Serial.print(f);
  Serial.print(F("°F  Heat index: "));
  Serial.print(hic);
  Serial.print(F("°C "));
  Serial.print(hif);
  Serial.println(F("°F"));

  // 센서
  uint8_t ret = pm2008_i2c.read();
  if (ret == 0) {
    Serial.print("PM 1.0 (GRIMM) : ");
    Serial.println(pm2008_i2c.pm1p0_grimm);
    Serial.print("PM 2.5 (GRIMM) : : ");
    Serial.println(pm2008_i2c.pm2p5_grimm);
    Serial.print("PM 10 (GRIMM) : : ");
    Serial.println(pm2008_i2c.pm10_grimm);
    Serial.print("PM 1.0 (TSI) : ");
    Serial.println(pm2008_i2c.pm1p0_tsi);
    Serial.print("PM 2.5 (TSI) : : ");
    Serial.println(pm2008_i2c.pm2p5_tsi);
    Serial.print("PM 10 (TSI) : : ");
    Serial.println(pm2008_i2c.pm10_tsi);
    Serial.print("Number of 0.3 um : ");
    Serial.println(pm2008_i2c.number_of_0p3_um);
    Serial.print("Number of 0.5 um : ");
    Serial.println(pm2008_i2c.number_of_0p5_um);
    Serial.print("Number of 1 um : ");
    Serial.println(pm2008_i2c.number_of_1_um);
    Serial.print("Number of 2.5 um : ");
    Serial.println(pm2008_i2c.number_of_2p5_um);
    Serial.print("Number of 5 um : ");
    Serial.println(pm2008_i2c.number_of_5_um);
    Serial.print("Number of 10 um : ");
    Serial.println(pm2008_i2c.number_of_10_um);
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

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);

  delay(10000);
}


//

#include <DFRobot_DHT11.h>
#include <ArduinoJson.h>
DFRobot_DHT11 DHT;
#define AD1_HUM 11

int AD1_IR_out = 5;
int AD1_LED = 10;

DynamicJsonDocument doc(128);

void setup() {
  
  pinMode(AD1_LED, OUTPUT);  // LED_BUILTIN
  pinMode(AD1_IR_out, INPUT);
  Serial.begin(9600);
}

void loop() {
  int state = digitalRead(AD1_IR_out);
  
  DHT.read(AD1_HUM);
  
  if (state == LOW) 
  {
    digitalWrite(AD1_IR_out, HIGH);
    digitalWrite(AD1_LED, LOW); // LED를 켭니다.
  } 
  else 
  {
    digitalWrite(AD1_IR_out, LOW);
    digitalWrite(AD1_LED, HIGH); // LED를 끕니다.
  }

  doc["IR_Sensor"]= state;
  doc["Temperature"] = DHT.temperature;
  doc["Humidity"] = DHT.humidity;

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);

  delay(10000);
}
