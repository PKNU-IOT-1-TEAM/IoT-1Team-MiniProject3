#include <SPI.h>
#include <MFRC522.h>
#include <ArduinoJson.h>

#define SS_PIN 10
#define RST_PIN 9

#define PIEZO 7
#define INA 2
#define INB 3
#define LIQUID 5

int Liquid_level = 0;

MFRC522 mfrc522(SS_PIN, RST_PIN); 

void setup(){
  pinMode(INA, OUTPUT);
  pinMode(INB, OUTPUT);
  pinMode(PIEZO, OUTPUT);
  pinMode(LIQUID, INPUT);
  
  Serial.begin(9600);
  SPI.begin(); // Init SPI bus
  mfrc522.PCD_Init(); // Init MFRC522 
}

void loop(){
  StaticJsonDocument<200> output;

  output["cardNumber"] = "";
  output["liquid"] = -1;
  
  if (Serial.available()){
    DynamicJsonDocument doc(128);
    DeserializationError error = deserializeJson(doc, Serial);
    if (!error) {
      int buzzer = doc["buzzer"];
      int fan = doc["fan"];

      if (buzzer == 1){
        tone(PIEZO, 500, 1000);
      }
      if (fan == 1){
        digitalWrite(INA, HIGH);
        digitalWrite(INB, LOW);
        delay(1000);
      } 
    }
  }
  
  if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial())
  {
     String cardNumber = "";
     for (byte i = 0; i < mfrc522.uid.size; i++){
     cardNumber += String(mfrc522.uid.uidByte[i], HEX);
     }
     output["cardNumber"] = cardNumber;
   }
   
   Liquid_level = digitalRead(LIQUID);
   output["liquid"] = Liquid_level;
   
   String message;
   serializeJson(output, message);
   Serial.println(message);

   delay(1000);
}
