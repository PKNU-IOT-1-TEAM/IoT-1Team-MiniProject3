import serial
import json
import threading
import time

arduino2  = serial.Serial('/dev/ttyAMA1', 9600, timeout=1)

def AD2_Thread():
    def Get_Json():
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

arduino2_Thread = threading.Thread(target=AD2_Thread)

arduino2_Thread.start()

arduino2_Thread.join()
