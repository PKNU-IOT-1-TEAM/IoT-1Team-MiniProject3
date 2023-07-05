/*
#include <ArduinoJson.h>

int motorPin1 = 6;
int motorPin2 = 5;
int waterPump1 =11;
int waterPump2 =10;
int TRIG = 9;
int ECHO = 8;
int dir = 2;
float duration, distance, temp, loop_init=0;
DynamicJsonDocument doc(128);
void setup() {
  Serial.begin(9600);
  pinMode(motorPin1,OUTPUT);
  pinMode(motorPin2,OUTPUT);
  pinMode(waterPump1,OUTPUT);
  pinMode(waterPump2,OUTPUT);
  pinMode(TRIG,OUTPUT);
  pinMode(ECHO,INPUT);
  pinMode(dir,OUTPUT);
  digitalWrite(dir,HIGH);
  
  printf("DC motor test");
}


void loop() {
  if(Serial.available()>0){
    char receivedChar = Serial.read();
    digitalWrite(motorPin1,LOW);
    digitalWrite(motorPin2,LOW);
    digitalWrite(waterPump1,LOW);
    digitalWrite(waterPump2,LOW);
    if(receivedChar == '1'){
      printf("Forward");
      digitalWrite(motorPin1,HIGH);
      digitalWrite(motorPin2,LOW);
      digitalWrite(waterPump1,HIGH);
      digitalWrite(waterPump2,LOW);
      digitalWrite(TRIG,LOW);
      digitalWrite(TRIG,HIGH);
      delayMicroseconds(10);
      digitalWrite(TRIG,LOW);
      duration = pulseIn(ECHO,HIGH);
      distance = duration * 17/1000;
      if(loop_init == 0)
      {
        temp = distance;
        loop_init =1;
      }
      if(temp < distance){
        delay(300);  
        Serial.println("abcdefghhhhhhhhhhhhhhh");
        while(temp != distance && distance <= 20)
        {
          Serial.print("dddddddddddddddddd");
          Serial.println(distance);
          
          doc["HC_SR04_sensor"] = "linear_forward";  
        }
        doc["HC_SR04_sensor"] = "linear_forward_completed";
      }
      
      temp = distance;
      //delay(2000);    
    }
    else if(receivedChar == '0'){
      printf("Backward");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,HIGH);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,HIGH);
      if(temp < distance){
        doc["HC_SR04_sensor"] = "linear_forward";
      }
      else if(temp > distance)
      {
        doc["HC_SR04_sensor"] = "linear_backward";
      }
      else if(temp == distance && distance < 15)
      {
        doc["HC_SR04_sensor"] = "linear_forward_completed";
      }
      else if(temp == distance && distance > 20)
      {
        doc["HC_SR04_sensor"] = "linear_backward_completed";
      }
      temp = distance;
      //delay(2000);    
    }
    else if(receivedChar == '2')
    {
      printf("Stop");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,LOW);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,LOW);
      //delay(2000);
    }


    
    serializeJson(doc,Serial);
    Serial.println();
  }
  delay(1000);
}
*/
