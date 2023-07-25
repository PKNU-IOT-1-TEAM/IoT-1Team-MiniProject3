from ast import Try
from picamera2 import Picamera2
from picamera2 import Picamera2
from picamera2.encoders import JpegEncoder
from picamera2.outputs import FileOutput
from threading import Condition
from http import server
from flask import Flask, render_template, Response
from flask_socketio import SocketIO
import RPi.GPIO as gp
import time
import os
import socketserver

app = Flask(__name__)
socketio = SocketIO(app)

PAGE = """\
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

width = 320
height = 240 

adapter_info = {  
    "A" : {   
        "i2c_cmd":"i2cset -y 10 0x70 0x00 0x04",
        "gpio_sta":[0,0,1],
    }, "B" : {
        "i2c_cmd":"i2cset -y 10 0x70 0x00 0x06",
        "gpio_sta":[0,1,0],
    }
}

class WorkThread(QThread):

    def __init__(self):
        super(WorkThread,self).__init__()
        gp.setwarnings(False)
        gp.setmode(gp.BOARD)
        gp.setup(7, gp.OUT)
        gp.setup(11, gp.OUT)
        gp.setup(12, gp.OUT)


    def select_channel(self,index):
        channel_info = adapter_info.get(index)
        if channel_info == None:
            print("Can't get this info")
        gpio_sta = channel_info["gpio_sta"] # gpio write
        gp.output(7, gpio_sta[0])
        gp.output(11, gpio_sta[1])
        gp.output(12, gpio_sta[2])

    def init_i2c(self,index):
        channel_info = adapter_info.get(index)
        os.system(channel_info["i2c_cmd"]) # i2c write

    def run(self):
        global picam2
        # picam2 = Picamera2()
        # picam2.configure( picam2.still_configuration(main={"size": (320, 240),"format": "BGR888"},buffer_count=1))

        flag = False

        for item in {"A","B"}:
            try:
                self.select_channel(item)
                self.init_i2c(item)
                time.sleep(0.5) 
                if flag == False:
                    flag = True
                else :
                    picam2.close()
                    # time.sleep(0.5) 
                print("init1 "+ item)
                picam2 = Picamera2()
                picam2.configure(picam2.create_still_configuration(main={"size": (320, 240),"format": "BGR888"},buffer_count=2)) 
                picam2.start()
                time.sleep(2)
                picam2.capture_array(wait=False)
                time.sleep(0.1)
            except Exception as e:
                print("except: "+str(e))

        while True:
            for item in {"A","B"}:
                self.select_channel(item)
                time.sleep(0.02)
                try:
                    buf = picam2.capture_array()
                    buf = picam2.capture_array()

                    if item == 'A':
                        image_label.setPixmap(pixmap)
                    elif item == 'B':
                        image_label2.setPixmap(pixmap)
                except Exception as e:
                    print("capture_buffer: "+ str(e))


# picam2 = Picamera2()

work = WorkThread()

if __name__ == "__main__":

    work.start()
 
    app.exec()
    work.quit()
    picam2.close()

