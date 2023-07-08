import serial
import json
import threading
import time

arduino1  = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
arduino4  = serial.Serial('/dev/ttyAMA3', 9600, timeout=1)

def AD1_Thread():

    while True:
        
        json_str = arduino1.readline().decode('utf-8').rstrip()

        try:
            data = json.loads(json_str)

            AD1_Ir = data["IR_Sensor"]
            AD1_Temp = data["Temperature"]
            AD1_Hum = data["Humidity"]
            
            json_data = {
                "IR_Sensor": AD1_Ir,
                "Temperature" : AD1_Temp,
                "Humidity": AD1_Hum
            }

            json_output = json.dumps(json_data)
            print(json_output)
        
        except json.JSONDecodeError:
            print("Invalid Json Data : ", json_str )


def AD4_Thread():
    while True :
        json_str = arduino4.readline().decode('utf-8').rstrip()

        try:
            data = json.loads(json_str)
            AD4_NFC = data["NFC"]
            AD4_WL_CNNT = data["WL_CNNT"]
            AD4_WL_NCNNT = data["WL_NCNNT"]

            json_data = {
                "NFC" : AD4_NFC,
                "WL_CNNT" : AD4_WL_CNNT,
                "WL_NCNNT" : AD4_WL_NCNNT
            }

            json_output = json.dumps(json_data)
            print(json_output)
        except json.JSONDecodeError:
            print("Invalid Json Data : ", json_str)

arduino1_thread= threading.Thread(target=AD1_Thread)
arduino4_Thread = threading.Thread(target=AD4_Thread)

arduino1_thread.start()
arduino4_Thread.start()

arduino1_thread.join()
arduino4_Thread.join()