from flask import Flask, render_template
from flask_socketio import SocketIO, emit
from picamera import PiCamera
from PIL import Image
from io import BytesIO
import numpy as np

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 초기화
camera = PiCamera()
width, height = 640, 480
camera.resolution = (width, height)

@app.route('/')
def index():
    return render_template('index.html')

def capture_and_emit():
    while True:
        # 이미지 캡처
        buf = camera.capture_array()
        img = Image.fromarray(np.uint8(buf))

        # 이미지를 JPEG 형식으로 인코딩하여 웹소켓 클라이언트로 전송
        image_bytes = BytesIO()
        img.save(image_bytes, format='JPEG')
        socketio.emit('update_image', {'image': image_bytes.getvalue()})

# 웹소켓 연결 시 호출되는 이벤트 핸들러
@socketio.on('connect', namespace='/camera')
def handle_connect():
    print('WebSocket client connected.')
    # 이미지 캡처 및 웹소켓 클라이언트로의 이미지 전송을 백그라운드에서 시작
    socketio.start_background_task(target=capture_and_emit)

if __name__ == '__main__':
    socketio.run(app, host='0.0.0.0', port=9000)