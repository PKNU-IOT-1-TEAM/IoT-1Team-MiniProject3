
int TRIG = 9; // 초음파 센서 트리거
int ECHO = 8; // 초음파 센서 에코

void setup() {
  Serial.begin(9600);
  // 아두이노 핀 세팅
  pinMode(TRIG,OUTPUT);
  pinMode(ECHO,INPUT);
}
void loop() 
{
  // 초음파 센서로 거리 측정
  int distance = int(measureDistance());
  Serial.println(distance);
  delay(500);
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
