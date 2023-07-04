#include <Servo.h>           // Servo 라이브러리 헥사 선언

Servo MyServo;              // 서보모터 선언
int pos = 0;                 // 모터 위치를 확인하기 위해 변수를 선언
int AD2_Servo = 6;          // 모터 제어를 위해 6번핀(PWM) 으로 선언
int AD2_Servo_LED = 7;      // 서보모터 선언
int AD2_Linear_LED = 8;     // 서보모터 선언
int Ras_Signal_Input = 9;   // 라즈베리파이 인풋 선언
int Open_Servo_Value = 10;  // MQTT에서 받을 Servo 신호
int Open_Linear_Value = 11; // MQTT에서 받을 Linear 신호
int AD2_IR_Sensor = 12;     // 차량 인식을 위한 IR 센서
void setup() {
  Serial.begin(115200);     // 시리얼 통신 115200
  pinMode(AD2_Servo_LED, OUTPUT);   / 서보 모터 LED 핀 출력으로 설정
  pinMode(AD2_Linear_LED, OUTPUT);  // 리니어 모터 LED 핀 출력으로 설정
  pinMode(AD2_Servo, OUTPUT);       // 서보 모터 핀 출력으로 설정
  pinMode(AD2_IR_Sensor, INPUT);    // IR 센서 핀 입력으로 설정
  pinMode(Open_Servo_Value, INPUT); // MQTT 서보 모터 핀 입력으로 설정
  pinMode(Open_Linear_Value, INPUT);// MQTT 리니어 모터 핀 입력으로 설정
}

void loop() {
  int Val = digitalRead(AD2_IR_Sensor); // IR 신호 핀 읽어와서 Val에 저장
  int LED_Val = digitalRead(Open_Linear_Value); // MQTT 리니어 값 읽어와서 LED_Val에 저장 
  Serial.println(Val);                  // 시리얼 포트에 Val값 출력
  MyServo.attach(6);                    // 서보 모터의 신호선을 6번핀에 연결

  if (Val == LOW)                       // IR 인식이 됐다면 (이후)라즈베리 카메라 인식 + IR 신호값이 들어왔다면
  {
    Serial.println("IR_Sensor READ");  // 시리얼에서 확인
    digitalWrite(AD2_Servo_LED, 1);    // 서보 신호핀에 HIGH 값 입력
    Serial.println("Gate Open");       // 시리얼에서 표시
    for (pos = 0; pos <= 180; pos += 1)// 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      MyServo.write(pos);               // 1도씩 증가
      delay(10);                        // 딜레이
    }
    MyServo.detach()                      // 서보모터 핀 연결 해제 -- 정지
  }
  else
  {
    Serial.println("IR_Sensor NOT READ");
    digitalWrite(AD2_Servo_LED, 1);    // 서보 신호핀에 HIGH 값 입력
    Serial.println("Gate Close");       // 시리얼에서 표시
    for (pos = 180; pos >= 0; pos -= 1)// 위에 변수를 선언한 pos는 180, 0도보다 크다면 , 1도씩 빼고
    {
      MyServo.write(pos);               // 1도씩 감소
      delay(10);                        // 딜레이
    }
    MyServo.detach()                      // 서보모터 핀 연결 해제 -- 정지
  }
  if (Open_Linear_Value = 1)
  {
    digitalWrite(AD2_Linear_LED, 1);
  }
  else if (Open_Linear_Value = 0)
  {
    digitalWrite(AD2_Linear_LED, 0);
  }
}
