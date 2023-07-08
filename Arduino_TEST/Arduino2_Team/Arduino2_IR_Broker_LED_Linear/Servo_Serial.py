import serial
import time
import json
#port="/dev/ttyACM0"
# 시리얼 포트
port="/dev/ttyAMA1"
# 시리얼 포트와 통신속도 설정
serialFromARdunio = serial.Serial(port, 9600)

def Get_Json():
    try:
        json_str = serialFromARdunio.readline().decode()
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
        serialFromARdunio.write(b'1')
        time.sleep(1)
        Get_Json()
    elif user_input == '-1':
        serialFromARdunio.write(b'-1')
        time.sleep(1)
        Get_Json()
    else:
        print("Invalid input. Please enter '1' or '0'.")

    