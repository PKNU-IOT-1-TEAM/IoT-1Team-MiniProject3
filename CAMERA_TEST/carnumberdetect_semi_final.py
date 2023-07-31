import cv2                                                  # OpenCV : 이미지 처리와 컴퓨터 비전에 사용되는 라이브러리
import numpy as np                                          # 파이썬의 과학적 컴퓨팅을 위한 핵심 라이브러리, 다차원 배열과 벡터/행렬 연산에 대한 높은 수준의 지원 제공
import matplotlib.pyplot as plt                             # 그래프와 플롯을 그리는데 사용되는 라이브러리, 이미지를 시각화 하거나 데이터를 그래프로 표현하는 등 다양한 시각화 작업에 사용됨
import pytesseract                                          # Tesseract OCR(Optical Character Recongnition)의 파이썬 래퍼, OCR을 사용하여 이미지에서 텍스트 추출

# 이미지 불러오기
plt.style.use('dark_background')                            # matplotlib 스타일 설정 : 배경을 어둡게 설정하여 이미지를 더 잘 시각화할 수 있도록 함
# 번호판 말고 다른 문자가 사진에 나오면 인식률 떨어짐
img_ori = cv2.imread("license_plate_img.jpg")                              # img_path에 저장된 이미지를 OpenCV로 불러와서 img_ori변수에 저장

height, width, channel = img_ori.shape                      # 불러온 이미지의 높이, 너비, 채널 수를 각 변수에 저장 

# 전처리
gray = cv2.cvtColor(img_ori, cv2.COLOR_BGR2GRAY)            # img_ori 이미지를 흑백 이미지로 변환하여 gray변수에 저장

img_blurred = cv2.GaussianBlur(gray, ksize=(5, 5), sigmaX=0)    # 흑백 이미지를 가우시안 블러로 블러링하여 img_blurred 변수에 저장

# 가우시안 블러링된 이미지를 이진화
img_blur_thresh = cv2.adaptiveThreshold(                    # 가우시안 블러링된 이미지를 이진화 하여(가우시안 임계처리 방법) img_blur_thresh 변수에 저장
    img_blurred,                                            # 가우시안 블러가 적용된 이미지, 이 이미지에 대해서 임계처리 수행
    maxValue=255.0,                                         # maxValue : 임계값보다 큰 픽셀의 값을 저장하는 파라미터,255.0으로 설정하여 흰색으로 표현함(일반적으로 이진화 작업에서 흰색으로 표시하고자 하는 최댓값을 설정함)
    adaptiveMethod=cv2.ADAPTIVE_THRESH_GAUSSIAN_C,          # 적응형 임계처리를 적용하는 방법을 지정하는 파라미터 , cv2.ADAPTIVE_THRESH_GAUSSIAN_C : 각 픽셀 주변의 영역을 고려하여 가우시안 가중치를 적용하여 임계값을 결정하는 방법
    thresholdType=cv2.THRESH_BINARY_INV,                    # 임계처리 방법 "cv2.THRESH_BINARY_INV" : 임계값보다 큰 값은 0으로, 작은 값은 maxValue로 설정하여 반전된 이진화 이미지를 생성
    blockSize=19,                                           # 임계값을 결정할 때 사용할 블록 크기를 지정하는 파라미터, 홀수로 설정해야 함, 블록 크기가 클수록 전역적인 밝기 변화에 덜 민감하며, 작을수록 민감
    C=9                                                     # 계산된 임계값에 더해주거나 뺴주는 상수 값을 지정하는 파라미터, 보정 상수로서 필요함
)
# gray 이미지를 이진화(가우시안 임계처리 방법)
img_thresh = cv2.adaptiveThreshold(
    gray,
    maxValue=255.0,
    adaptiveMethod=cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
    thresholdType=cv2.THRESH_BINARY_INV,
    blockSize=19,
    C=9
)

