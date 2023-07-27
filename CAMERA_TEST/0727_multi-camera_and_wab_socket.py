from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import cv2
from io import BytesIO

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 초기화
camera = cv2.VideoCapture(0)  # 카메라 인덱스는 0일 수도 있고, 다른 값일 수도 있습니다.

@app.route('/')
def index():
    return render_template('index.html')

def capture_and_emit():
    while True:
        # 이미지 캡처
        ret, frame = camera.read()
        if ret:
            # 이미지를 JPEG 형식으로 인코딩하여 웹소켓 클라이언트로 전송
            _, image_bytes = cv2.imencode('.jpg', frame)
            socketio.emit('update_image', {'image': image_bytes.tobytes()})

# 웹소켓 연결 시 호출되는 이벤트 핸들러
@socketio.on('connect', namespace='/camera')
def handle_connect():
    print('WebSocket client connected.')
    # 이미지 캡처 및 웹소켓 클라이언트로의 이미지 전송을 백그라운드에서 시작
    socketio.start_background_task(target=capture_and_emit)

if __name__ == '__main__':
    socketio.run(app, host='0.0.0.0', port=8765)
