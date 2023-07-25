#include <ArduinoJson.h>  // json모듈
#include <DHT.h>    // 온습도 모듈
#include <pm2008_i2c.h> // 미세먼지 센서 모듈

int AD1_IR1_out = 6;
int AD1_LED1_RED = 3; // LED_빨간색 핀
int AD1_LED1_GREEN = 5; // LED_초록색 핀
int AD1_LED1_BLUE = 10;   // LED_파란색 핀
#define DHTPIN 9     // 온습도 센서 연결 핀 번호
#define DHTTYPE DHT22   // 온습도 센서 타입 | DHT 22 

DHT dht(DHTPIN, DHTTYPE); // DHT객체 생성
PM2008_I2C pm2008_i2c;  // pm2008_i2c 객체 생성
  
DynamicJsonDocument doc(128);   // json 데이터 저장할 동적 메모리 할당

void setup() {
  // 온습도
  Serial.begin(9600);
  dht.begin();

  // 센서
  #ifdef PM2008N
    // PM2008N이 I2C모드로 변경되기 위해 1초 대기
    delay(1000);
  #endif
  pm2008_i2c.begin();
  pm2008_i2c.command();
  delay(1000);

  pinMode(AD1_LED1_RED, OUTPUT);  
  pinMode(AD1_LED1_GREEN, OUTPUT);  
  pinMode(AD1_LED1_BLUE, OUTPUT);  
  pinMode(AD1_IR1_out, INPUT);
}

void loop() {
  // 온습도 측정 위해 1초 대기
  delay(1000);

  // 온습도 읽어오기
  float h = dht.readHumidity();   // 습도 
  float t = dht.readTemperature();  // 섭씨 온도
  float f = dht.readTemperature(true);  // 화씨 온도

  // 온습도 측정 실패할 경우 다시 시도하기 위해 loop 함수 종료
  if (isnan(h) || isnan(t) || isnan(f)) {
    return;
  }

  // 센서
  uint8_t ret = pm2008_i2c.read();  // 미세먼지 농도 읽어옴
  digitalWrite(AD1_IR1_out, HIGH);    // 적외선 센서 활성화

  // 시리얼 통신을 통해 명령어를 입력받고, 해당 명령어에 따라 LED를 제어
  if (Serial.available())   // 시리얼 버퍼에 읽을 수 있는 데이터가 있으면 True
  {
    char command = Serial.read(); // 시리얼 버퍼에서 데이터를 읽어옴 -> 1바이트씩 || 데이터 없으면 -1 반환
  
    /*
     case 1. 제대로 주차(예약된 자리에 제대로 주차 & 빈자리에 주차된 차 O)    => 흰색
     case 2. 예약된 자리에 차가 없을 경우   => 주황색
     case 3. 예약도 없고 자리도 비었을 때   => 녹색
     case 4. 예약된 자리에 잘못된 차가 들어왔을 경우   => 빨간색
    */
    
    if(command == '1')
    {
      // 흰색 LED
      analogWrite(AD1_LED1_RED, 120); 
      analogWrite(AD1_LED1_GREEN,5);
      analogWrite(AD1_LED1_BLUE, 0);
    }
    else if(command =='2')
    {
      // 주황색
      analogWrite(AD1_LED1_RED, 30); 
      analogWrite(AD1_LED1_GREEN, 165);
      analogWrite(AD1_LED1_BLUE, 255);
    }
    else if(command =='3')
    {
      // 녹색
      analogWrite(AD1_LED1_RED, 255); 
      analogWrite(AD1_LED1_GREEN, 0);
      analogWrite(AD1_LED1_BLUE, 255);
    }
    else if(command =='4')
    {
      // 빨간색
      analogWrite(AD1_LED1_RED, 0); 
      analogWrite(AD1_LED1_GREEN, 255);
      analogWrite(AD1_LED1_BLUE, 255);
    }
  }

  int state = digitalRead(AD1_IR1_out);   // IR센서의 값을 state 변수에 저장 ( LOW(0) / HIGH(1) )

  doc["AD1_RCV_IR_Sensor"] = state;   // IR 센서
  doc["AD1_RCV_Temperature"] = t;     // 온도
  doc["AD1_RCV_Humidity"] = h;        // 습도
  doc["AD1_RCV_Dust"] = pm2008_i2c.number_of_2p5_um;    // 미세먼지

  // JSON 데이터를 문자열로 변환하고, 시리얼 통신을 통해 출력
  String jsonStr;
  serializeJson(doc, jsonStr);
  Serial.println(jsonStr);

  delay(1000);
}
