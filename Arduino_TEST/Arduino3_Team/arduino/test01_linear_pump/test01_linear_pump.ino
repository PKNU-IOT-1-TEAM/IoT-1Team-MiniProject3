int motorPin1 = 6;
int motorPin2 = 5;
int waterPump1 =11;
int waterPump2 =10;
int TRIG = 9;
int ECHO = 8;
int dir = 2;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(motorPin1,OUTPUT);
  pinMode(motorPin2,OUTPUT);
  pinMode(waterPump1,OUTPUT);
  pinMode(waterPump2,OUTPUT);
  pinMode(TRIG,OUTPUT);
  pinMode(ECHO,INPUT);
  pinMode(dir,OUTPUT);
  digitalWrite(dir,HIGH);
  digitalWrite(motorPin1,LOW);
  digitalWrite(motorPin2,LOW);
  digitalWrite(waterPump1,LOW);
  digitalWrite(waterPump2,LOW);
  digitalWrite(TRIG,LOW);
  printf("DC motor test");
}

void loop() {
  float duration, distance;
  digitalWrite(TRIG,HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG,LOW);

  duration = pulseIn(ECHO,HIGH);
  distance = duration * 17/1000;
  
  if(Serial.available()){
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
      //delay(2000);    
    }
    else if(receivedChar == '0'){
      printf("Backward");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,HIGH);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,HIGH);
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
  }
  delay(1000);
  Serial.print(distance);
  if(distance > 25 || distance < 5){
    Serial.println("정지");
  }
  else
  {
    Serial.println("동작중");
  }
  delay(1000);
}