# img_blur_thresh 이미지에서 외곽선을 찾아서 contours 리스트에 저장하고, 반환값은 무시, _ 변수에는 윤곽선들의 계층정보가 저장된다
contours, _ = cv2.findContours(                             # cv2.findContours : 이진화된 이미지에서 윤곽선을 검출하는 함수, 반환값은 검출된 윤곽선들과 계층정보로 이루어진 리스트
    img_blur_thresh,                                        # 가우시안 블러링 된 이미지를 이진화 한 이미지(검출하고자 하는 객체 또는 영역을 흰색으로 표현하고 배경을 검정색으로 표현한 이미지이여야 함)
    mode=cv2.RETR_LIST,                                     # 윤곽선 검출 방법을 지정하는 파라미터, cv2.RETR_LIST : 모든 검출된 윤곽선을 리스트로 반환하는 방식을 의미
    method=cv2.CHAIN_APPROX_TC89_KCOS                       # 윤곽선 근사 방법을 지정하는 파라미터
    # CHAIN_APPROX_SIMPLE :최소한의 점으로 표현 / CHAIN_APPROX_NONE : 모든 점들이 반환됨 / CHAIN_APPROX_TC89_L1 : Teh-Chin연결 근사 알고리즘 / CHAIN_APPROX_TC89_KCOS : kcos 거리는 윤곽선의 길이와 곡률을 모두 계산(좀 더 정확)
)

# 이미지에 외곽선을 그려서 시각화 한 결과를 저장(외곽선 잘 그려지는지 보기위함, 꼭 필요한 부분은 아님)
temp_result = np.zeros((height, width, channel), dtype=np.uint8)                        # 배열 생성(temp_result)
cv2.drawContours(temp_result, contours=contours, contourIdx=-1, color=(255,255,255))    # cv2.drawContours() 함수를 사용하여 윤곽선을 'temp_result'에 그리기


temp_result = np.zeros((height, width, channel), dtype=np.uint8)                        # 배열 생성
contours_dict = []                                                                      # 윤곽선 정보를 저장할 빈 리스트


for contour in contours:                                                                        # contours 리스트에 저장된 윤곽선 하나씩 불러오기
    x, y, w, h = cv2.boundingRect(contour)                                                      # 해당 윤곽선을 둘러싼 최소 크기의 사각형 구하기
    cv2.rectangle(temp_result, pt1=(x,y), pt2=(x+w, y+h), color=(255,255,255), thickness=2)     # temp_result 이미지에 해당 윤곽선을 둘러싼 사각형 그리기
    # pt1=(x,y) : 사각형 왼쪽 상단 모서리, pt2=(x+w, y+h) : 사각형 오른쪽 하단 모서리
    
    # 윤곽선에 대한 정보를 contours_dict 리스트에 딕셔너리 형태로 저장(사각형의 좌표와 크기, 중심좌표(cx,cy))
    contours_dict.append({
        'contour': contour,
        'x': x,
        'y': y,
        'w': w,
        'h': h,
        'cx': x + (w / 2),
        'cy': y + (h / 2)
    })
    
MIN_AREA = 80                                                               # 윤곽선이 감싸는 사각형의 넓이가 이 값보다 큰 경우에만 고려
MIN_WIDTH, MIN_HEIGHT=2, 8                                                  # 윤곽선이 감싸는 사각형의 너비와 높이가 이 값보다 큰 경우에만 고려
MIN_RATIO, MAX_RATIO = 0.25, 1.0                                            # 윤곽선이 감싸는 사각형의 너비와 높이의 비율이 이 범위에 해당하는 경우에만 고려

possible_contours = []                                                      # 선별된 윤곽선들을 저장할 빈 리스트를 생성합니다.

cnt = 0                                                                     # possible_contours 리스트에 추가될 윤곽선의 개수를 카운트하는 변수를 초기화합니다.
for d in contours_dict:                                                     # contours_dict 리스트에 저장된 윤곽선에 대해 반복
    area = d['w'] * d['h']                                                  # 현재 윤곽선이 감싸는 사각형의 넓이를 계산
    ratio = d['w'] / d['h']                                                 # 현재 윤곽선이 감싸는 사각형의 너비와 높이의 대해 계산

    # 현재 윤곽선이 조건을 만족하는 경우(넓이, 너비, 높이, 비율 등이 모두 임계값 범위 내에 있다면)    
    if area > MIN_AREA \
    and d['w'] > MIN_WIDTH and d['h'] > MIN_HEIGHT \
    and MIN_RATIO < ratio < MAX_RATIO:
        d['idx'] = cnt                                                      # 윤곽선의 인덱스를 추가하여 나중에 해당 윤곽선이 possible_contours리스트에 추가되었음을 표시함
        cnt += 1                                                            # 카운트 변수를 증가시킴
        possible_contours.append(d)                                         # 현재 윤곽선을 possible_contours리스트에 추가

# 임계값을 만족하는 윤곽선을 그리는데 사용할 임시 결과 이미지를 생성함(빈 이미지)
temp_result = np.zeros((height, width, channel), dtype = np.uint8)

