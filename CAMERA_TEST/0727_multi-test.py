import RPi.GPIO as gp
import time
import os
from picamera2 import Picamera2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
from PIL import Image
from io import BytesIO
import threading

# GPIO 초기화
gp.setwarnings(False)
gp.setmode(gp.BOARD)
gp.setup(7, gp.OUT)
gp.setup(11, gp.OUT)
gp.setup(12, gp.OUT)

# 각 채널에 대한 카메라 작업 처리를 위한 스레드 클래스
class CameraThread(threading.Thread):
    def __init__(self, channel, cam_name):
        super(CameraThread, self).__init__()
        self.channel = channel
        self.cam_name = cam_name
        self.picam = None

    def select_channel(self):
        channel_info = {
            "A": {
                "i2c_cmd": "i2cset -y 10 0x70 0x00 0x04",
                "gpio_sta": [0, 0, 1],
            },
            "C": {
                "i2c_cmd": "i2cset -y 10 0x70 0x00 0x06",
                "gpio_sta": [0, 1, 0],
            }
        }
        channel_info = channel_info.get(self.channel)
        if channel_info is None:
            print("해당 정보를 찾을 수 없습니다.")
        gpio_sta = channel_info["gpio_sta"]
        gp.output(7, gpio_sta[0])
        gp.output(11, gpio_sta[1])
        gp.output(12, gpio_sta[2])

    def init_i2c(self):
        channel_info = {
            "A": {
                "i2c_cmd": "i2cset -y 10 0x70 0x00 0x04",
                "gpio_sta": [0, 0, 1],
            },
            "C": {
                "i2c_cmd": "i2cset -y 10 0x70 0x00 0x06",
                "gpio_sta": [0, 1, 0],
            }
        }
        channel_info = channel_info.get(self.channel)
        os.system(channel_info["i2c_cmd"])

    def run(self):
        self.select_channel()
        self.init_i2c()
        time.sleep(0.5)

        self.picam = Picamera2()
        self.picam.configure(self.picam.create_still_configuration(main={"size": (320, 240), "format": "BGR888"}, buffer_count=2))
        self.picam.start()
        time.sleep(2)
        self.picam.capture_array(wait=False)
        time.sleep(0.1)

        while True:
            try:
                buf = self.picam.capture_array()
                img = Image.frombytes('RGB', (320, 240), buf)
                image_bytes = BytesIO()
                img.save(image_bytes, format='JPEG')

                # 웹 소켓을 통해 이미지를 클라이언트로 전송
                socketio.emit('update_image', {'cam_name': self.cam_name, 'image': image_bytes.getvalue()}, namespace='/camera', broadcast=True)
            except Exception as e:
                print("캡처 버퍼: " + str(e))
            time.sleep(0.1)

# Flask 설정
app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret_key'
socketio = SocketIO(app, cors_allowed_origins='*')

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
    global camera_threads
    if not camera_threads:
        camera_threads = {
            "A": CameraThread("A", "Camera 1"),
            "C": CameraThread("C", "Camera 2")
        }
        for cam_thread in camera_threads.values():
            cam_thread.start()

# 웹 소켓 연결 해제 핸들러
@socketio.on('disconnect', namespace='/camera')
def handle_disconnect():
    pass

if __name__ == "__main__":
    camera_threads = {}
    socketio.run(app, host='0.0.0.0', port=9000)