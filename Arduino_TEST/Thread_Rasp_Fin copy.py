import serial
import json
import threading
import time
arduino1  = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
# arduino2  = serial.Serial('/dev/ttyAMA1', 9600, timeout=1)
# arduino3  = serial.Serial('/dev/ttyAMA2', 9600, timeout=1)
# arduino4  = serial.Serial('/dev/ttyAMA3', 9600, timeout=1)

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
            print("Invalid Json Data_1: ", json_str )
    
# def AD2_Thread():
#     def Get_Json():
#         try:
#             json_str = arduino2.readline().decode()
#             data = json.loads(json_str)

#             AD2_RCV_CGuard = data["AD2_RCV_CGuard"]
#             json_data = {
#                 "AD2_RCV_CGuard" : AD2_RCV_CGuard
#             }
#             json_output = json.dumps(json_data)
#             print(json_output)  
#         except json.JSONDecodeError:
#             print("Invaild Json Data_2: ", json_str)

#     while True:
#         user_input = input("Enter '1' or '-1' : ")
#         if user_input == '1':
#             arduino2.write(b'1')
#             time.sleep(1)
#             Get_Json()
#         elif user_input == '-1':
#             arduino2.write(b'-1')
#             time.sleep(1)
#             Get_Json()
#         else:
#             print("Invalid input. Please enter '1' or '0'.")

# def AD3_Thread():
#     def read_serial():
#         while True:
#             json_str = arduino3.readline().decode('utf-8').rstrip()

#             try:
#                 data = json.loads(json_str)
#                 # AD3 데이터 처리
#                 AD3_RCV_WGuard_Wave = data["AD3_RCV_WGuard_Wave"]
#                 json_data = {
#                     "AD3_RCV_WGuard_Wave": AD3_RCV_WGuard_Wave
#                 }
#                 json_output = json.dumps(json_data)
#                 print(json_output)
#             except json.JSONDecodeError:
#                 print("Invalid Json Data_3:", json_str)

#     def hand_input():
#         while True:
#             user_input = input("Enter a command: ")  # 사용자 입력 처리
#             if user_input == "1":
#                 arduino3.write(b'1')
#             elif user_input == "0":
#                 arduino3.write(b'0')
#             else:
#                 print("Invalid command")
    
#     # 중첩 스레드라서 지우면 안됌..!!!!
#     serial_thread = threading.Thread(target=read_serial)
#     serial_thread.start()
#     hand_input()
#     serial_thread.join()

# def AD4_Thread():
#     while True :
#         json_str = arduino4.readline().decode('utf-8').rstrip()

#         try:
#             data = json.loads(json_str)
#             AD4_NFC = data["NFC"]
#             AD4_WL_CNNT = data["WL_CNNT"]
#             AD4_WL_NCNNT = data["WL_NCNNT"]

#             json_data = {
#                 "NFC" : AD4_NFC,
#                 "WL_CNNT" : AD4_WL_CNNT,
#                 "WL_NCNNT" : AD4_WL_NCNNT
#             }

#             json_output = json.dumps(json_data)
#             print(json_output)
#         except json.JSONDecodeError:
#             print("Invalid Json Data_4: ", json_str)

arduino1_Thread = threading.Thread(target=AD1_Thread)
# arduino2_Thread = threading.Thread(target=AD2_Thread)
# arduino3_Thread = threading.Thread(target=AD3_Thread)
# arduino4_Thread = threading.Thread(target=AD4_Thread)

arduino1_Thread.start()
# arduino2_Thread.start()
# arduino3_Thread.start()
# arduino4_Thread.start()

arduino1_Thread.join()
# arduino2_Thread.join()
# arduino3_Thread.join()
# arduino4_Thread.join()