# 윤곽선을 감싸는 사각형의 좌표를 이용하여 사각형을 temp_result 이미지에 그림, pt1은 사각형 왼쪽 상단, pt2는 사각형의 오른쪽 하단 꼭지점
for d in possible_contours:
    cv2.rectangle(temp_result, pt1=(d['x'], d['y']), pt2=(d['x']+d['w'], d['y']+d['h']), color=(255, 255, 255), thickness=2)


MAX_DIAG_MULTIPLYER = 5                                                     # 윤곽선 간의 거리 비율의 상한값, 최대 대각선 길이의 몇 배까지 윤곽선을 매칭시킬지 지정
MAX_ANGLE_DIFF = 12.0                                                       # 윤곽선의 각도 차이의 상한값, 최대 각도 차이가 이 값보다 작을 때 윤곽선을 매칭시킴
MAX_AREA_DIFF = 0.5                                                         # 윤곽선의 넓이 차이의 상한값, 최대 넓이 차이가 이 값보다 작을 때 윤곽선을 매칭시킴
MAX_WIDTH_DIFF = 0.8                                                        # 윤곽선의 너비 차이의 상한값, 최대 너비 차이가 이 값보다 작을 때 윤곽선을 매칭시킴
MAX_HEIGHT_DIFF = 0.2                                                       # 윤곽선의 높이 차이의 상한값, 최대 높이 차이가 이 값보다 작을 때 윤곽선을 매칭시킴
MIN_N_MATCHED = 3                                                           # 윤곽선을 매칭하기 위해 필요한 최소 매칭 개수, 이보다 작은 경우 해당 윤곽선은 매칭되지 않음

def find_chars(contour_list):
    matched_result_idx = []                                                 # 매칭된 윤곽선들의 인덱스를 저장할 빈 리스트를 생성
    
    for d1 in contour_list:                                                 # contour_list 리스트에 저장된 윤곽선들에 대해 반복
        matched_contours_idx = []                                           # 현재 윤곽선과 매칭되는 윤곽선들의 인덱스를 저장할 빈 리스트를 생성
        for d2 in contour_list:                                             # contour_list 리스트에 저장된 윤곽선들에 대해 반복
            if d1['idx'] == d2['idx']:                                      # 같은 윤곽선일 때 무시하고 다음 윤곽선으로 넘어감
                continue
                
            dx = abs(d1['cx'] - d2['cx'])                                   # 현재 윤곽선과 다른 윤곽선의 중심 좌표의 x축 차이를 계산
            dy = abs(d1['cy'] - d2['cy'])                                   # 현재 윤곽선과 다른 윤곽선의 중심 좌표의 y축 차이를 계산
            
            diagonal_length1 = np.sqrt(d1['w'] ** 2 + d1['h'] ** 2)         # 현재 윤곽선이 감싸는 사각형의 대각선 길이 계산
            
            distance = np.linalg.norm(np.array([d1['cx'], d1['cy']]) - np.array([d2['cx'], d2['cy']]))      # 두 윤곽선의 중심 좌표 간의 거리를 계산
            if dx == 0:                                                     # 두 윤곽선의 중심 좌표가 x축 방향으로 같은 위치에 있을 때를 의미(두 윤곽선이 수직 방향으로 정렬되어 있을 때)
                angle_diff = 90                                             # 두 윤곽선이 수직 방향으로 정렬되어 있는 경우 각도 차이를 90도로 설정함(수직하게 맞닿아 있을 땐 각도차이가 90도이기 때문)
            else:                                                           # 두 윤곽선이 수평 방향으로 정렬되지 않았을때
                # 라디안 -> 각도로 변환하여 angle_diff 변수에 저장
                angle_diff = np.degrees(np.arctan(dy / dx))                 # np.arctan(dy / dx) : 'dy / dx'의 아크탄젠트 값을 계산(두 점 사이의 기울기에 해당하는 각도) / np.degrees(np.arctan(dy / dx)) : 두 윤곽선 사이의 각도 차이를 계산
                
            area_diff = abs(d1['w'] * d1['h'] - d2['w'] * d2['h']) / (d1['w'] * d1['h'])        # 두 윤곽선이 감싸는 사각형의 넓이 차이를 계산, 크기가 얼마나 다른지 비율로 나타냄
            width_diff = abs(d1['w'] - d2['w']) / d1['w']                                       # 두 윤곽선의 너비 차이를 계산, 너비가 얼마나 다른지 비율로 나타냄
            height_diff = abs(d1['h'] - d2['h']) / d1['h']                                      # 두 윤곽선의 높이 차이를 계산, 높이가 얼마나 다른지 비율로 나타냄
            
            # 두 윤곽선 간의 거리, 각도 차이, 넓이 차이, 너비 차이, 높이 차이가 모두 임계값 범위에 해당하는 경우
            if distance < diagonal_length1 * MAX_DIAG_MULTIPLYER \
            and angle_diff < MAX_ANGLE_DIFF and area_diff < MAX_AREA_DIFF \
            and width_diff < MAX_WIDTH_DIFF and height_diff < MAX_HEIGHT_DIFF:
                matched_contours_idx.append(d2['idx'])                                          # 현재 윤곽선과 매칭되는 윤곽선의 인덱스를 리스트에 추가함

        # 현재 윤곽선 자체도 매칭되었다고 가정하고 자기 자신의 인덱스를 추가함
        matched_contours_idx.append(d1['idx'])          
        
        # 매칭된 윤곽선들의 인덱스 개수가 최소 매칭 개수보다 작으면 해당 윤곽선은 매칭되지 않은 것으로 간주하고 다음 윤곽선으로 넘어감
        if len(matched_contours_idx) < MIN_N_MATCHED:
            continue
            
        # 매칭된 윤곽선들의 인덱스를 리스트에 추가
        matched_result_idx.append(matched_contours_idx)         
        
        unmatched_contour_idx = []                                              # 매칭되지 않은 윤곽선들의 인덱스를 저장할 빈 리스트를 생성
        for d4 in contour_list:                                                 # 매칭되지 않은 윤곽선들의 인덱스를 찾아서 리스트에 추가
            if d4['idx'] not in matched_contours_idx:
                unmatched_contour_idx.append(d4['idx'])
        
        # 매칭되지 않은 윤곽선들을 다시 'possible_contours'리스트에서 찾아서 재귀적으로 매칭
        unmatched_contour = np.take(possible_contours, unmatched_contour_idx)
        recursive_contour_list = find_chars(unmatched_contour)
        
        # 재귀적으로 찾은 매칭된 윤곽선들의 인덱스를 리스트에 추가함
        for idx in recursive_contour_list:
            matched_result_idx.append(idx)
            
        break       # 모든 매칭을 찾았으면 반복문 종료
        
    return matched_result_idx       # 매칭된 윤곽선들의 인덱스로 구성된 리스트를 반환

