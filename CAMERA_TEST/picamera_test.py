import cv2
from picamera2 import Picamera2 as cam2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import threading

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 초기화
# camera = cv2.VideoCapture(0)
picam = cam2()
picam.preview_configuration.size=(800,600)
picam.preview_configuration.main.format='RGB888'
picam.preview_configuration.align()


def emit_camera_frames():
    picam.configure('preview')
    picam.start()

    while True:
        success, frame = picam.read()
        if not success:
            break
        
        # 영상 데이터를 바이트 스트림으로 인코딩
        ret, buffer = cv2.imencode('.jpg', frame)
        frame_data = buffer.tobytes()

        # 웹소켓을 통해 클라이언트에게 영상 데이터 전송
        socketio.emit('video_stream', frame_data, binary=True)

        # 100ms마다 영상 전송
        socketio.sleep(0.1)
    
@app.route('/')
def index():
    return render_template('index.html')

if __name__ == '__main__':
    # 멀티 스레딩으로 카메라 영상 송출
    # for camera_port in camera:
    thread = threading.Thread(target=emit_camera_frames)
    thread.daemon = True
    thread.start()

    socketio.run(app, host='0.0.0.0', port=9000)
