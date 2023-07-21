import serial
import json
import threading
import time
import paho.mqtt.client as mqtt
import datetime as dt

arduino1  = serial.Serial('/dev/ttyS0', 9600, timeout=1)
arduino2  = serial.Serial('/dev/ttyAMA1', 9600, timeout=1)
arduino3  = serial.Serial('/dev/ttyAMA2', 9600, timeout=1)
arduino4  = serial.Serial('/dev/ttyAMA3', 9600, timeout=1)

AD2_CGuard = 0
AD3_WGuard_Wave = 0

original_result = {'AD1_RCV_IR_Sensor':None, 'AD1_RCV_Temperature':None, 'AD1_RCV_Humidity':None, 'AD1_RCV_Dust':None,'AD2_RCV_CGuard':None ,'AD3_RCV_WGuard_Wave':None, 'AD4_RCV_NFC': None, 'AD4_RCV_WL_CNNT':None, 'AD4_RCV_WL_NCNNT':None}
is_send_mqtt = False

topic1='TEAM_ONE/parking/data/'
topic2='TEAM_ONE/parking/s_data/'
broker='210.119.12.83'
port=1883

def on_connect(client, userdata, flags, rc):
    print("MQTT 연결성공")
    print("Connected with result code: ", str(rc))
    client.subscribe(topic2)
    print("subscribing to topic : "+topic2)

def on_message(client, userdata, message):
    json_str = message.payload.decode('utf-8')

    data = json.loads(json_str)

    if 'AD2' in data:
        global AD2_CGuard 
        AD2_CGuard = data["AD2"]    
    
    global AD3_WGuard_Wave 
    
    if 'AD3_CNNT' in data:
        AD3_WGuard_Wave = data["AD3_CNNT"]
    elif 'AD3_NCNNT' in data:
        AD3_WGuard_Wave = data["AD3_NCNNT"]
    
    
    print("Data : " + json_str)

def main():
    print("WAIT for max: ", 2)
    while True:
        time.sleep(1)
        client.publish(topic1,"dfdfd")

### MQTT ###
client = mqtt.Client('TEAM_ONE')
client.connect(broker, port)
client.on_connect = on_connect

def subscribing():
    client.on_message = on_message
    client.loop_forever()

sub=threading.Thread(target=subscribing)

def AD1_Thread():
    global original_result, is_send_mqtt, client
    while True:
        if arduino1.in_waiting > 0:
            json_str = arduino1.readline().decode('utf-8').rstrip()

            try:
                data = json.loads(json_str)

                AD1_Ir = data["AD1_RCV_IR_Sensor"]
                AD1_Temp = data["AD1_RCV_Temperature"]
                AD1_Hum = data["AD1_RCV_Humidity"]
                AD1_Dust = data["AD1_RCV_Dust"]
                
                if is_send_mqtt == False :
                    original_result['AD1_RCV_IR_Sensor'] = AD1_Ir
                    original_result['AD1_RCV_Temperature'] = AD1_Temp
                    original_result['AD1_RCV_Humidity'] = AD1_Hum
                    original_result['AD1_RCV_Dust'] = AD1_Dust

                json_data = json.dumps(original_result)
                client.publish(topic='TEAM_ONE/parking/data/', payload=json_data)                

            except json.JSONDecodeError:
                print("Invalid Json Data_1: ", json_str )

def AD2_Thread():
    global original_result, is_send_mqtt, client
    def Get_Json():
        try:
            json_str = arduino2.readline().decode()
            data = json.loads(json_str)

            AD2_RCV_CGuard = data["AD2"]
            json_data = {
                "AD2" : AD2_RCV_CGuard
            }

        except json.JSONDecodeError:
            print("Invaild Json Data_2: ", json_str)

    while True:        
        global AD2_CGuard
        if  AD2_CGuard == 1:
            arduino2.write(b'1')
            time.sleep(1)
            Get_Json()
        elif AD2_CGuard == -1:
            arduino2.write(b'-1')
            time.sleep(1)
            Get_Json()

def AD3_Thread():
    global original_result, is_send_mqtt, client
    def read_serial():
        while True:
            if arduino3.in_waiting > 0:
                json_str = arduino3.readline().decode('utf-8').rstrip()

                try:
                    data = json.loads(json_str)
                    # AD3 데이터 처리
                    AD3_RCV_WGuard_Wave = data["AD3_RCV_WGuard_Wave"]                    
                    if is_send_mqtt == False:
                        original_result['AD3_RCV_WGuard_Wave'] = AD3_RCV_WGuard_Wave

                    print(original_result)

                except json.JSONDecodeError:
                    print("Invalid Json Data_3:", json_str)

    def hand_input():
        global AD3_WGuard_Wave
        while True:
            if AD3_WGuard_Wave == 0:
                arduino3.write(b'0')
            elif AD3_WGuard_Wave == 1:
                arduino3.write(b'1')
            elif AD3_WGuard_Wave == 2:
                arduino3.write(b'2')
            elif AD3_WGuard_Wave == 3:
                arduino3.write(b'3')

    # 중첩 스레드라서 지우면 안됌..!!!!
    serial_thread = threading.Thread(target=read_serial)
    serial_thread.start()
    hand_input()
    serial_thread.join()

def AD4_Thread():
    global original_result, is_send_mqtt, client

    while True :
        if arduino4.in_waiting > 0:
            json_str = arduino4.readline().decode('utf-8').rstrip()

            try:
                data = json.loads(json_str)
                AD4_Nfc = data["AD4_RCV_NFC"]
                AD4_WL_Cnnt = data["AD4_RCV_WL_CNNT"]
                AD4_WL_NCnnt = data["AD4_RCV_WL_NCNNT"]

                if is_send_mqtt == False :
                    original_result['AD4_RCV_NFC'] = AD4_Nfc
                    original_result['AD4_RCV_WL_CNNT'] = AD4_WL_Cnnt
                    original_result['AD4_RCV_WL_NCNNT'] = AD4_WL_NCnnt

                json_data = json.dumps(original_result)
                client.publish(topic='TEAM_ONE/parking/data/', payload=json_data)

            except json.JSONDecodeError:
                print("Invalid Json Data_4: ", json_str)


arduino1_Thread = threading.Thread(target=AD1_Thread)
arduino2_Thread = threading.Thread(target=AD2_Thread)
arduino3_Thread = threading.Thread(target=AD3_Thread)
arduino4_Thread = threading.Thread(target=AD4_Thread)

arduino1_Thread.start()
arduino2_Thread.start()
arduino3_Thread.start()
arduino4_Thread.start()

sub.start()

arduino1_Thread.join()
arduino2_Thread.join()
arduino3_Thread.join()
arduino4_Thread.join()