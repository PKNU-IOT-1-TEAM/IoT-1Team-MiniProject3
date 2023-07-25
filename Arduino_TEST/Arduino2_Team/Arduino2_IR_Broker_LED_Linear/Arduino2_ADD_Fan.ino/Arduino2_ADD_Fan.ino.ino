#include <ArduinoJson.h>
#include <Servo.h>           // Servo 라이브러리 헥사 선언

Servo MyServo;              // 서보모터 선언
int pos = 190;                 // 모터 위치를 확인하기 위해 변수를 선언
int Fan_In = 3;           // 퍤 모터
int CGuard = 6;          // 서보모터 제어를 위해 핀번호 (PWM)
int CGuard_LED = 7;      // LED 핀 선언(RED)
int Buzz = 8;             // 부저 핀 선언
int AD2_RCV_CGuard = 10;  // MQTT에 보낼 Servo 신호
int IR = 11;     // 차량 인식을 위한 IR 센서

DynamicJsonDocument doc(254);   // json값 전달할 객체 생성

void setup() {
  Serial.begin(9600);             // 시리얼 통신 9600
  pinMode(CGuard_LED, OUTPUT);    // 서보 모터 LED 핀 출력으로 설정 
  pinMode(CGuard, OUTPUT);        // 서보 모터 핀 출력으로 설정
  pinMode(Fan_In, OUTPUT);        // 팬 핀 출력 
  pinMode(IR, INPUT);             // IR 센서 핀 입력으로 설정
  pinMode(AD2_RCV_CGuard, INPUT); // MQTT 서보 모터 핀 입력으로 설정
  pinMode(Buzz, INPUT);           // 부저 입력 핀 설정
  MyServo.attach(6);              // 모터의 신호선을 6번핀에 연결
  MyServo.write(pos);             // 서보모터 초기 위치로 설정
}
  
void loop() {  
  int AD2_CGuard = 0;   // MQTT로 받은 서보모터 제어 신호 읽어서 저장하는 변수
  int Val = digitalRead(IR);  // IR센서의 값을 VAR변수에 저장
  digitalWrite(Fan_In, HIGH);   // 팬 동작 설정
 
  if(Serial.available() > 0){   // 시리얼 버퍼에 값이 있는지 판단
    AD2_CGuard = Serial.parseInt();   // 시리얼 통신을 통해 전달되는 숫자를 읽어와서 변수에 저장
    delay(10);
  }
  
  if ((Val == LOW && AD2_CGuard == 1) || AD2_CGuard == 2)
  {
    // IR센서 인식 되고, 유니티에서 차가 있다고 판단한 값(AD2_CGuard == 1)이면 차단봉 열림
    // 토글버튼 ON (AD2_CGuard == 2)
    digitalWrite(CGuard_LED, 0);     // LED ON
    for (pos = 190; pos >= 90; pos -= 10)    // 서보모터릴 180도에서 90도로 이동
    {
      if(pos == 90){
        pos = 90;
        MyServo.write(pos); // 서보모터 180도로 고정
        tone(Buzz, 1200);   // 부저 울림
        delay(10);
      }
    }
    doc["AD2_RCV_CGuard"] = int(pos);
    //serializeJson(doc, Serial);
    String jsonStr;
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(1000);
    noTone(Buzz); // 부저 울림 중지
  }
  else if((Val == HIGH && AD2_CGuard == -1) || AD2_CGuard == 3)
  {
    // IR센서 인식 안되고, 유니티에서 차가 없다고 판단한 값(AD2_CGuard == 1)이면 차단봉 닫힘
    // 토글버튼 OFF (AD2_CGuard == 3)
    digitalWrite(CGuard_LED, 1);  // LED OFF
    tone(Buzz, 1200);   // 부저 울림
    for (pos = 90; pos <= 190; pos += 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 180){
        pos = 180; 
        MyServo.write(pos); // 서보모터 90도로 고정
        delay(10);
      }
    }
    doc["AD2_RCV_CGuard"] = int(pos);
    
    //serializeJson(doc, Serial);
    String jsonStr;
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(1000);
    noTone(Buzz);   // 부저 끄기
  }
  else
  {
    digitalWrite(CGuard_LED, 1);  // LED OFF
    doc["AD2_RCV_CGuard"] = int(pos);
    //serializeJson(doc, Serial);
    String jsonStr;
    serializeJson(doc, jsonStr);
    Serial.println(jsonStr);
    delay(1000);
    return 0;
  }
}
