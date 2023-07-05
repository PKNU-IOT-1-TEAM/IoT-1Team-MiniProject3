#include <Servo.h>           // Servo 라이브러리 헥사 선언

Servo MyServo;              // 서보모터 선언
int pos = 90;                 // 모터 위치를 확인하기 위해 변수를 선언
int AD2_Servo = 6;          // 모터 제어를 위해 6번핀(PWM) 으로 선언
int AD2_Servo_LED = 7;      // 서보모터 선언
int AD2_Linear_LED = 8;     // 서보모터 선언
int Ras_Signal_Input = 9;   // 라즈베리파이 인풋 선언
int Open_Servo_Value = 10;  // MQTT에서 받을 Servo 신호
int Open_Linear_Value = 11; // MQTT에서 받을 Linear 신호
int AD2_IR_Sensor = 12;     // 차량 인식을 위한 IR 센서

void setup() {
  Serial.begin(9600);     // 시리얼 통신 9600
  pinMode(AD2_Servo_LED, OUTPUT);   // 서보 모터 LED 핀 출력으로 설정
  pinMode(AD2_Linear_LED, OUTPUT);  // 리니어 모터 LED 핀 출력으로 설정
  pinMode(AD2_Servo, OUTPUT);       // 서보 모터 핀 출력으로 설정
  pinMode(AD2_IR_Sensor, INPUT);    // IR 센서 핀 입력으로 설정
  pinMode(Open_Servo_Value, INPUT); // MQTT 서보 모터 핀 입력으로 설정
  pinMode(Open_Linear_Value, INPUT);// MQTT 리니어 모터 핀 입력으로 설정
  MyServo.attach(6);                               // 모터의 신호선을 6번핀에 연결
}

void loop() {  
  int Val = digitalRead(AD2_IR_Sensor);
  int LED_Val = digitalRead(Open_Linear_Value);
  int Car_Num_True_False = 2;

  if(Serial.available() > 0){
    Serial.println("input num plz");
    Car_Num_True_False = Serial.parseInt();
    delay(10);
  }
  
  if (Val == LOW && Car_Num_True_False == 1)
  {
    Serial.print("Arduino received : ");
    Serial.println(Car_Num_True_False); // 입력받은 서보 상태 명령어 출력 
    digitalWrite(AD2_Servo_LED, 1);     // LED ON
    for (pos = 90; pos <= 180; pos += 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 180){
        pos = 180;
        MyServo.write(pos); // 서보모터 180도로 고정
        delay(10);
      }
    }
    Serial.println("OPEN");
  }
  else if(Val == HIGH && Car_Num_True_False == 0)
  {
    Serial.print("Arduino received : ");
    Serial.println(Car_Num_True_False);
    digitalWrite(AD2_Servo_LED, 0);
    for (pos = 180; pos >= 90; pos -= 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 90){
        pos = 90; 
        MyServo.write(pos); // 서보모터 90도로 고정
        delay(10);
      }
    }
    Serial.println("CLOSE");    
  }
}
