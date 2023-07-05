int motorPin1 = 6;
int motorPin2 = 5;
int waterPump1 =11;
int waterPump2 =10;
int dir = 2;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(motorPin1,OUTPUT);
  pinMode(motorPin2,OUTPUT);
  pinMode(waterPump1,OUTPUT);
  pinMode(waterPump2,OUTPUT);
  pinMode(dir,OUTPUT);
  digitalWrite(dir,HIGH);
  digitalWrite(motorPin1,LOW);
  digitalWrite(motorPin2,LOW);
  digitalWrite(waterPump1,LOW);
  digitalWrite(waterPump2,LOW);
  Serial.println("DC motor test");
}

void loop() {
  // put your main code here, to run repeatedly:
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
      delay(2000);    
    }
    else if(receivedChar == '0'){
      printf("Backward");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,HIGH);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,HIGH);
      delay(2000);    
    }
    else if(receivedChar == '2')
    {
      printf("Stop");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,LOW);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,LOW);
      delay(2000);
    }
  }
  if(digitalRead(motorPin2) && digitalRead(waterPump1) ==LOW){
    Serial.println('0');
  }
  else
  {
    Serial.println('1');
  }
  delay(1000);
}
