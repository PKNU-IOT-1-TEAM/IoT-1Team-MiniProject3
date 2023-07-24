#include <ArduinoJson.h>
#include <DHT.h>
#include <pm2008_i2c.h>

//DFRobot_DHT11 DHT;
//#define AD1_HUM 11

int AD1_IR1_out = 6;
int AD1_LED1_RED = 2;
int AD1_LED1_GREEN = 3;
#define DHTPIN 9     // Digital pin connected to the DHT sensor
#define DHTTYPE DHT22   // DHT 22 (AM2302), AM2321

DHT dht(DHTPIN, DHTTYPE);
PM2008_I2C pm2008_i2c;

DynamicJsonDocument doc(128);

void setup() {
  // 온습도
  Serial.begin(9600);
  dht.begin();

  // 센서
  #ifdef PM2008N
    // wait for PM2008N to be changed to I2C mode
    delay(10000);
  #endif
  pm2008_i2c.begin();
  pm2008_i2c.command();
  delay(1000);

  pinMode(AD1_LED1_RED, OUTPUT);  // LED_BUILTIN
  pinMode(AD1_LED1_GREEN, OUTPUT);
  pinMode(AD1_IR1_out, INPUT);
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

  if (Serial.available())
  {
    char command = Serial.read();
    if(command == '0')
    {
      
      Serial.println("0받음");
      digitalWrite(AD1_IR1_out, HIGH);
      digitalWrite(AD1_LED1_RED, LOW); // LED를 켭니다.
      digitalWrite(AD1_LED1_GREEN, HIGH); // LED를 끕니다.
    }
    else if(command =='1')
    {
      Serial.println("1받음");
      digitalWrite(AD1_IR1_out, LOW);
      digitalWrite(AD1_LED1_RED, HIGH); // LED를 끕니다.
      digitalWrite(AD1_LED1_GREEN, LOW);  // LED를 켭니다.
    }
  }

  //DHT.read(AD1_HUM);

  int state = digitalRead(AD1_IR1_out);
  /*차량등록된 차면 초록색 아니면 빨간색--> json파일 읽어와서 led바뀌는걸로 코드 변경해야함
  if (state == LOW) {
    digitalWrite(AD1_IR1_out, HIGH);
    digitalWrite(AD1_LED1_RED, LOW); // LED를 켭니다.
  } else {
    digitalWrite(AD1_IR1_out, LOW);
    digitalWrite(AD1_LED1_RED, HIGH); // LED를 끕니다.
  }*/

  doc["AD1_RCV_IR_Sensor"] = state;
  doc["AD1_RCV_Temperature"] = t;
  doc["AD1_RCV_Humidity"] = h;
  doc["AD1_RCV_Dust"] = pm2008_i2c.number_of_2p5_um;

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);

  delay(10000);
}
