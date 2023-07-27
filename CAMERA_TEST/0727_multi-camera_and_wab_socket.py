import RPi.GPIO as gp
import time
import os
from picamera2 import Picamera2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
from PIL import Image
from io import BytesIO
import threading

width = 320
height = 240

adapter_info = {
    "A": {
        "i2c_cmd": "i2cset -y 10 0x70 0x00 0x04",
        "gpio_sta": [0, 0, 1],
    },
    "B": {
        "i2c_cmd": "i2cset -y 10 0x70 0x00 0x06",
        "gpio_sta": [0, 1, 0],
    }
}

# 각 채널에 대한 카메라 작업 처리를 위한 스레드 클래스
class WorkThread(threading.Thread):
    def __init__(self):
        super(WorkThread, self).__init__()
        gp.setwarnings(False)
        gp.setmode(gp.BOARD)
        gp.setup(7, gp.OUT)
        gp.setup(11, gp.OUT)
        gp.setup(12, gp.OUT)

    def select_channel(self, index):
        channel_info = adapter_info.get(index)
        if channel_info is None:
            print("해당 정보를 찾을 수 없습니다.")
        gpio_sta = channel_info["gpio_sta"]
        gp.output(7, gpio_sta[0])
        gp.output(11, gpio_sta[1])
        gp.output(12, gpio_sta[2])

    def init_i2c(self, index):
        channel_info = adapter_info.get(index)
        os.system(channel_info["i2c_cmd"])

    def run(self):
        global picam2
        flag = False

        for item in {"A", "B"}:
            try:
                self.select_channel(item)
                self.init_i2c(item)
                time.sleep(0.5)

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
            for item in {"A", "B"}:
                self.select_channel(item)
                time.sleep(0.02)
                try:
                    buf = picam2.capture_array()
                    buf = picam2.capture_array()
                    img = Image.frombytes('RGB', (width, height), buf)
                    image_bytes = BytesIO()
                    img.save(image_bytes, format='JPEG')

                    # 웹 소켓을 통해 이미지를 클라이언트로 전송
                    socketio.emit('update_image', {'item': item, 'image': image_bytes.getvalue()}, namespace='/camera')
                except Exception as e:
                    print("캡처 버퍼: " + str(e))
            time.sleep(0.1)

# Flask 설정
app = Flask(__name__)
socketio = SocketIO(app)

# 첫 번째 카메라 페이지 라우트
@app.route('/cam1')
def cam1():
    return render_template('camera.html', cam_name="Camera 1")

# 두 번째 카메라 페이지 라우트
@app.route('/cam2')
def cam2():
    return render_template('camera.html', cam_name="Camera 2")

# 웹 소켓 연결 핸들러
@socketio.on('connect', namespace='/camera')
def handle_connect():
    global picam2, work
    work = WorkThread()
    work.start()
    picam2 = Picamera2()
    picam2.configure(picam2.create_still_configuration(main={"size": (320, 240), "format": "BGR888"}, buffer_count=2))
    picam2.start()

# 웹 소켓 연결 해제 핸들러
@socketio.on('disconnect', namespace='/camera')
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