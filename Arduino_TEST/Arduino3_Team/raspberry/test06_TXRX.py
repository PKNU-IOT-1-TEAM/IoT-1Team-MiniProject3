import serial
import time
import RPi.GPIO as GPIO

# arduino1_TX_PIN = 2  # 아두이노1의 TX 핀
# arduino1_RX_PIN = 3  # 아두이노1의 RX 핀

# arduino2_TX_PIN = 4  # 아두이노2의 TX 핀
# arduino2_RX_PIN = 17  # 아두이노2의 RX 핀

# arduino3_TX_PIN = 27  # 아두이노3의 TX 핀
# arduino3_RX_PIN = 22  # 아두이노3의 RX 핀

# arduino4_TX_PIN = 10  # 아두이노4의 TX 핀
# arduino4_RX_PIN = 9  # 아두이노4의 RX 핀

# GPIO설정
# GPIO.setmode(GPIO.BCM)
# GPIO.setup(arduino1_TX_PIN, GPIO.OUT)
# GPIO.setup(arduino1_RX_PIN, GPIO.IN)

# GPIO.setup(arduino2_TX_PIN, GPIO.OUT)
# GPIO.setup(arduino2_RX_PIN, GPIO.IN)

# GPIO.setup(arduino3_TX_PIN, GPIO.OUT)
# GPIO.setup(arduino3_RX_PIN, GPIO.IN)

# GPIO.setup(arduino4_TX_PIN, GPIO.OUT)
# GPIO.setup(arduino4_RX_PIN, GPIO.IN)

arduino1 = serial.Serial(
    port=None,
    baudrate=9600,
    timeout=1
)
arduino2 = serial.Serial(
    port=None,
    baudrate=9600,
    timeout=1
)
# arduino3 = serial.Serial(
#     port=None,
#     baudrate=9600,
#     timeout=1
# )
# arduino4 = serial.Serial(
#     port=None,
#     baudrate=9600,
#     timeout=1
# )

# 소프트웨어 UART 포트설정
arduino1.port = '/dev/ttyAMA0'    # 아두이노 1의 소프트웨어 UART 포트 설정
arduino2.port = '/dev/ttyAMA1'    # 아두이노 2의 소프트웨어 UART 포트 설정
# arduino3.port = '/dev/ttyS2'    # 아두이노 3의 소프트웨어 UART 포트 설정
# arduino4.port = '/dev/ttyS3'    # 아두이노 4의 소프트웨어 UART 포트 설정

# 소프트웨어 UART 열기
arduino1.open()
arduino2.open()
# arduino3.open()
# arduino4.open()

# 데이터 전송
arduino1.write(b'Distance Sensor')
arduino2.write(b'HumidANDTemp Sensor')

while True:    
    # 데이터 수신
    data1 = arduino1.readline().decode('utf-8').rstrip()
    data2 = arduino2.readline().decode('utf-8').rstrip()

    # 수신된 데이터 출력
    print('Arduino 1 : ', data1)
    print('Arduino 2 : ', data2)
    time.sleep(0.5)

# 시리얼 포트 닫기
arduino1.close()
arduino2.close()

