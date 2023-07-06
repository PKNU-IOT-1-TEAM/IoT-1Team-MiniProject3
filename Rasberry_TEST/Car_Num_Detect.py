import cv2
import pytesseract

# 번호판 인식을 위한 Haar Cascade 파일 경로
cascade_path = 'haarcascade_russian_plate_number.xml'

# Tesseract의 실행 경로
pytesseract.pytesseract.tesseract_cmd = '/usr/bin/tesseract'

# 번호판 인식을 위한 Haar Cascade 분류기를 로드합니다.
plate_cascade = cv2.CascadeClassifier(cascade_path)

# Tesseract OCR에 한글 언어 데이터를 추가합니다.
custom_config = r'--oem 3 --psm 6 -l kor'

# 비디오 스트림에서 이미지를 읽기 위해 VideoCapture 객체를 생성합니다.
cap = cv2.VideoCapture(0)

while True:
    # 비디오 스트림에서 이미지 프레임을 읽어옵니다.
    ret, frame = cap.read()

    # 이미지를 그레이스케일로 변환합니다.
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # Haar Cascade를 사용하여 번호판을 탐지합니다.
    plates = plate_cascade.detectMultiScale(gray, scaleFactor=1.3, minNeighbors=5, minSize=(100, 100))

    # 번호판 주변에 사각형을 그리고 텍스트를 추출합니다.
    for (x, y, w, h) in plates:
        # 번호판 주위에 사각형을 그립니다.
        cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

        # 번호판 영역의 이미지를 추출합니다.
        plate_image = gray[y:y + h, x:x + w]

        # 텍스트 추출을 위해 이미지를 이진화합니다.
        _, threshold = cv2.threshold(plate_image, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

        # 추출된 텍스트를 저장합니다.
        text = pytesseract.image_to_string(threshold, config=custom_config)

        # 추출된 텍스트를 터미널에 출력합니다.
        print("번호판 텍스트:", text)

    # 화면에 이미지 프레임을 표시합니다.
    cv2.imshow('License Plate Detection', frame)

    # 'q' 키를 누르면 루프를 종료합니다.
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# 비디오 스트림과 창을 해제합니다.
cap.release()
cv2.destroyAllWindows()
