#include <ArduinoJson.h>

int motorPin1 = 6;  // 모터 정회전
int motorPin2 = 5;  // 모터 역회전
int waterPump1 =11; // 워터펌프 정회전
int waterPump2 =10; // 워터펌프 역회적
int TRIG = 9; // 초음파 센서 트리거
int ECHO = 8; // 초음파 센서 에코
int dir = 2;  // 릴레이 시그널 핀

// 동작 상태 (리니어 모터)
enum MotorState
{
  IDLE, // 대기상태
  FORWARD, // 전진 중
  BACKWARD, // 후진 중
  COMPLETED // 동작 완료
};

MotorState motorState = IDLE; // 초기 상태는 대기 상태로 설정

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

  digitalWrite(waterPump1,LOW);
  digitalWrite(waterPump2,LOW);
  //Serial.println("DC motor test");
}


DynamicJsonDocument doc(128);
void loop() 
{
  
  if (Serial.available())
  {
    char command = Serial.read();

    if (command == '0')
    {
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,LOW);
    }
    else if (command == '1')
    {
      digitalWrite(waterPump1,HIGH);
      digitalWrite(waterPump2,LOW);
    } 
    else if (command == '2')
    {   
      forwardMotion();  // 전진 명령 수행
      motorState = FORWARD;
    }
    else if (command =='3')
    {
      backwardMotion(); // 후진 명령 수행  
      motorState = BACKWARD;
    }
  }
    // 초음파 센서로 거리 측정
  if(motorState == FORWARD || motorState == BACKWARD)
  {
   
    int distance = int(measureDistance());
    doc["AD3_RCV_WGuard_Wave"] = distance;
    String jsonStr;
    serializeJson(doc,jsonStr);
    Serial.println(jsonStr);

    // 목표 거리에 도달하였는지 확인하여 동작 완료 상태로 변경
    if(distance >= 20 || distance <= 10)
    {
      motorState = COMPLETED;
      stopMotor();
    }

  }
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
