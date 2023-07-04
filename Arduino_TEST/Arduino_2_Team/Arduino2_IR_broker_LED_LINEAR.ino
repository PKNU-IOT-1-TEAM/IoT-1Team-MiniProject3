#include <Servo.h>           // Servo 라이브러리 헥사 선언

Servo MyServo;              // 서보모터 선언
int pos = 0;                 // 모터 위치를 확인하기 위해 변수를 선언
int AD2_Servo = 6;          // 모터 제어를 위해 6번핀(PWM) 으로 선언
int AD2_Servo_LED = 7;      // 서보모터 선언
int AD2_Linear_LED = 8;     // 서보모터 선언
int Ras_Signal_Input = 9;   // 라즈베리파이 인풋 선언
int Open_Servo_Value = 10;
int Open_Linear_Value = 11;
int AD2_IR_Sensor = 12;
void setup() {
  Serial.begin(115200);
  pinMode(AD2_Servo_LED, OUTPUT);
  pinMode(AD2_Linear_LED, OUTPUT);
  pinMode(AD2_Servo, OUTPUT);           // 모터 제어핀을 출력으로 설정
  pinMode(AD2_IR_Sensor, INPUT);
  pinMode(Open_Servo_Value, INPUT);
  pinMode(Open_Linear_Value, INPUT);
  MyServo.attach(6);                               // 모터의 신호선을 6번핀에 연결
}

void loop() {
  int Val = digitalRead(AD2_IR_Sensor);
  Serial.println(Val);
  int LED_Val = digitalRead(Open_Linear_Value);
  if (Val == LOW)
  {
    Serial.println("IR_Sensor READ");
    digitalWrite(AD2_Servo_LED, 1);
    Serial.println("Gate Open");
    for (pos = 0; pos <= 180; pos += 1)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      MyServo.write(pos);
      delay(10);
    }
  }
  else
  {
    Serial.println("IR_Sensor NOT READ");
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
