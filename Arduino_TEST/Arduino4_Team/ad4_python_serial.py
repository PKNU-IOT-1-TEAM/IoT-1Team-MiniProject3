import serial
import json

port = "/dev/ttyACM0"
ser = serial.Serial(port, 9600)    

while True :
    json_str = ser.readline().decode('utf-8').rstrip()

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

ser.close()
