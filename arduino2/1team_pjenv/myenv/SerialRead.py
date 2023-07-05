import serial

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

    if serialFromArduino.in_waiting != 0:
        data = serialFromArduino.readline().decode()
        print(data)
    


serialFromArduino.close()