# find_chars 함수를 호출하여 매칭된 윤곽선들의 인덱스를 얻음
result_idx = find_chars(possible_contours)

matched_result = []     # 가능한 윤곽선들을 매칭한 결과를 저장할 빈 리스트
for idx_list in result_idx:                                         # 매칭된 윤곽선들의 인덱스들을 리스트가 'idx_list'변수에 순차적으로 대입되며 반복
    matched_result.append(np.take(possible_contours, idx_list))     # 매칭된 윤곽선의 인덱스를 이용하여 'possible_contours'에서 해당하는 윤곽선들을 추출하여 'matched_result'리스트에 추가함
    
temp_result = np.zeros((height, width, channel), dtype=np.uint8)    # 매칭된 결과를 시각화하기 위해 임시로 사용할 템플릿 이미지

for r in matched_result:                                            # 'matched_result'에 있는 매칭된 윤곽선들을 하나씩 대입하며 반복
    for d in r:                                                     # 매칭된 윤곽선들의 하나씩 대입되며 반복
        cv2.rectangle(temp_result, pt1=(d['x'], d['y']), pt2=(d['x']+d['w'], d['y']+d['h']), color=(255,255,255), thickness=2)      # 매칭된 윤곽선의 위치를 'temp_result'이미지에 흰색 사각형으로 표시

PLATE_WIDTH_PADDING = 1.3   # 번호판의 폭 추출할 때 사용(추출된 번호판 영역의 가로 폭을 'PLATE_WIDTH_PADDING'로 곱하여 최종 번호판의 가로 폭을 계산) => 번호판의 가로폭을 확장시킴(숫자가 커질수록)
PLATE_HEIGHT_PADDING = 1.5  # 번호판의 높이 추출할 때 사용(추출된 번호판 영역의 세로 폭을 'PLATE_HEIGHT_PADDING'로 곱하여 최종 번호판의 세로 폭을 계산) => 번호판의 세로 폭을 확장시킴(숫자가 커질수록)
MIN_PLATE_RATIO = 3         # 최소 번호판 비율
MAX_PLATE_RATIO = 10        # 최대 번호판 비율

