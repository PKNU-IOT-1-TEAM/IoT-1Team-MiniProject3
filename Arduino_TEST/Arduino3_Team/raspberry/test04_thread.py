import threading
import serial

# 시리얼 통신 관련 설정
serial_port = "/dev/ttyACM0"    # 시리얼 포트
ser = serial.Serial(serial_port, 9600) # 시리얼 객체 생성

# 시리얼 값을 읽어오는 함수
def read_serial():  # 초음파 센서 거리 읽음
    global ser
    while True:
        if ser.readable():
            data = ser.readline().decode().rstrip()
            print("Received data:", data)

# 사용자 입력을 처리하는 함수
def handle_input():
    global ser
    while True:
        user_input = input("Enter a command: ")
        if user_input == "1":
            # 1 입력 시 아두이노에 데이터 전송 (전진)
            ser.write(b'1')
        elif user_input == "0":
            # 0 입력시 아두이노에 데이터 전송(후진)
            ser.write(b'0')
        else:
            print("Invalid command")

# 쓰레드 생성 및 실행
serial_thread = threading.Thread(target=read_serial)
serial_thread.start()   # 센서 읽는 쓰레드 시작


# 메인 쓰레드에서 사용자 입력 처리
handle_input()

# 쓰레드 종료 대기
serial_thread.join()
