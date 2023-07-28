import cv2
import threading
from flask import Flask, render_template, Response

app = Flask(__name__)

camera_a = cv2.VideoCapture(0)  # A 카메라 포트
if not camera_a.isOpened():
    raise Exception("A 포트 카메라를 찾을 수 없습니다. 인덱스를 확인해주세요.")
camera_c = cv2.VideoCapture(1)  # D 카메라 포트
if not camera_c.isOpened():
    raise Exception("D 포트 카메라를 찾을 수 없습니다. 인덱스를 확인해주세요.")

camera_a.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
camera_a.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

camera_c.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
camera_c.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

frame_a = None
frame_c = None
lock = threading.Lock()

def video_stream(camera):
    global frame_a, frame_c

    while True:
        ret, frame = camera.read()
        if not ret:
            break

        with lock:
            if camera == camera_a:
                frame_a = frame.copy()
            else:
                frame_c = frame.copy()

thread_a = threading.Thread(target=video_stream, args=(camera_a,))
thread_a.daemon = True
thread_a.start()

thread_c = threading.Thread(target=video_stream, args=(camera_c,))
thread_c.daemon = True
thread_c.start()

def gen_frames(camera):
    global frame_a, frame_c

    while True:
        with lock:
            if camera == 'a' and frame_a is not None:
                ret, buffer = cv2.imencode('.jpg', frame_a)
            elif camera == 'c' and frame_c is not None:
                ret, buffer = cv2.imencode('.jpg', frame_c)
            else:
                continue

        if not ret:
            continue

        frame = buffer.tobytes()
        yield (b'--frame\r\n'
               b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')

# 각각의 웹페이지 라우트
@app.route('/camera_a')
def camera_a():
    return render_template('camera_a.html')

@app.route('/camera_c')
def camera_c():
    return render_template('camera_c.html')

# 영상 스트리밍 라우트
@app.route('/video_feed_a')
def video_feed_a():
    return Response(gen_frames('a'), mimetype='multipart/x-mixed-replace; boundary=frame')

@app.route('/video_feed_c')
def video_feed_c():
    return Response(gen_frames('c'), mimetype='multipart/x-mixed-replace; boundary=frame')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=9000, debug=True)
