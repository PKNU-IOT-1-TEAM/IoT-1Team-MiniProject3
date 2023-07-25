import serial
import json
import time

ser = serial.Serial("/dev/ttyACM1", 9600, timeout=1)

while True:
    json_str = ser.readline().decode('utf-8').rstrip()

    try:
        data = json.loads(json_str)

        AD1_Ir = data["IR_Sensor"]
        AD1_Temp = data["Temperature"]
        AD1_Hum = data["Humidity"]
        AD1_1_0_GRI = data["PM_1.0_GRIMM"]
        AD1_2_5_GRI = data["PM_2.5_GRIMM"]
        AD1_10_GRI = data["PM_10_GRIMM"]
        AD1_1_0_TSI = data["PM_1.0_TSI"]
        AD1_2_5_TSI = data["PM_2.5_TSI"]
        AD1_10_TSI = data["PM_10_TSI"]
        AD1_0_3_um = data["Number_of_0.3_um"]
        AD1_0_5_um = data["Number_of_0.5_um"]
        AD1_1_um = data["Number_of_1_um"]
        AD1_2_5_um = data["Number_of_2.5_um"]
        AD1_5_um = data["Number_of_5_um"]
        AD1_10_um = data["Number_of_10_um"]

        json_data = {
            "IR_Sensor": AD1_Ir,
            "Temperature": AD1_Temp,
            "Humidity": AD1_Hum,
            "PM_1.0_GRIMM": AD1_1_0_GRI,
            "PM_2.5_GRIMM": AD1_2_5_GRI,
            "PM_10_GRIMM": AD1_10_GRI,
            "PM_1.0_TSI": AD1_1_0_TSI,
            "PM_2.5_TSI": AD1_2_5_TSI,
            "PM_10_TSI": AD1_10_TSI,
            "Number_of_0.3_um": AD1_0_3_um,
            "Number_of_0.5_um": AD1_0_5_um,
            "Number_of_1_um": AD1_1_um,
            "Number_of_2.5_um": AD1_2_5_um,
            "Number_of_5_um": AD1_5_um,
            "Number_of_10_um": AD1_10_um,
        }

        json_output = json.dumps(json_data)
        print(json_output)

    except json.JSONDecodeError:
        print("Invalid Json Data:", json_str)

ser.close()
