int motorPin1 = 6;
int motorPin2 = 5;
int waterPump1 =11;
int waterPump2 =10;
int TRIG = 9;
int ECHO = 8;
int dir = 2;
int i = 0;
float duration;
float distance, temp, loop_init=0;
int Dis,Tmp;
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

  Serial.println("DC motor test");
}


void loop() {
  if(Serial.available()>0){
    char receivedChar = Serial.read();
    digitalWrite(motorPin1,LOW);
    digitalWrite(motorPin2,LOW);
    digitalWrite(waterPump1,LOW);
    digitalWrite(waterPump2,LOW);
    if(receivedChar == '1'){
      Serial.println("Forward");
      digitalWrite(motorPin1,HIGH);
      digitalWrite(motorPin2,LOW);
      digitalWrite(waterPump1,HIGH);
      digitalWrite(waterPump2,LOW);
      
      
      if(loop_init == 0)
      {
        temp = distance;
        loop_init =1;
      }
      while(true)
      {
        digitalWrite(TRIG,LOW);
        delay(10);
        digitalWrite(TRIG,HIGH);
        delayMicroseconds(10);
        digitalWrite(TRIG,LOW);
        duration = pulseIn(ECHO,HIGH);
        distance = duration * 17/1000;
        
        Serial.print("distance : ");
        Serial.println(distance);
        Serial.print("temp : ");
        Serial.println(temp);
        delay(300);
        Dis = int(distance);
        Tmp = int(temp);
        if(Tmp <= Dis)
        {
          Serial.println("Forward");
          Serial.println(Dis);
          Serial.print("--->");
          Serial.println(Tmp);
          
          if(Tmp == Dis && Dis > 20)
          {
            Serial.println("Forward Complete");
            break;
          }
        }
        temp=distance;  
      }
      Serial.print("Bye~~~~~~~~~~~~~~!");
    }
    else if(receivedChar == '0'){
      Serial.println("Backward");
      digitalWrite(motorPin1,LOW);
      digitalWrite(motorPin2,HIGH);
      digitalWrite(waterPump1,LOW);
      digitalWrite(waterPump2,HIGH);
      
    }

  }
  delay(1000);
}
