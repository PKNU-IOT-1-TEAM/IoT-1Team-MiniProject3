import os
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import RPi.GPIO as GPIO
from time import sleep
import threading
from picamera import PiCamera

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret!'
socketio = SocketIO(app)

# Arducam에서 사용할 GPIO 핀 번호
camera1_pin = 17
camera2_pin = 18

# 각 카메라를 위한 PiCamera 객체
camera1 = PiCamera()
camera2 = PiCamera()

# 카메라 해상도 설정 (옵션)
camera1.resolution = (640, 480)
camera2.resolution = (640, 480)

# 카메라 스트리밍 스레드 종료를 위한 변수
thread_stop = False

def camera_stream(camera, pin, namespace):
    global thread_stop
    stream_url = f'/video_stream/{pin}'
    camera.start_preview()
    camera.preview_fullscreen = False
    camera.preview_window = (0, 0, 640, 480)

    while not thread_stop:
        try:
            camera.capture(stream_url, use_video_port=True, format='jpeg')
            socketio.emit('image_update', {'pin': pin, 'image_url': stream_url}, namespace=namespace)
        except Exception as e:
            print(f"Error capturing image from Camera {pin}: {e}")
        sleep(0.1)

    camera.stop_preview()

@app.route('/')
def index():
    return render_template('index.html')

@socketio.on('connect', namespace='/camera1')
def camera1_connect():
    print('Camera 1 Client connected')
    if not thread1.is_alive():
        thread1.start()

@socketio.on('connect', namespace='/camera2')
def camera2_connect():
    print('Camera 2 Client connected')
    if not thread2.is_alive():
        thread2.start()

@socketio.on('disconnect', namespace='/camera1')
def camera1_disconnect():
    print('Camera 1 Client disconnected')

@socketio.on('disconnect', namespace='/camera2')
def camera2_disconnect():
    print('Camera 2 Client disconnected')

if __name__ == '__main__':
    GPIO.setmode(GPIO.BCM)
    GPIO.setup(camera1_pin, GPIO.OUT)
    GPIO.setup(camera2_pin, GPIO.OUT)

    # 카메라 스트리밍을 위한 스레드 생성
    thread1 = threading.Thread(target=camera_stream, args=(camera1, camera1_pin, '/camera1'))
    thread2 = threading.Thread(target=camera_stream, args=(camera2, camera2_pin, '/camera2'))

    try:
        socketio.run(app, host='0.0.0.0', port=5000, debug=True)
    except KeyboardInterrupt:
        thread_stop = True
        thread1.join()
        thread2.join()
        GPIO.cleanup()