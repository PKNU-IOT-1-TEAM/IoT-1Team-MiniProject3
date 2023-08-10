import cv2                                                  # OpenCV : 이미지 처리와 컴퓨터 비전에 사용되는 라이브러리
import numpy as np                                          # 파이썬의 과학적 컴퓨팅을 위한 핵심 라이브러리, 다차원 배열과 벡터/행렬 연산에 대한 높은 수준의 지원 제공
import matplotlib.pyplot as plt                             # 그래프와 플롯을 그리는데 사용되는 라이브러리, 이미지를 시각화 하거나 데이터를 그래프로 표현하는 등 다양한 시각화 작업에 사용됨
import pytesseract                                          # Tesseract OCR(Optical Character Recongnition)의 파이썬 래퍼, OCR을 사용하여 이미지에서 텍스트 추출
import RPi.GPIO as GPIO                                     # 라즈베리 파이에서 GPIO핀을 제어하는데 사용
from picamera2 import Picamera2                             # Raspberry Pi 카메라 모듈을 사용하여 이미지 또는 비디오를 캡처하는데 사용되는 라이브러리, picamera2 모듈을 카메라를 제어하는데 도움 준다
from time import sleep
import time
import paho.mqtt.client as mqtt
import json

import carnumberdetect_gpio_semi_final as carNum 

# print(carNum.IR_Sensor_data)
# print(f'test file : {carNum.car_Number_Fin}')

# MQTT 통신 변수
BROKER = '210.119.12.112'
PORT = 11000
TOPIC3 = 'TEAM_ONE/parking/Car_Number_data/'
CLIENT_ID = 'IoT_Team1_Rasberry_PUB'

def on_connect(client, userdata, flags, rc):
    print("MQTT 연결 성공")
    print(f"Connect with result code : {str(rc)}")
    client.subscribe(TOPIC3)
    print(f"subscribing to topic : {TOPIC3}")

def on_message(client, userdata, message):
    print(f"Data requested + {str(message.payload)}")

def subscribing():
    client.on_message = on_message
    client.loop_forever()

global original_result, is_send_mqtt, client

original_result = {'IR_CAR_NUM': None, 'CAR_NUMBER' : None}

is_send_mqtt = False

client = mqtt.Client('TEAM_ONE')
client.connect(BROKER, PORT)
client.on_connect = on_connect


try:
    while True:
        carNum.IR_Func()
        if is_send_mqtt == False:
            original_result['IR_CAR_NUM'] = carNum.IR_Sensor_data
            original_result['CAR_NUMBER'] = carNum.car_Number_Fin

        json_data = json.dumps(original_result)
        client.publish(topic=TOPIC3, payload=json_data)
        print(f'published data : {json_data}')\
        
        # carNum.IR_Sensor_data = None
        carNum.car_Number_Fin = ''

        time.sleep(1)

except KeyboardInterrupt:
    pass

finally:
    GPIO.cleanup()
    client.disconnect()