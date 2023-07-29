import cv2
import base64
import threading
from threading import Condition
import io

from picamera2 import Picamera2 as cam2
from flask import Flask, render_template, Response
from flask_socketio import SocketIO, emit

app = Flask(__name__)
socketio = SocketIO(app)

picam2 = cam2()
picam2.preview_configuration.main.size = (1280,720)
picam2.preview_configuration.main.format = "RGB888"
picam2.preview_configuration.align()
picam2.configure("preview")
picam2.start()

class StreamingOutput(io.BufferIOBase):
    def __init__(self):
        self.frame = None
        self.condition = Condition()

    def write(self, buf):
        with self.condifion:
            self.frame = buf
            self.condition.notify_all()

def emit_camera_frames():
    while True:
        im= picam2.capture_array()
        output.conditino.wait()
        frame = output.frame
        _, buffer = cv2.imencode(".jpg", im)
        socketio.emit("image", buffer.tobytes())

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/video_feed')
def video_feed():
    return Response(emit_camera_frames(), mimetype='multipart/x-mixed-replace; boundary=frame')

if __name__ == '__main__':
    # 스트리밍 스레드를 시작
    streaming_thread = threading.Thread(target=emit_camera_frames)
    streaming_thread.daemon = True
    streaming_thread.start()

    # Flask 앱 실행
    socketio.run(app, port=9000, debug=True)