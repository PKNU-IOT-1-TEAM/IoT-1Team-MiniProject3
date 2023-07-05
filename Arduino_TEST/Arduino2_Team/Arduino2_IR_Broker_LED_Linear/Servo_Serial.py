import serial
import time
#port="/dev/ttyACM0"
# 시리얼 포트
port="/dev/ttyACM0"
# 시리얼 포트와 통신속도 설정
serialFromARdunio = serial.Serial(port, 9600)

while True:
    user_input = input("Enter '1' or '0' : ")
    if user_input == '1':
        serialFromARdunio.write(b'1')
        time.sleep(1)
    elif user_input == '0':
        serialFromARdunio.write(b'0')
        time.sleep(1)
    else:
        print("Invalid input. Please enter '1' or '0'.")

    input_s = serialFromARdunio.readline().decode()
    print(input_s)
