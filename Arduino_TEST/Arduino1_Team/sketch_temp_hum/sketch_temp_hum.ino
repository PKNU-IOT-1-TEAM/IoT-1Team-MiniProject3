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
