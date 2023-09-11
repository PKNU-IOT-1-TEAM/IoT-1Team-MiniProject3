import cv2
import pytesseract
import time
from picamera2 import Picamera2

# Tesseract OCR 설정
pytesseract.pytesseract.tesseract_cmd = '/usr/bin/tesseract'
tessdata_dir_config = '--tessdata-dir "/usr/share/tesseract-ocr/4.00/tessdata" -l kor'

# 카메라 초기화
cam = Picamera2()
cam.preview_configuration.main.format = 'RGB888'
cam.preview_configuration.align()

cam.configure('preview')
cam.start()

while True:
    frame = cam.capture_array()

    # 이미지 전처리 (예: 그레이스케일 변환, 가우시안 블러)
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    blur = cv2.GaussianBlur(gray, ksize = (5, 5), sigmaX = 0)
    blur_thresh = cv2.adaptiveThreshold(
        blur,
        maxValue = 255.0,
        adaptiveMethod = cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
        thresholdType = cv2.THRESH_BINARY_INV,
        blockSize = 19,
        C = 9
    )
    thresh = cv2.adaptiveThreshold(
        gray,
        maxValue = 255.0,
        adaptiveMethod= cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
        thresholdType = cv2.THRESH_BINARY_INV,
        blockSize = 19,
        C = 9
    )

    contours, _ = cv2.findContours(
        blur_thresh,
        mode = cv2.RETR_LIST,
        method = cv2.CHAIN_APPROX_SIMPLE
    )

    # _, binary = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
    # # 가우시안 블러
    # blur = cv2.GaussianBlur(binary, (5, 5), 0)


    # # 번호판 영역 검출 (예: 캐니 에지 검출, 컨투어 검출)
    edges = cv2.Canny(thresh, 50, 150)
    contours, _ = cv2.findContours(edges.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    contours = sorted(contours, key=cv2.contourArea, reverse=True)[:10]
    plate = None

    for contour in contours:
        perimeter = cv2.arcLength(contour, True)
        approx = cv2.approxPolyDP(contour, 0.02 * perimeter, True)

        if len(approx) == 4:  # 사각형 모양의 컨투어 검출
            x, y, w, h = cv2.boundingRect(approx)
            aspect_ratio = w / float(h)

            if 1.5 <= aspect_ratio <= 3.5:  # 번호판 가로 세로 비율 조건
                plate = gray[y:y + h, x:x + w]
                break

    if plate is not None:
        # Tesseract OCR을 사용하여 번호판 인식
        result = pytesseract.image_to_string(plate, config=tessdata_dir_config)
        print("번호판:", result.strip())

    cv2.imshow('piCam', frame)
    if cv2.waitKey(1) == ord('q'):
        break

# 정리
cam.release()
cv2.destroyAllWindows()
