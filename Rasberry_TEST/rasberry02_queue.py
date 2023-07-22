from threading import Thread, Timer
import paho.mqtt.client as mqtt
import queue
import json
import serial

# Serial 통신 포트
AD1_PORT='/dev/ttyS0'
AD2_PORT='/dev/ttyAMA1'
AD3_PORT='/dev/ttyAMA2'
AD4_PORT='/dev/ttyAMA3'

# MQTT 통신 변수
BROKER = '210.119.12.83'
PORT=1883

# MQTT publisher(보내기) Thread
class Publisher(Thread):
    def __init__(self, sensor_queue): # 초기화
        Thread.__init__(self)   # 스레드 초기화

        # 쓰레드 사이에 센서값 전달을 위해 queue를 사용함.
        self.sensor_queue = sensor_queue  # 참조 복사   # 센서값 담는 큐

        # MQTT 통신 변수
        self.broker = BROKER
        self.port = PORT
        self.clientId = 'IoT_Team1_Rasberry_PUB'
        self.topic = 'TEAM_ONE/parking/Sensor_data/'

        # Publish 클라인언트 생성
        self.client = mqtt.Client(client_id=self.clientId)
        # 호출함수 등록
        self.client.on_connect = self.onConnect

    def onConnect(self, mqttc, obj, flags, rc):
        if rc == 0:
            print("MQTT Publisher 연결 성공")
        else:
            print("MQTT Publisher 연결 실패, result code:", rc)

    def run(self):
        # mqtt_broker 연결
        self.client.connect(self.broker, self.port)
        self.publish_data_auto()    # 자동 publish 함수

    def publish_data_auto(self):
        # 큐에서 모든 센서 값을 가져와 딕셔너리에 저장
        sensor_data = {'AD1_RCV_IR_Sensor':None, 'AD1_RCV_Temperature':None, 'AD1_RCV_Humidity':None, 'AD1_RCV_Dust': None, # 아두이노1
                       'AD2_RCV_CGuard':None ,                                                                              # 아두이노2
                       'AD3_RCV_WGuard_Wave':None,                                                                          # 아두이노3
                       'AD4_RCV_NFC': None, 'AD4_RCV_WL_CNNT':None, 'AD4_RCV_WL_NCNNT':None}                                # 아두이노4
        while True:
            while not self.sensor_queue.empty():   # 큐가 비워질때까지 루프
                new_sensor_data = self.sensor_queue.get()    # 큐에서 하나 get
                sensor_data.update(new_sensor_data) # 새 센서 데이터를 업데이트(합치기)

                # 딕셔너리를 json형태로 변환하여 mqtt 브로커로 전송
                self.client.publish(topic=self.topic, payload=json.dumps(sensor_data))

# MQTT subscriber(받아오기) Thread
class Subscriber(Thread):
    def __init__(self, command_queue):
        Thread.__init__(self)

        # 쓰레드 사이에 명령값 전달을 위해 queue를 사용함.
        self.command_queue = command_queue  # 명령값을 담는 큐

        # MQTT 통신 변수
        self.broker = BROKER
        self.port = PORT
        self.clientId = 'IoT_Team1_Rasberry_SUB'
        self.topic = 'TEAM_ONE/parking/Control_data/'

         # subscriber 클라인언트 생성
        self.client = mqtt.Client(client_id=self.clientId)

        # 콜백함수 등록
        self.client.on_connect = self.onConnect
        self.client.on_message = self.onMessage


    def onConnect(self, mqttc, obj, flags, rc): # 연결 성공시 호출되는 콜백함수
        if rc == 0:
            print("MQTT Publisher 연결 성공")
            self.client.subscribe(topic=self.topic)     # 구독
        else:
            print("MQTT Publisher 연결 실패, result code:", rc)


    def onMessage(self, mqttc, obj, msg):   # 특정 토픽에서 메세지를 받았을때 호출되는 콜백함수
        rcv_msg = str(msg.payload.decode('utf-8'))
        data = json.loads(rcv_msg)

        self.command_queue.put(data)

    def run(self):
        self.client.connect(self.broker, self.port)   # MQTT브로커 연결
        self.client.loop_forever()  # MQTT브로커와 통신을 유지하며 메시지를 주고 받을 수 있도록 루프를 돌며 대기하는 기능

# Serial arduino Thread
class Arduino(Thread):
    def __init__(self, arduino_type, arduino_port, sensor_queue, command_queue):
        Thread.__init__(self)
        self.arduino_type = arduino_type
        self.arduino = serial.Serial(arduino_port, 9600, timeout=1) # 시리얼 객체 생성
        self.sensor_queue = sensor_queue
        self.command_queue = command_queue

    # 시리얼 통신 아두이노 센서 값 읽어오기
    def read_arduino_value(self):
        if self.arduino.in_waiting > 0:
            json_str = self.arduino.readline().decode('utf-8').rstrip() # 시리얼 한줄단위로 읽고 str형태로

            try:
                data = json.loads(json_str) # json형식으로 파싱
                self.sensor_queue.put(data)    # 센서 큐에 저장

            except json.JSONDecodeError:
                print("Invalid Json Data: ", json_str)

    # 명령큐에서 명령 꺼내서 아두이노로 보내기
    def write_arduino_value(self):
        if not self.command_queue.empty():
            commands = self.command_queue.get()
            for command_name, command in commands.items():
                if self.arduino_type in command_name:
                    command_str = json.dumps({command_name: command})
                    self.arduino.write(command_str.encode())

    def run(self):
        while True:
            self.read_arduino_value()
            self.write_arduino_value()



if __name__ == '__main__':
    # 센서 값을 담을 큐 생성
    sensor_queue = queue.Queue()
    # 명령 값을 담을 큐 생성
    command_queue = queue.Queue()

    # MQTT Publisher 쓰레드 생성
    publisher_thread = Publisher(sensor_queue)

    # MQTT Subscriber 쓰레드 생성
    subscriber_thread = Subscriber(command_queue)

    # 아두이노 1,2,3,4 쓰레드 생성
    arduino1_thread = Arduino('AD1',AD1_PORT, sensor_queue, command_queue)
    # arduino2_thread = Arduino('AD2',AD2_PORT, sensor_queue, command_queue)
    # arduino3_thread = Arduino('AD3', AD3_PORT, sensor_queue, command_queue)
    # arduino4_thread = Arduino('AD4', AD4_PORT, sensor_queue, command_queue)

    # 쓰레드 시작
    publisher_thread.start()
    subscriber_thread.start()
    arduino1_thread.start()
    # arduino2_thread.start()
    # arduino3_thread.start()
    # arduino4_thread.start()

    # 쓰레드 종료 대기
    publisher_thread.join()
    subscriber_thread.join()
    arduino1_thread.join()
    # arduino2_thread.join()
    # arduino3_thread.join()
    # arduino4_thread.join()