from threading import Thread, Timer
import time
import json
import datetime as dt
import paho.mqtt.client as mqtt


class publisher(Thread):
    def __init__(self):
        Thread.__init__(self)   # 스레드 초기화
        self.host = '210.119.12.61'
        self.port = 1883
        self.clientId = 'IoT_Team1_PUB'
        self.count = 0
        print('publisher 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientId)

    def run(self):
        self.client.connect(self.host, self.port)
        self.publish_data_auto()

    def publish_data_auto(self):

        origin_data = { 'temp' : 12,
                        'humid' : 30 }
        pub_data = json.dumps(origin_data)
        self.client.publish(topic='pknu/rpi/control/', payload=pub_data)
        print(f'Data published #{self.count}')
        self.count += 1
        Timer(2.0, self.publish_data_auto).start()

class subscriber(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.host = '210.119.12.61'
        self.port = 1883
        self.clientId = 'IoT_Team1_SUB'
        self.topic = 'pknu/monitor/control/'
        print('subscriber 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientId)

    def run(self):
        self.client.on_connect = self.onConnect
        self.client.on_message = self.onMessage
        self.client.connect(self.host, self.port)
        self.client.subscribe(topic=self.topic)
        self.client.loop_forever()

    def onConnect(self, mqttc, obj, flags, rc):
        print(f'subscriber 연결됨 rc > {rc}')

    def onMessage(self, mqttc, obj, msg):
        rcv_msg = str(msg.payload.decode('utf-8'))
        data = json.loads(rcv_msg)
        stat = data['STAT']
        print(f'현재 STAT : {stat}')
        time.sleep(1.0)

if __name__ == '__main__':
    thPub = publisher()
    thSub = subscriber()
    thPub.start()
    thSub.start()
    
        