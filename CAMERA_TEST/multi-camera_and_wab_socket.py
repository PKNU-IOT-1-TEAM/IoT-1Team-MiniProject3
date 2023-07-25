import time
import picamera
import threading
import io
from flask import Flask, render_template
from flask_socketio import SocketIO

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 1 초기화
camera1 = picamera.PiCamera()
camera1.resolution = (640, 480)
stream1 = io.BytesIO()

# 카메라 2 초기화
camera2 = picamera.PiCamera()
camera2.resolution = (640, 480)
stream2 = io.BytesIO()

# 카메라 스레드 시작
def camera_thread(camera, stream):
    while True:
        camera.capture(stream, format='jpeg')
        stream.seek(0)
        socketio.emit('image', stream.getvalue())
        stream.seek(0)
        stream.truncate()

# 페이지 라우팅
@app.route('/')
def index():
    return render_template('index.html')

@app.route('/camera1')
def camera1_view():
    return render_template('camera.html')

@app.route('/camera2')
def camera2_view():
    return render_template('camera.html')

# 웹소켓 이벤트 핸들러
@socketio.on('connect')
def on_connect():
    print('Client connected')

@socketio.on('disconnect')
def on_disconnect():
    print('Client disconnected')

# 카메라 스레드 시작
camera1_thread = threading.Thread(target=camera_thread, args=(camera1, stream1))
camera1_thread.daemon = True
camera1_thread.start()

camera2_thread = threading.Thread(target=camera_thread, args=(camera2, stream2))
camera2_thread.daemon = True
camera2_thread.start()

if __name__ == '__main__':
    # 웹소켓 서버 실행
    socketio.run(app, host='0.0.0.0', port=5000)