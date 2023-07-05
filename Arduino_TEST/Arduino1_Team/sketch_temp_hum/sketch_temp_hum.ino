#include <DFRobot_DHT11.h>
#include <ArduinoJson.h>
DFRobot_DHT11 DHT;
#define AD1_HUM 11

int AD1_IR_in = 5;
int AD1_IR_out = 6;
int AD1_LED = 10;

void setup() {
  
  pinMode(AD1_LED, OUTPUT);  // LED_BUILTIN
  pinMode(AD1_IR_in, INPUT);
  pinMode(AD1_IR_out, OUTPUT);
  Serial.begin(9600);
}

void loop() {

  DynamicJsonDocument doc(128);
  
  int state = digitalRead(AD1_IR_in);
  
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

  doc["IR_Sensor"]= int(state);
  doc["Temperature"] = int(DHT.temperature);
  doc["Humidity"] = int(DHT.humidity);
  
  serializeJson(doc, Serial);
  Serial.println("");
  
  delay(5000);
}
