import cv2
from flask import Flask, render_template
from flask_socketio import SocketIO, emit
import threading

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret'
socketio = SocketIO(app)

# 카메라 초기화
cameras = {
    'A': cv2.VideoCapture(0),
    'C': cv2.VideoCapture(1)
}

def emit_camera_frames(camera_port):
    camera = cameras[camera_port]

    while True:
        ret, frame = camera.read()

        if not ret:
            break

        ret, buffer = cv2.imencode('.jpg', frame)
        frame_bytes = buffer.tobytes()

        socketio.emit('image', {'image_data': frame_bytes, 'camera_port': camera_port}, namespace='/camera')
    
    camera.release()

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/camera/<camera_port>')
def camera_page(camera_port):
    return render_template('camera.html', camera_port=camera_port)

@socketio.on('connect', namespace='/camera')
def connect():
    print('Client connected')

@socketio.on('disconnect', namespace='/camera')
def disconnect():
    print('Client disconnected')

if __name__ == '__main__':
    # 멀티 스레딩으로 카메라 영상 송출
    for camera_port in cameras:
        thread = threading.Thread(target=emit_camera_frames, args=(camera_port,))
        thread.daemon = True
        thread.start()

    socketio.run(app, host='0.0.0.0', port=9000)