plate_imgs = []             # 잘라낸 번호판의 이미지들을 담을 리스트
plate_infos = []            # 번호판 영역의 위치와 크기 정보를 담을 리스트

for i, matched_chars in enumerate(matched_result):      # 'matched_result' 매칭된 번호판 영역들의 리스트가 담겨있음
    sorted_chars = sorted(matched_chars, key=lambda x: x['cx'])  # 'matched_chars' : 번호판으로 추정되는 윤곽선들의 리스트 / 'cx'(중심의 좌표) 기준으로 정렬하여 번호판의 좌우 순서를 찾음

    plate_cx = (sorted_chars[0]['cx'] + sorted_chars[-1]['cx']) / 2     # 번호판의 중심 좌표 찾음
    plate_cy = (sorted_chars[0]['cy'] + sorted_chars[-1]['cy']) / 2     # 번호판의 중심 좌표 찾음
    
    plate_width = (sorted_chars[-1]['x'] + sorted_chars[-1]['w'] - sorted_chars[0]['x']) * PLATE_WIDTH_PADDING  # 번호판의 가로폭 계산후 'PLATE_WIDTH_PADDING' 이용해서 폭 조정
    
    sum_height = 0      
    for d in sorted_chars:      # 윤곽선의 평균 높이 계산
        sum_height += d['h']

    plate_height = int(sum_height / len(sorted_chars) * PLATE_HEIGHT_PADDING)                                   # 번호판의 세로폭 계산후 'PLATE_HEIGHT_PADDING' 이용해서 폭 조정
    
    triangle_height = sorted_chars[-1]['cy'] - sorted_chars[0]['cy']                        # 번호판의 기울기를 보정하기 위해 삼각형 높이 계산
    triangle_hypotenus = np.linalg.norm(                                                    # 번호판의 기울기를 보정하기 위해 삼각형 대각선 계산
        np.array([sorted_chars[0]['cx'], sorted_chars[0]['cy']]) - 
        np.array([sorted_chars[-1]['cx'], sorted_chars[-1]['cy']])
    )
    angle = np.degrees(np.arcsin(triangle_height / triangle_hypotenus))                     # 번호판의 기울기를 보정하기 위해 삼각형 각도 계산
    
    rotation_matrix = cv2.getRotationMatrix2D(center=(plate_cx, plate_cy), angle=angle, scale=1.0)      # 번호판을 보정하기 위한 회전 변환 행렬 생성
    
    img_rotated = cv2.warpAffine(img_thresh, M=rotation_matrix, dsize=(width, height))                  # 회전 변환 행렬을 이용하여 번호판 이미지를 보정하여 회전시킴
    
    img_cropped = cv2.getRectSubPix(                # 보정된 이미지에서 최종 번호판 영역 잘라냄
        img_rotated, 
        patchSize=(int(plate_width), int(plate_height)),    # 번호판 크기
        center=(int(plate_cx), int(plate_cy))
    )
    
    # 최종 잘라낸 번호판의 가로와 세로 비율이 'MIN_PLATE_RATIO'보다 작거나 'MAX_PLATE_RATIO'보다 크면 번호판으로 추정하지 않고 넘어감
    if img_cropped.shape[1] / img_cropped.shape[0] < MIN_PLATE_RATIO or img_cropped.shape[1] / img_cropped.shape[0] < MIN_PLATE_RATIO > MAX_PLATE_RATIO:
        continue
    
    plate_imgs.append(img_cropped)              # 추출한 이미지를 'plate_imgs'에 추가
    plate_infos.append({                        # 번호판의 위치와 크기 정보를 'plate_infos'에 추가함
        'x': int(plate_cx - plate_width / 2),
        'y': int(plate_cy - plate_height / 2),
        'w': int(plate_width),
        'h': int(plate_height)
    })

longest_idx, longest_text = -1, 0                   # 가장 긴 번호판의 인덱스와 문자 길이를 저장할 변수를 초기화 함
plate_chars = []                                    # 번호판 문자들을 저장할 리스트를 초기화 함

