import cv2

def find_camera_port(camera_id):
    camera = cv2.VideoCapture(camera_id)
    if camera.isOpened():
        print(f"카메라 {chr(ord('A') + camera_id)}의 인덱스는 {camera_id}")
        camera.release()
    else:
        print(f"카메라 {chr(ord('A') + camera_id)}의 인덱스를 찾을수 없습니다")

find_camera_port(0)
find_camera_port(1)
find_camera_port(2)
find_camera_port(3)