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

original_result = {'IR_Sensor':None, 'Temperature':None, 'Humidity':None,'AD2_RCV_CGuard':None ,'AD3_RCV_WGuard_Wave':None, 'NFC': None, 'WL_CNNT':None, 'WL_NCNNT':None}
is_send_mqtt = False

client = mqtt.Client(client_id='TEAM_ONE')
client.connect('210.119.12.83', 1883)
if client.is_connected:
    print('MQTT 연결성공!')
else:
    print('MQTT 연결실패!')

def AD1_Thread():
    global original_result, is_send_mqtt, client
    while True:
        if arduino1.in_waiting > 0:
            json_str = arduino1.readline().decode('utf-8').rstrip()

            try:
                data = json.loads(json_str)

                AD1_Ir = data["IR_Sensor"]
                AD1_Temp = data["Temperature"]
                AD1_Hum = data["Humidity"]
                
                if is_send_mqtt == False :
                    original_result['IR_Sensor'] = AD1_Ir
                    original_result['Temperature'] = AD1_Temp
                    original_result['Humidity'] = AD1_Hum

                json_data = json.dumps(original_result)
                client.publish(topic='TEAM_ONE/parking/data/', payload=json_data)
                
                # json_output = json.dumps(json_data)

                # print(json_output)

            except json.JSONDecodeError:
                print("Invalid Json Data_1: ", json_str )

def AD2_Thread():
    global original_result, is_send_mqtt, client
    def Get_Json():
        try:
            json_str = arduino2.readline().decode()
            data = json.loads(json_str)

            AD2_RCV_CGuard = data["AD2_RCV_CGuard"]
            json_data = {
                "AD2_RCV_CGuard" : AD2_RCV_CGuard
            }


            # json_output = json.dumps(json_data)
            # print(json_output)

            # if is_send_mqtt == False:
                # original_result['AD2_RCV_CGuard'] = AD2_RCV_CGuard

        except json.JSONDecodeError:
            print("Invaild Json Data_2: ", json_str)

    while True:
        user_input = input("Enter '1' or '-1' : ")
        if user_input == '1':
            arduino2.write(b'1')
            time.sleep(1)
            Get_Json()
        elif user_input == '-1':
            arduino2.write(b'-1')
            time.sleep(1)
            Get_Json()
        else:
            print("Invalid input. Please enter '1' or '0'.")

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
                    # json_data = {
                    #     "AD3_RCV_WGuard_Wave": AD3_RCV_WGuard_Wave
                    # }
                    # json_output = json.dumps(json_data)
                    # print(json_output)
                    if is_send_mqtt == False:
                        original_result['AD3_RCV_WGuard_Wave'] = AD3_RCV_WGuard_Wave

                    print(original_result)

                except json.JSONDecodeError:
                    print("Invalid Json Data_3:", json_str)

    def hand_input():
        while True:
            user_input = input("Enter a command: ")  # 사용자 입력 처리
            if user_input == "1":
                arduino3.write(b'1')
            elif user_input == "0":
                arduino3.write(b'0')
            else:
                print("Invalid command")

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
                AD4_Nfc = data["NFC"]
                AD4_WL_Cnnt = data["WL_CNNT"]
                AD4_WL_NCnnt = data["WL_NCNNT"]

                # json_data = {
                #     "NFC" : AD4_NFC,
                #     "WL_CNNT" : AD4_WL_CNNT,
                #     "WL_NCNNT" : AD4_WL_NCNNT
                # }
                # print(json_output)

                if is_send_mqtt == False :
                    original_result['NFC'] = AD4_Nfc
                    original_result['WL_CNNT'] = AD4_WL_Cnnt
                    original_result['WL_NCNNT'] = AD4_WL_NCnnt

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

arduino1_Thread.join()
arduino2_Thread.join()
arduino3_Thread.join()
arduino4_Thread.join()
