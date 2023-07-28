import cv2
import io
import threading
from flask import Flask, render_template, Response
from flask_socketio import SocketIO

app = Flask(__name__)
socketio = SocketIO(app)

# 카메라 초기화
camera = cv2.VideoCapture(0)
camera.set(3, 640)  # 가로 해상도 설정
camera.set(4, 480)  # 세로 해상도 설정

# 웹소켓으로 영상 전송하는 함수
def send_video():
    while True:
        success, frame = camera.read()
        if not success:
            break
        
        # 영상 데이터를 바이트 스트림으로 인코딩
        ret, buffer = cv2.imencode('.jpg', frame)
        frame_data = buffer.tobytes()

        # 웹소켓을 통해 클라이언트에게 영상 데이터 전송
        socketio.emit('video_stream', frame_data, binary=True)

        # 100ms마다 영상 전송
        socketio.sleep(0.1)

# 웹 페이지 렌더링
@app.route('/')
def index():
    return render_template('index.html')

# 웹소켓 연결
@socketio.on('connect')
def on_connect():
    print('Client connected')
    global video_thread
    if not video_thread.is_alive():
        video_thread = socketio.start_background_task(send_video)

# 웹소켓 연결 해제
@socketio.on('disconnect')
def on_disconnect():
    print('Client disconnected')

if __name__ == '__main__':
    video_thread = threading.Thread(target=send_video)
    video_thread.daemon = True
    socketio.run(app, host='0.0.0.0', port=9000, debug=False)
