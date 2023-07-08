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
  
  doc["NFC"] = cardNumber;
  doc["WL_CNNT"] = clevel;
  doc["WL_NCNNT"] = nlevel;

  if ( ! rfid.PICC_IsNewCardPresent())
  { 
    serializeJson(doc, Serial);
    Serial.println("");
    delay(10000);
    return;
  }
  if ( ! rfid.PICC_ReadCardSerial())
  {
    serializeJson(doc, Serial);
    Serial.println("");
    delay(10000);
    return;
  }
  cardNumber = "";

  for (byte i = 0; i < 4; i++) {    
    cardNumber += rfid.uid.uidByte[i];
  }

  doc["NFC"] = cardNumber;
  
  serializeJson(doc, Serial);
  Serial.println("");
   
  rfid.PICC_HaltA();
  rfid.PCD_StopCrypto1();
  delay(10000);
}
