import serial
import time
import json

port="/dev/ttyACM0"
serialFromArduino = serial.Serial(port,9600)

while True:
    user_input = input("Enter command (1 to rurn on, 0 to turn off): ")
    if user_input == '1':
        serialFromArduino.write(b'1')
    elif user_input == '0':
        serialFromArduino.write(b'0')
    else:
        serialFromArduino.write(b'2')

    json_str = serialFromArduino.readline().decode('utf-8').rstrip()

    try:
        data = json.loads(json_str)

        AD3_WGuard_Wave = data["HC_SR04_sensor"]
        json_data = {
            "HC_SR04_sensor" : AD3_WGuard_Wave
        }
        json_output = json.dumps(json_data)
        print(json_output)
    except json.JSONDecodeError:
        print("Invalid Json Data: ",json_str)


    # if serialFromArduino.readable() > 0:
        
    #     data = serialFromArduino.readline().decode('utf-8')
    #     print(data)
    

serialFromArduino.close()
