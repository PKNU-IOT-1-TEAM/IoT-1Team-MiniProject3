import cv2
import numpy as np
from keras.models import load_model

# 번호판 인식에 사용할 딥러닝 모델을 로드합니다.
model = load_model('korean_license_plate_model.h5')

# 비디오 스트림에서 이미지를 읽기 위해 VideoCapture 객체를 생성합니다.
cap = cv2.VideoCapture(0)

while True:
    # 비디오 스트림에서 이미지 프레임을 읽어옵니다.
    ret, frame = cap.read()

    # 이미지를 사전 처리합니다. (크기 조정, 정규화 등)
    processed_frame = preprocess_image(frame)

    # 모델을 사용하여 번호판을 예측합니다.
    prediction = model.predict(np.expand_dims(processed_frame, axis=0))

    # 예측 결과를 텍스트로 변환합니다.
    plate_text = decode_prediction(prediction)

    # 이미지에 번호판 텍스트를 그립니다.
    frame = draw_plate_text(frame, plate_text)

    # 화면에 이미지 프레임을 표시합니다.
    cv2.imshow('License Plate Detection', frame)

    # 'q' 키를 누르면 루프를 종료합니다.
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# 비디오 스트림과 창을 해제합니다.
cap.release()
cv2.destroyAllWindows()