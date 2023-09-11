# 텍스트 추출을 위해 이미지를 이진화합니다.
        _, threshold = cv2.threshold(plate_image, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
