import requests
import pandas as pd
import time
from urllib.parse import unquote, urlencode, quote_plus
from datetime import datetime, date, timedelta

from joblib import load

url = "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst"

serviceKey = "wGokgRxD1t3z5G4u7MsWumpoCeiWO8JM6yZ87rX1ELTO9nMSUuMOQjHj70rAzuopgyB1iLdKX0S9WK0RLs88bQ==" # 공공데이터 포털에서 생성된 본인의 서비스 키를 복사 / 붙여넣기
serviceKeyDecoded = unquote(serviceKey, 'UTF-8') # 공데이터 포털에서 제공하는 서비스키는 이미 인코딩된 상태이므로, 디코딩하여 사용해야 함

while True:
  now = datetime.now()
  today = datetime.today().strftime("%Y%m%d")
  y = date.today() - timedelta(days=1)
  yesterday = y.strftime("%Y%m%d")

  if now.minute<45: # base_time와 base_date 구하는 함수
      if now.hour==0:
          base_time = "2330"
          base_date = yesterday
      else:
          pre_hour = now.hour-1
          if pre_hour<10:
              base_time = "0" + str(pre_hour) + "30"
          else:
              base_time = str(pre_hour) + "30"
          base_date = today
  else:
      if now.hour < 10:
          base_time = "0" + str(now.hour) + "30"
      else:
          base_time = str(now.hour) + "30"
      base_date = today

  queryParams = '?' + urlencode({ quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('base_date') : base_date, quote_plus('pageNo') : 1,
                                      quote_plus('base_time') : base_time, quote_plus('nx') : 98, quote_plus('ny') : 76,
                                      quote_plus('dataType') : 'json', quote_plus('numOfRows') : '60'}) #페이지로 안나누고 한번에 받아오기 위해 numOfRows=60으로 설정해주었다


      # 값 요청 (웹 브라우저 서버에서 요청 - url주소와 파라미터)
  res = requests.get(url + queryParams, verify=False) # verify=False이거 안 넣으면 에러남ㅜㅜ
  items = res.json().get('response').get('body').get('items') #데이터들 아이템에 저장



  # 저장된 모델 로드
  stacking = load('Bigdata\model.joblib')

  data = {
      '기온(°C)': [float(items['item'][3]['obsrValue'])],
      '풍향(deg)': [float(items['item'][5]['obsrValue'])],
      '풍속(m/s)': [float(items['item'][7]['obsrValue'])],
      '강수량(mm)': [float(items['item'][2]['obsrValue'])]
  }

  new_data = pd.DataFrame(data)

  predictions = stacking.predict(new_data)

  probabilities = stacking.predict_proba(new_data)

  print(data)
  print(f"침수확률 : {probabilities[0][1]:.2%}")

  time.sleep(10)