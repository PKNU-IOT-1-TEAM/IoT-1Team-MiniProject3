import cv2
from picamera2 import Picamera2 as cam2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import threading
import base64

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 초기화
picam = cam2()
picam.preview_configuration.size = (800, 600)
picam.preview_configuration.main.format = 'RGB888'
picam.preview_configuration.align()

def emit_camera_frames():
    picam.configure(".jpg")
    picam.start_preview()
    picam.start()

    while True:
        frame = picam.get_frame()  # picamera에서 현재 프레임을 가져옴
        _, buffer = cv2.imencode(".jpg", frame)  # 프레임을 JPEG 형식으로 인코딩

        # 인코딩된 프레임을 base64로 인코딩하여 클라이언트로 전송
        encoded_frame = base64.b64encode(buffer)
        socketio.emit("stream", encoded_frame.decode("utf-8"))

@app.route('/')
def index():
    return render_template('index.html')

if __name__ == '__main__':
    # 스트리밍 스레드를 시작
    streaming_thread = threading.Thread(target=emit_camera_frames)
    streaming_thread.daemon = True
    streaming_thread.start()

    # Flask 앱 실행
    socketio.run(app, port=9000)