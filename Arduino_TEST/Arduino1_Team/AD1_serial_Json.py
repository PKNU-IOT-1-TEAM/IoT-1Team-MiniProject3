import serial
import json

ser = serial.Serial('/dev/ttyACM0', 9600)

while True:

    json_str = ser.readline().decode('utf-8').rstrip()

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

ser.close()