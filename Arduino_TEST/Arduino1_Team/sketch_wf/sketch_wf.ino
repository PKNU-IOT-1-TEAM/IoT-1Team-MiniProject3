#include <DFRobot_DHT11.h>
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
DFRobot_DHT11 DHT;
#define AD1_HUM 11

int AD1_IR_in = 5;
int AD1_IR_out = 6;
int AD1_LED = 10;

const char* ssid = "iotzone3";
const char* password = "1234567890";
const char* mqtt_server = "192.168.0.103";
const char* mqtt_topic = "test";

WiFiClient espClient;
PubSubClient client(espClient);

void setup() {
  
  pinMode(AD1_LED, OUTPUT);  // LED_BUILTIN
  pinMode(AD1_IR_in, INPUT);
  pinMode(AD1_IR_out, OUTPUT);
  Serial.begin(9600);

  setupWiFi();
  setupMQTT();
}

void loop() {

  DynamicJsonDocument doc(128);
  
  int state = digitalRead(AD1_IR_in);
  
  DHT.read(AD1_HUM);
  
  if (state == LOW) 
  {
    digitalWrite(AD1_IR_out, HIGH);
    Serial.println("Infrared detected!");
    digitalWrite(AD1_LED, LOW); // LED를 켭니다.
    Serial.println("LED ON"); // 시리얼 모니터에 LED가 켜졌음을 출력합니다.
  } 
  else 
  {
    digitalWrite(AD1_IR_out, LOW);
    Serial.println("No infrared detected.");
    digitalWrite(AD1_LED, HIGH); // LED를 끕니다.
    Serial.println("LED OFF"); // 시리얼 모니터에 LED가 꺼졌음을 출력합니다.
  }

  doc["IR_Sensor"]= int(state);
  doc["Temperature"] = int(DHT.temperature);
  doc["humidity"] = int(DHT.humidity);
  
  serializeJson(doc, jsonString);
  Serial.println("");

  publishData(jsonString);
  
  delay(5000);
}

void setupWiFi()
{
    delay(10);
    Serial.println();
    Serial.print("Connectiong to ");
    Serial.println(ssid);
    WiFi.begin(ssid, password);

    while (WiFi.status() !=WL_CONNECTED)
    {
      delay(500);
      Serial.print(".");
    }

    Serial.println("");
    Serial.println("WiFi connected");
    Seiral.print("IP address: ");
    Serial.println(WiFi.localIP());
}

void setupMQTT()
{
    client.setServer(mqtt_server, 1883);
}

void publishData(const String& payload)
{
  if(client.connected())
  {
      client.publish(mqtt_topic, payload.c_str());
  }
}
