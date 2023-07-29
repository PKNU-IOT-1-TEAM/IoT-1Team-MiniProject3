import io
import logging
import socketserver
from http import server
from threading import Condition

from flask import Flask, render_template, Response
from picamera2 import Picamera2
from picamera2.encoders import JpegEncoder
from picamera2.outputs import FileOutput
from flask_socketio import SocketIO

# Flask 앱을 초기화
app = Flask(__name__)
socketio = SocketIO(app)

# 웹 페이지 템플릿
PAGE = """
<html>
<head>
<title>PiCamera Streaming Demo</title>
<script src="https://cdnjs.cloudflare.com/ajax/libs/socket.io/2.3.1/socket.io.js"></script>
<script type="text/javascript">
    var socket = io();
    socket.on('connect', function() {
        console.log('Connected to Flask-SocketIO');
    });
    socket.on('image', function(image) {
        document.getElementById('stream').src = 'data:image/jpeg;base64,' + image;
    });
</script>
</head>
<body>
<h1>PiCamera Streaming Demo</h1>
<img id="stream" width="640" height="480" />
</body>
</html>
"""

# StreamingOutput 클래스 정의 - 프레임 스트리밍을 위한 출력 클래스
class StreamingOutput(io.BufferedIOBase):
    def __init__(self):
        self.frame = None
        self.condition = Condition()

    def write(self, buf):
        with self.condition:
            self.frame = buf
            self.condition.notify_all()

# StreamingHandler 클래스 정의 - HTTP 요청을 처리하는 핸들러 클래스
class StreamingHandler(server.BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/':
            self.send_response(301)
            self.send_header('Location', '/stream.mjpg')
            self.end_headers()
        # elif self.path == '/index.html':
        #     content = PAGE.encode('utf-8')
        #     self.send_response(200)
        #     self.send_header('Content-Type', 'text/html')
        #     self.send_header('Content-Length', len(content))
        #     self.end_headers()
        #     self.wfile.write(content)
        elif self.path == '/stream.mjpg':
            self.send_response(200)
            self.send_header('Age', 0)
            self.send_header('Cache-Control', 'no-cache, private')
            self.send_header('Pragma', 'no-cache')
            self.send_header('Content-Type', 'multipart/x-mixed-replace; boundary=FRAME')
            self.end_headers()
            try:
                while True:
                    with output.condition:
                        output.condition.wait()
                        frame = output.frame
                    self.wfile.write(b'--FRAME\r\n')
                    self.send_header('Content-Type', 'image/jpeg')
                    self.send_header('Content-Length', len(frame))
                    self.end_headers()
                    self.wfile.write(frame)
                    self.wfile.write(b'\r\n')
                    # 프레임을 Flask-SocketIO로 보냄
                    socketio.emit('image', frame, namespace='/stream')
            except Exception as e:
                logging.warning(
                    'Removed streaming client %s: %s',
                    self.client_address, str(e))
        else:
            self.send_error(404)
            self.end_headers()

# StreamingServer 클래스 정의 - HTTP 서버를 생성하는 클래스
class StreamingServer(socketserver.ThreadingMixIn, server.HTTPServer):
    allow_reuse_address = True
    daemon_threads = True

# Flask 라우트와 기능들을 정의
@app.route('/')
def index():
    return render_template('index.html')

@app.route('/video_feed')
def video_feed():
    return Response(gen(),
                    mimetype='multipart/x-mixed-replace; boundary=frame')

def gen():
    while True:
        frame = output.frame
        if frame is not None:
            yield (b'--frame\r\n'
                   b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n\r\n')

if __name__ == '__main__':
    # Picamera2를 초기화하고 설정
    picamera = Picamera2()
    picamera.resolution = (640, 480)
    output = StreamingOutput()
    picamera.start_recording(JpegEncoder(), FileOutput(output))

    try:
        # 서버 설정 및 백그라운드에서 서버를 실행
        address = ('', 9000)
        server = StreamingServer(address, StreamingHandler)
        socketio.init_app(app)
        socketio.start_background_task(server.serve_forever)
        app.run(host='0.0.0.0', debug=False)
    finally:
        # 서버 실행을 마치면 녹화를 중단.
        picamera.stop_recording()