for i, plate_img in enumerate(plate_imgs):          # 'plate_imgs' 에 저장된 번호판 이미지들을 하나씩 순회
    plate_img = cv2.resize(plate_img, dsize=(0, 0), fx=1.6, fy=1.6)         #3  이미지크기를 1.6배 확대(OCR이 높은 해상도의 이미지에서 더 잘 작동하는 경우가 있기 때문)
    _, plate_img = cv2.threshold(plate_img, thresh=0.0, maxval=255.0, type=cv2.THRESH_BINARY | cv2.THRESH_OTSU) # 이미지 이진화 수행
    
    # 이미지에서 윤곽선 찾음 / cv2.RETR_LIST : 모든 윤곽선 찾음 / cv2.CHAIN_APPROX_TC89_KCOS : 윤곽선 압축하여 저장 
    contours, _ = cv2.findContours(plate_img, mode=cv2.RETR_LIST, method=cv2.CHAIN_APPROX_TC89_L1)
    
    # 추출된 윤곽선들 중에서 최대/최소 x,y좌표 초기화
    plate_min_x, plate_min_y = plate_img.shape[1], plate_img.shape[0]
    plate_max_x, plate_max_y = 0, 0

    # 찾은 윤곽선 하나씩 순회
    for contour in contours:
        x, y, w, h = cv2.boundingRect(contour)  # 윤곽선의 경계 사각형 구함
        
        area = w * h                            # 윤곽선 넓이
        ratio = w / h                           # 윤곽선 가로 세로 비율

        # 윤곽선의 넓이, 가로, 세로, 비율이 조건을 만족하면 번호판 영역으로 판단, 해당 영역 좌표 갱신
        if area > MIN_AREA \
        and w > MIN_WIDTH and h > MIN_HEIGHT \
        and MIN_RATIO < ratio < MAX_RATIO:
            if x < plate_min_x:
                plate_min_x = x
            if y < plate_min_y:
                plate_min_y = y
            if x + w > plate_max_x:
                plate_max_x = x + w
            if y + h > plate_max_y:
                plate_max_y = y + h
    # 최종 번호판 영역 추출                
    img_result = plate_img[plate_min_y:plate_max_y, plate_min_x:plate_max_x]
    
    # 추출된 번호판 영역에 가우시안 블러 적용하여 노이즈 줄이기 
    img_result = cv2.GaussianBlur(img_result, ksize=(3, 3), sigmaX=0)
    # 이미지 이진화
    _, img_result = cv2.threshold(img_result, thresh=0.0, maxval=255.0, type=cv2.THRESH_BINARY | cv2.THRESH_OTSU)
    # 이미지 주변을 검은색으로 경계선 만들기(OCR이 잘 동작하도록 도와줌)
    img_result = cv2.copyMakeBorder(img_result, top=10, bottom=10, left=10, right=10, borderType=cv2.BORDER_CONSTANT, value=(0,0,0))
    
    # 'pytesseract'라이브러리 사용하여 번호판 이미지에서 문자 인식
    chars = pytesseract.image_to_string(img_result, lang='kor', config= '--psm 13') 
    
    result_chars = ''
    has_digit = False

    # 인식된 문자 중에서 한글과 숫자만 추출하여 'plate_chars'리스트에 저장
    for c in chars:
        if ord('가') <= ord(c) <= ord('힣') or c.isdigit():
            if c.isdigit():
                has_digit = True
            result_chars += c
    plate_chars.append(result_chars)

    # 인식된 문자열에 숫자가 포함되어 있고, 해당 문자열의 길이가 이전까지의 최대 길이보다 크다면, 이를 최종 결과로 선택
    if has_digit and len(result_chars) > longest_text:
        longest_idx = i

info = plate_infos[longest_idx]     # 최종 선택된 인덱스를 이용해서 'plate_infos'에서 번호판의 위치와 크기정보 가져옴
chars = plate_chars[longest_idx]    # 최종 선택된 인덱스를 이용해서 'plate_chars'에서 인식된 번호판 문자열 가져옴

# 번호판 결과 저장하는 변수
print(chars)

# 인식된 번호판 영역 시각화 해주는 부분(필요없으니 지워도 됨)
# 원본이미지(img_out) 복사하여 새로운 이미지 변수 생성(원본이미지 영향 x)
img_out = img_ori.copy()
# 사각형 그리기
cv2.rectangle(img_out, pt1=(info['x'], info['y']), pt2=(info['x']+info['w'], info['y']+info['h']), color=(255,0,0), thickness=2)
# 그려진 이미지를 'jpg'파일로 저장('char'.jpg)
cv2.imwrite(chars + '.jpg', img_out)
