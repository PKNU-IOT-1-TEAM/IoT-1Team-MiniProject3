import RPi.GPIO as gp
import time
import os
from picamera2 import Picamera2
from flask import Flask, render_template, request
from flask_socketio import SocketIO, emit
from PIL import Image
from io import BytesIO
import threading

# 카메라 해상도 설정
width = 320
height = 240

# 다른 카메라 채널에 대한 설정
adapter_info = {
    "A": {
        "i2c_cmd": "i2cset -y 10 0x70 0x00 0x04",  # 채널 A에 대한 I2C 명령어
        "gpio_sta": [0, 0, 1],  # 채널 A의 GPIO 상태 (이진수: 001)
    },
    "B": {
        "i2c_cmd": "i2cset -y 10 0x70 0x00 0x06",  # 채널 B에 대한 I2C 명령어
        "gpio_sta": [0, 1, 0],  # 채널 B의 GPIO 상태 (이진수: 010)
    }
}

# 각 채널에 대한 카메라 작업 처리를 위한 스레드 클래스
class WorkThread(threading.Thread):

    def __init__(self):
        super(WorkThread, self).__init__()
        gp.setwarnings(False)
        gp.setmode(gp.BOARD)
        gp.setup(7, gp.OUT)   # GPIO 7을 출력으로 설정
        gp.setup(11, gp.OUT)  # GPIO 11을 출력으로 설정
        gp.setup(12, gp.OUT)  # GPIO 12을 출력으로 설정

    # 채널 선택을 위한 함수로 적절한 GPIO 상태를 설정합니다.
    def select_channel(self, index):
        channel_info = adapter_info.get(index)
        if channel_info is None:
            print("해당 정보를 찾을 수 없습니다.")
        gpio_sta = channel_info["gpio_sta"]
        gp.output(7, gpio_sta[0])
        gp.output(11, gpio_sta[1])
        gp.output(12, gpio_sta[2])

    # 특정 채널에 대한 I2C 초기화 함수
    def init_i2c(self, index):
        channel_info = adapter_info.get(index)
        os.system(channel_info["i2c_cmd"])

    # 메인 스레드 실행
    def run(self):
        global picam2
        flag = False

        # 각 채널 "A"와 "B"에 대해 반복합니다.

        for item in {"A", "B"}:
            try:
                # 채널과 I2C 초기화
                self.select_channel(item)
                self.init_i2c(item)
                time.sleep(0.5)  # 안정화를 위해 잠시 대기

                # 이미 카메라가 초기화된 경우에는 종료합니다.
                if flag is False:
                    flag = True
                else:
                    picam2.close()

                print("초기화 " + item)
                picam2 = Picamera2()
                picam2.configure(picam2.create_still_configuration(main={"size": (320, 240), "format": "BGR888"}, buffer_count=2))
                picam2.start()
                time.sleep(2)
                picam2.capture_array(wait=False)
                time.sleep(0.1)
            except Exception as e:
                print("예외: " + str(e))

        while True:
            # 각 채널로부터 지속적으로 이미지를 캡처합니다.
            for item in {"A", "B"}:
                self.select_channel(item)
                time.sleep(0.02)
                try:
                    buf = picam2.capture_array()
                    buf = picam2.capture_array()
                    img = Image.frombytes('RGB', (width, height), buf)
                    image_bytes = BytesIO()
                    img.save(image_bytes, format='JPEG')
                    emit('update_image', {'item': item, 'image': image_bytes.getvalue()})
                except Exception as e:
                    print("캡처 버퍼: " + str(e))
            time.sleep(0.1)

# Flask 설정
app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret_key'
socketio = SocketIO(app)

# HTML 페이지를 제공하는 Flask 라우트
@app.route('/')
def index():
    return render_template('index.html')

# WebSocket 연결 핸들러
@socketio.on('connect')
def handle_connect():
    global picam2, work
    work = WorkThread()
    work.start()
    picam2 = Picamera2()
    picam2.configure(picam2.create_still_configuration(main={"size": (320, 240), "format": "BGR888"}, buffer_count=2))
    picam2.start()

# WebSocket 연결 해제 핸들러
@socketio.on('disconnect')
def handle_disconnect():
    global picam2
    picam2.close()

if __name__ == "__main__":
    gp.setwarnings(False)
    gp.setmode(gp.BOARD)
    gp.setup(7, gp.OUT)
    gp.setup(11, gp.OUT)
    gp.setup(12, gp.OUT)

    # Flask 앱을 SocketIO 지원으로 실행합니다.
    socketio.run(app, host='0.0.0.0', port=9000)