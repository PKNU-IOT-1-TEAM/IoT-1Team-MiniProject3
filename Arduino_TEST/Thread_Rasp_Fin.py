import serial
import json
import threading
import time
arduino1  = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
arduino2  = serial.Serial('/dev/ttyAMA1', 9600, timeout=1)
arduino3  = serial.Serial('/dev/ttyAMA2', 9600, timeout=1)
arduino4  = serial.Serial('/dev/ttyAMA3', 9600, timeout=1)

def AD1_Thread():
    
    while True:
        json_str = arduino1.readline().decode('utf-8').rstrip()

        try:
            data = json.loads(json_str)

            AD1_IR = data["IR_Sensor"]
            AD1_Temp = data["Temperature"]
            AD1_Hum = data["Humidity"]
            
            json_data = {
                "IR_Sensor": AD1_IR,
                "Temperature" : AD1_Temp,
                "Humidity":AD1_Hum
            }

            json_output = json.dumps(json_data)
            print(json_output)
        
        except json.JSONDecodeError:
            print("Invalid Json Data : ", json_str )

        arduino1.close()
    
def AD2_Thread():
    def Get_Json(self):
        try:
            json_str = arduino2.readline().decode()
            data = json.loads(json_str)

            AD2_RCV_CGuard = data["AD2_RCV_CGuard"]
            json_data = {
                "AD2_RCV_CGuard" : AD2_RCV_CGuard
            }
            json_output = json.dumps(json_data)
            print(json_output)  
        except json.JSONDecodeError:
            print("Invaild Json Data : ", json_str)

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
    while True:
        json_str = arduino3.readline().decode('utf-8').rstrip()


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

        arduino4.close()

arduino1_Thread = threading.Thread(target=AD1_Thread)
arduino2_Thread = threading.Thread(target=AD2_Thread)
arduino3_Thread = threading.Thread(target=AD3_Thread)
arduino4_Thread = threading.Thread(target=AD4_Thread)

arduino1_Thread.start()
arduino2_Thread.start()
arduino3_Thread.start()
arduino4_Thread.start()