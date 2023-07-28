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
        frame = picam.capture_array()

        # 웹소켓을 통해 클라이언트에게 영상 데이터 전송
        socketio.emit('video_stream', frame)

        # 100ms마다 영상 전송
        socketio.sleep(0.1)
        # cv2.imshow('piCam', frame)

    # cv2.destroyAllWindows()
    
@app.route('/')
def index():
    return render_template('index.html')

#  @app.route('/camera/<camera_port>')
# def camera_page(camera_port):
#     return render_template('camera.html', camera_port=camera_port)

@socketio.on('connect', namespace='/camera')
def connect():
    print('Client connected')

@socketio.on('disconnect', namespace='/camera')
def disconnect():
    print('Client disconnected')

if __name__ == '__main__':
    # 멀티 스레딩으로 카메라 영상 송출
    # for camera_port in camera:
    thread = threading.Thread(target=emit_camera_frames)
    thread.daemon = True
    thread.start()

    socketio.run(app, host='0.0.0.0', port=9000)
