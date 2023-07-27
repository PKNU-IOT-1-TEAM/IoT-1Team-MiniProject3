import RPi.GPIO as gp
import time
import os
import io
from picamera import PiCamera
from flask import Flask, render_template
from flask_socketio import SocketIO, emit

# GPIO 초기화
gp.setwarnings(False)
gp.setmode(gp.BOARD)
gp.setup(7, gp.OUT)
gp.setup(11, gp.OUT)
gp.setup(12, gp.OUT)

# 카메라 초기화
camera = PiCamera()
camera.resolution = (320, 240)

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
    gp.output(7, 0)  # 채널 A 선택
    gp.output(11, 0)
    gp.output(12, 1)
    time.sleep(0.5)
    socketio.start_background_task(camera_stream, 'Camera 1')  # 첫 번째 카메라 스트리밍 시작

# 웹 소켓 연결 해제 핸들러
@socketio.on('disconnect', namespace='/camera')
def handle_disconnect():
    pass

def camera_stream(cam_name):
    stream = io.BytesIO()
    for frame in camera.capture_continuous(stream, format='jpeg', use_video_port=True):
        # 이미지 스트림을 소켓으로 전송
        stream.seek(0)
        image_bytes = stream.read()
        socketio.emit('update_image', {'cam_name': cam_name, 'image': image_bytes}, namespace='/camera', broadcast=True)

        stream.seek(0)
        stream.truncate()

if __name__ == "__main__":
    socketio.run(app, host='0.0.0.0', port=9000)