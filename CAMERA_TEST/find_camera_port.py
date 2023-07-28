import cv2

camera = cv2.VideoCapture(3)

ret, frame = camera.read()

camera.release()