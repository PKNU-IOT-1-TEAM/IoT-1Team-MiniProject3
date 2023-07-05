#include <ArduinoJson.h>
#include <Servo.h>           // Servo 라이브러리 헥사 선언

Servo MyServo;              // 서보모터 선언
int pos = 190;                 // 모터 위치를 확인하기 위해 변수를 선언
int Fan_In = 3;
int Fan_Out = 5;
int CGuard = 6;          // 모터 제어를 위해 6번핀(PWM) 으로 선언
int CGuard_LED = 7;      // 서보모터 LED 핀 선언
int Buzz = 8;             // 부저 핀 선언
int Ras_Signal_Input = 9;   // 라즈베리파이 인풋 선언
int AD2_RCV_CGuard = 10;  // MQTT에 보낼 Servo 신호
int IR = 11;     // 차량 인식을 위한 IR 센서

void setup() {
  Serial.begin(9600);             // 시리얼 통신 9600
  pinMode(CGuard_LED, OUTPUT);    // 서보 모터 LED 핀 출력으로 설정 
  pinMode(CGuard, OUTPUT);        // 서보 모터 핀 출력으로 설정
  pinMode(Fan_In, OUTPUT);        // 팬 핀 출력 
  pinMode(IR, INPUT);             // IR 센서 핀 입력으로 설정
  pinMode(AD2_RCV_CGuard, INPUT); // MQTT 서보 모터 핀 입력으로 설정
  pinMode(Buzz, INPUT);           // 부저 입력 핀 설정
  MyServo.attach(6);              // 모터의 신호선을 6번핀에 연결
  MyServo.write(pos);
}

void loop() {  
  
  int AD2_CGuard = 0;
  int Val = digitalRead(IR);
  DynamicJsonDocument doc(254);   // json값 전달할 객체 생성
  digitalWrite(Fan_In, HIGH);

  if(Serial.available() > 0){
    AD2_CGuard = Serial.parseInt();
    delay(10);
  }
  
  if (Val == LOW && AD2_CGuard == 1)
  {
    digitalWrite(CGuard_LED, 1);     // LED ON
    for (pos = 190; pos >= 90; pos -= 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 90){
        pos = 90;
        MyServo.write(pos); // 서보모터 180도로 고정
        tone(Buzz, 1200);
        delay(10);
      }
    }
    doc["AD2_RCV_CGuard"] = int(pos);
    serializeJson(doc, Serial);
    Serial.println("");
    delay(1000);
    noTone(Buzz);
  }
  else if(Val == HIGH && AD2_CGuard == -1)
  {
    digitalWrite(CGuard_LED, 1);
    tone(Buzz, 1200);
    for (pos = 90; pos <= 190; pos += 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 180){
        pos = 180; 
        MyServo.write(pos); // 서보모터 90도로 고정
        delay(10);
      }
    }
    doc["AD2_RCV_CGuard"] = int(pos);   
    serializeJson(doc, Serial);
    Serial.println("");
    delay(1000);
    noTone(Buzz);
  }
}
