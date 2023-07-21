// NFC header file
#include <SPI.h>
#include <MFRC522.h>
#include <ArduinoJson.h>

// Pin define
#define SS_PIN 10
#define RST_PIN 9
#define CNT_WATER 7
#define NCNT_WATER 5

MFRC522 rfid(SS_PIN, RST_PIN);
MFRC522::MIFARE_Key key; 

void setup() {
  // Serial speed
  Serial.begin(9600);
  pinMode(NCNT_WATER, INPUT);
  SPI.begin();
  rfid.PCD_Init();
}

void loop() {
  DynamicJsonDocument doc(128);
  
  int clevel = analogRead(CNT_WATER);
  int nlevel = digitalRead(NCNT_WATER);
  String cardNumber = "None";
  
  doc["AD4_RCV_NFC"] = cardNumber;
  doc["AD4_RCV_WL_CNNT"] = clevel;
  doc["AD4_RCV_WL_NCNNT"] = nlevel;

  if ( ! rfid.PICC_IsNewCardPresent())
  { 
    String jsonStr; 
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(10000);
    return;
  }
  if ( ! rfid.PICC_ReadCardSerial())
  {
    String jsonStr; 
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(10000);
    return;
  }
  cardNumber = "";

  for (byte i = 0; i < 4; i++) {    
    cardNumber += rfid.uid.uidByte[i];
  }

  doc["AD4_RCV_NFC"] = cardNumber;

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);
   
  rfid.PICC_HaltA();
  rfid.PCD_StopCrypto1();
  delay(10000);
}
