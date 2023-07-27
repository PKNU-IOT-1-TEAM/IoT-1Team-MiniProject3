import time
from threading import Thread
from flask import Flask, render_template, Response
from flask_socketio import SocketIO, emit
import picamera

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret_key'
socketio = SocketIO(app)

# 카메라 초기화
camera_a = picamera.PiCamera(camera_num=0)
camera_c = picamera.PiCamera(camera_num=2)

# 카메라 해상도 설정 (옵션)
# camera_a.resolution = (640, 480)
# camera_c.resolution = (640, 480)

# 웹 소켓 통신을 위한 스레드
def background_thread():
    while True:
        # 카메라 A 캡처 및 웹소켓 전송
        with camera_a as camera:
            image_a = camera.capture_bytes(format='jpeg')
            socketio.emit('image_a', image_a, broadcast=True)

        # 카메라 C 캡처 및 웹소켓 전송
        with camera_c as camera:
            image_c = camera.capture_bytes(format='jpeg')
            socketio.emit('image_c', image_c, broadcast=True)

        time.sleep(0.1)

# 웹 소켓 연결 시 처리
@socketio.on('connect')
def on_connect():
    print('Client connected')
    emit('message', 'Connected')
    emit('start_streaming')  # 스트리밍 시작 요청

# 웹 소켓 연결 종료 시 처리
@socketio.on('disconnect')
def on_disconnect():
    print('Client disconnected')

# 카메라 A 이미지 전송
@socketio.on('request_image_a')
def request_image_a():
    with camera_a as camera:
        image_a = camera.capture_bytes(format='jpeg')
        emit('image_a', image_a)

# 카메라 C 이미지 전송
@socketio.on('request_image_c')
def request_image_c():
    with camera_c as camera:
        image_c = camera.capture_bytes(format='jpeg')
        emit('image_c', image_c)

# 카메라 A 화면 렌더링
@app.route('/camera_a')
def camera_a():
    return render_template('camera_a.html')

# 카메라 C 화면 렌더링
@app.route('/camera_c')
def camera_c():
    return render_template('camera_c.html')

if __name__ == '__main__':
    # 웹 소켓 스레드 시작
    socket_thread = Thread(target=background_thread)
    socket_thread.daemon = True
    socket_thread.start()

    # Flask 앱 실행
    socketio.run(app, host='0.0.0.0', port=9000)
