#include <ArduinoJson.h>

int motorPin1 = 6;  // 모터 정회전
int motorPin2 = 5;  // 모터 역회전
int waterPump1 =11; // 워터펌프 정회전
int waterPump2 =10; // 워터펌프 역회적
int TRIG = 9; // 초음파 센서 트리거
int ECHO = 8; // 초음파 센서 에코
int dir = 2;  // 릴레이 시그널 핀

void setup() {
  Serial.begin(9600);
  // 아두이노 핀 세팅
  pinMode(motorPin1,OUTPUT);
  pinMode(motorPin2,OUTPUT);
  pinMode(waterPump1,OUTPUT);
  pinMode(waterPump2,OUTPUT);
  pinMode(TRIG,OUTPUT);
  pinMode(ECHO,INPUT);
  pinMode(dir,OUTPUT);
  // 핀 값 초기화
  digitalWrite(dir,HIGH);

  // 모터 정지 초기화
  stopMotor();
  
  //Serial.println("DC motor test");
}


DynamicJsonDocument doc(128);
void loop() 
{
  
  if (Serial.available())
  {
    char command = Serial.read();

    if (command == '1')
    {
      forwardMotion();  // 전진 명령 수행
    }
    else if (command == '0')
    {
      backwardMotion(); // 후진 명령 수행  
    }
  }

  // 초음파 센서로 거리 측정
  int distance = int(measureDistance());

  // 이전에 측정한 거리와 비교하여 모터 동작 상태 판단
  static int temp = 0;
  if(distance > temp)
  {
    doc["AD3_RCV_WGuard_Wave"] = "Forward";
  }
  else if(distance < temp)
  {
    doc["AD3_RCV_WGuard_Wave"] = "Backward";
  }
  else if (distance >= 15)
  {
    doc["AD3_RCV_WGuard_Wave"] = "Forward completed";
  }
  else if (distance < 5)
  {
    doc["AD3_RCV_WGuard_Wave"] = "Backward completed";
  }
  serializeJson(doc,Serial);
  temp = distance;
  delay(1000);
}

void forwardMotion()
{
  digitalWrite(motorPin1,HIGH);
  digitalWrite(motorPin2,LOW);
}
void backwardMotion()
{
  digitalWrite(motorPin1,LOW);
  digitalWrite(motorPin2,HIGH);
}
void stopMotor()
{
  digitalWrite(motorPin1,LOW);
  digitalWrite(motorPin2,LOW);
}
float measureDistance()
{
  digitalWrite(TRIG,LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG,HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG,LOW);

  unsigned long duration = pulseIn(ECHO,HIGH);
  float distance = duration * 0.034 / 2;

  return distance;
}
