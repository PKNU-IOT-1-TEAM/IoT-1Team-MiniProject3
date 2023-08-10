// NFC header file
#include <SPI.h>    // spi통신을 위한 라이브러리
#include <MFRC522.h>
#include <ArduinoJson.h>

// Pin define
#define SS_PIN 10 // SDA 핀 번호
#define RST_PIN 9  // RST 핀 번호
// SCK 13, MOSI 11, MISO 12, IRQ 없음


MFRC522 rfid(SS_PIN, RST_PIN);      // MFRC522 라이브러리를 사용하여 NFC 모듈 객체 생성
MFRC522::MIFARE_Key key;            // MIFARE 카드 인증을 위한 키 객체

void setup() {
  // Serial speed
  Serial.begin(9600);
  SPI.begin();    // spi 통신 초기화
  rfid.PCD_Init();    // NFC 모듈 초기화
}

void loop() {
  DynamicJsonDocument doc(128);   // 128byte  크기의 동적 json 객체 생성
  
  int clevel = analogRead(A0);    // 물 높이를 아날로그 핀 (A0)에서 읽어옴
  String cardNumber = "None";     // 카드 번호를 저장할 변수 초기화
  
  doc["AD4_RCV_NFC"] = cardNumber;
  doc["AD4_RCV_WL_CNNT"] = clevel;

  // NFC 카드가 인식되지 않은 경우, json 객체를 시리얼 통신으로 전송하고 10초 대기
  if ( ! rfid.PICC_IsNewCardPresent())
  { 
    String jsonStr; 
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(1000);
    return;
  }
  // NFC 카드 UID를 읽어오지 못한 경우, json 객체를 시리얼 통신으로 전송하고 10초 대기
  if ( ! rfid.PICC_ReadCardSerial())
  {
    String jsonStr; 
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(1000);
    return;
  }
  cardNumber = "";      // 카드번호 초기화

  //  NFC 카드 UID를 문자열 형태로 변환하여 저장
  for (byte i = 0; i < 4; i++) {    
    cardNumber += rfid.uid.uidByte[i];
  }

  doc["AD4_RCV_NFC"] = cardNumber;

  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);
   
  rfid.PICC_HaltA();    // NFC 카드 통신 종료
  rfid.PCD_StopCrypto1();   // NFC 카드 암호화 종료
  delay(1000);   // 10초 대기
}
