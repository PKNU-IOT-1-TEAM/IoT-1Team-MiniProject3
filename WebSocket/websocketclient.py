from flask import Flask, render_template, Response
from flask_socketio import SocketIO, emit
import cv2
import json

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret_key'
socketio = SocketIO(app)
camera = cv2.VideoCapture(0)

@app.route('/')
def sessions():
    return 'Hello world'

def generate_frames():
    while True:
        success, frame = camera.read()
        if not success:
            break
        ret, buffer = cv2.imencode('.jpg', frame)
        frame = buffer.tobytes()
        yield (b'--frame\r\n'
               b'Content-Tye: image/jpeg\r\n\r\n' + frame + b'\r\n\r\n')

@app.route('/video_feed')
def video_feed():
    return Response(generate_frames(),
                    mimetype='multipart/x-mixed-replace; boundary=frame')

@socketio.on('connect')
def handle_connect():
    print('Client connected')

@socketio.on('disconnect')
def handle_disconnect():
    print('Client disconnected')

@socketio.on('message')
def handle_message(message):
    print('Received message :', message)
    socketio.emit('message', message, broadcast = True)

if __name__ == '__main__':
    socketio.run(app, host='0.0.0.0', port=8000, debug = True)
