import io
import logging
import socketserver
from http import server
from threading import Condition
from flask import Flask, render_template, Response
from picamera import PiCamera
import simplejpeg
from flask_socketio import SocketIO

app = Flask(__name__)
socketio = SocketIO(app)

PAGE_1 = """\
<html>
<head>
<title>PiCamera Streaming Demo - Camera 1</title>
<script src="https://cdnjs.cloudflare.com/ajax/libs/socket.io/2.3.1/socket.io.js"></script>
<script type="text/javascript">
    var socket1 = io('/stream1');

    socket1.on('image', function(image) {
        document.getElementById('stream1').src = 'data:image/jpeg;base64,' + image;
    });
</script>
</head>
<body>
<h1>PiCamera Streaming Demo - Camera 1</h1>
<img id="stream1" width="640" height="480" />
</body>
</html>
"""

PAGE_2 = """\
<html>
<head>
<title>PiCamera Streaming Demo - Camera 2</title>
<script src="https://cdnjs.cloudflare.com/ajax/libs/socket.io/2.3.1/socket.io.js"></script>
<script type="text/javascript">
    var socket2 = io('/stream2');

    socket2.on('image', function(image) {
        document.getElementById('stream2').src = 'data:image/jpeg;base64,' + image;
    });
</script>
</head>
<body>
<h1>PiCamera Streaming Demo - Camera 2</h1>
<img id="stream2" width="640" height="480" />
</body>
</html>
"""


class StreamingOutput(io.BufferedIOBase):
    def __init__(self, camera_id):
        self.frame = None
        self.condition = Condition()
        self.camera_id = camera_id

    def write(self, buf):
        with self.condition:
            self.frame = buf
            self.condition.notify_all()


class StreamingHandler(server.BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/':
            self.send_response(301)
            self.send_header('Location', '/camera1')
            self.end_headers()
        elif self.path == '/camera1':
            self.send_response(200)
            self.send_header('Content-Type', 'text/html')
            self.end_headers()
            self.wfile.write(PAGE_1.encode('utf-8'))
        elif self.path == '/camera2':
            self.send_response(200)
            self.send_header('Content-Type', 'text/html')
            self.end_headers()
            self.wfile.write(PAGE_2.encode('utf-8'))
        elif self.path == '/stream1.mjpg':
            self.stream_camera(1)
        elif self.path == '/stream2.mjpg':
            self.stream_camera(2)
        else:
            self.send_error(404)
            self.end_headers()

    def stream_camera(self, camera_id):
        if camera_id == 1:
            output = output1
        elif camera_id == 2:
            output = output2
        else:
            self.send_error(404)
            self.end_headers()
            return

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
                # Send the frame to Flask-SocketIO
                socketio.emit('image', frame, namespace=f'/stream{camera_id}')
        except Exception as e:
            logging.warning(
                'Removed streaming client %s: %s',
                self.client_address, str(e))


class StreamingServer(socketserver.ThreadingMixIn, server.HTTPServer):
    allow_reuse_address = True
    daemon_threads = True


if __name__ == '__main__':
    camera1 = PiCamera(resolution=(640, 480))
    camera2 = PiCamera(resolution=(640, 480))
    output1 = StreamingOutput(camera_id=1)
    output2 = StreamingOutput(camera_id=2)

    camera1.start_recording(simplejpeg.jpegenc.SimJpegEnc(output1), format='mjpeg')
    camera2.start_recording(simplejpeg.jpegenc.SimJpegEnc(output2), format='mjpeg')

    try:
        address = ('', 8000)
        server = StreamingServer(address, StreamingHandler)
        socketio.init_app(app)
        socketio.start_background_task(server.serve_forever)
        app.run(host='0.0.0.0', port=9000, debug=False)
    finally:
        camera1.stop_recording()
        camera2.stop_recording()
        camera1.close()
        camera2.close()