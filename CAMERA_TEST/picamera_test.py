import cv2
from picamera2 import Picamera2 as cam2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import threading

app = Flask(__name__)
socketio = SocketIO(app)

picam2 = cam2()
picam2.preview_configuration.main.size = (1280,720)
picam2.preview_configuration.main.format = "RGB888"
picam2.preview_configuration.align()

def emit_camera_frames():
    picam2.configure("preview")
    picam2.start()

    while True:
        im= picam2.capture_array()
        buffer = cv2.imencode(".jpg", im)
        socketio.emit("stream", buffer)

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