import requests
import pandas as pd
import time
from urllib.parse import unquote, urlencode, quote_plus
from datetime import datetime, date, timedelta
from datetime import timedelta as td
import pymysql
from datetime import datetime as dt
import json

from joblib import load

url = "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst" # 초단기 실황 (ML용)

url_predict= "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtFcst" # 초단기 예보 (저장용)

url_water = 'http://apis.data.go.kr/6260000/BusanRvrwtLevelInfoService/getRvrwtLevelInfo'  # 실시간 하천 수위 (저장용)

# 초단기 예보 강물 데이터 API를 받아오는 소스 작성 json -> 원하는 값만 뽑아서 data랑  강수예측 % 랑 같이 DB에 업데이트 하는 코드 
 
serviceKey= "wGokgRxD1t3z5G4u7MsWumpoCeiWO8JM6yZ87rX1ELTO9nMSUuMOQjHj70rAzuopgyB1iLdKX0S9WK0RLs88bQ==" # 공공데이터 포털에서 생성된 본인의 서비스 키를 복사 / 붙여넣기

serviceKey_water = "4n4Miwzm5p37SLb9Jk9bJa/MhFYSTl8mkQIensYxsOuwWyjpePzkk6oyRp3pOsd8GVnzwwQelKHMwSc0bPVfSA=="  # 공공 데이터 포털 하천 수위 가져 올려고 

serviceKeyDecoded = unquote(serviceKey, 'UTF-8') # 공공데이터 포털에서 제공하는 서비스키는 이미 인코딩된 상태이므로, 디코딩하여 사용해야 함 -> 초단기 실황(예보도 동일)

serviceKeyDecoded_water = unquote(serviceKey_water, 'UTF-8') # 공공데이터 포털에서 제공하는 서비스키는 이미 인코딩된 상태이므로, 디코딩하여 사용해야 함 -> 하천 수위 

conn = pymysql.connect(host='localhost', user='root', password='12345', db='team1_iot', charset='utf8')

# DB 예측값 삽입 (주현 FcstDB2 참고)

def insertDB(self, baseDate, baseTime): 
    # DB connection
    conn = pymysql.connect(host='localhost', user = 'root', password='12345', db = 'miniproject', charset='utf8')
    cur = conn.cursor()    # Connection으로부터 Cursor 생성
  
    # DB 초기화
    query = '''TRUNCATE weather'''
    cur.execute(query)
        
    # API 데이터
    allFcstData = self.newData(baseDate, baseTime)
    # DB에 API 데이터 Insert
    for fcstData in allFcstData:
        self.insertQuery(fcstData, cur)                
        conn.commit()
        
    # DB connection 닫기
    conn.close()
    print(f'[{dt.now()}] [{baseDate} {baseTime}] DB 데이터 삽입')


def insertQuery(self, fcstData, cur):
    if 'TMN' in fcstData.keys():
        query = '''INSERT INTO weather (fcstDate, fcstTime, TMP, VEC, WSD, SKY, PTY, POP, PCP, REH, SNO, TMN)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'''
        cur.execute(query, (fcstData['fcstDate'], fcstData['fcstTime'], fcstData['TMP'], fcstData['VEC'],
                            fcstData['WSD'], fcstData['SKY'], fcstData['PTY'], fcstData['POP'],
                            fcstData['PCP'], fcstData['REH'], fcstData['SNO'], fcstData['TMN']))
        
    elif 'TMM' in fcstData.keys():
        query = '''INSERT INTO weather (fcstDate, fcstTime, TMP, VEC, WSD, SKY, PTY, POP, PCP, REH, SNO, TMM)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'''
        cur.execute(query, (fcstData['fcstDate'], fcstData['fcstTime'], fcstData['TMP'], fcstData['VEC'],
                            fcstData['WSD'], fcstData['SKY'], fcstData['PTY'], fcstData['POP'],
                            fcstData['PCP'], fcstData['REH'], fcstData['SNO'], fcstData['TMM']))
        
    else:
        query = '''INSERT INTO weather (fcstDate, fcstTime, TMP, VEC, WSD, SKY, PTY, POP, PCP, REH, SNO)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'''
        cur.execute(query, (fcstData['fcstDate'], fcstData['fcstTime'], fcstData['TMP'], fcstData['VEC'],
                            fcstData['WSD'], fcstData['SKY'], fcstData['PTY'], fcstData['POP'],
                            fcstData['PCP'], fcstData['REH'], fcstData['SNO']))

def updateDB(self,  baseDate, baseTime):
     # DB conn 연결
    conn = pymysql.connect(host='localhost', user = 'root', password='12345',
                           db = 'miniproject', charset='utf8')        
    cur = conn.cursor()    # Connection으로부터 Cursor 생성
    # SELECT 쿼리문
    query = '''SELECT DATE_FORMAT(fcstDate, '%Y%m%d'), date_format(fcstTime, '%H%i%s')
                 FROM weather'''
    cur.execute(query) 
    rows = cur.fetchall()
    # DB데이터 pandas.DataFrame에 넣기
    df = pd.DataFrame(rows)
    df.columns = ['Date', 'Time']

    # API 데이터
    allFcstData = self.newData(baseDate, baseTime)
    # API, DB 데이터 비교하여 Update 또는 Insert
    for fcstData in allFcstData:
        # 날짜, 시간이 같은 데이터 행 조회
        exitdb = df[(df['Date'] == fcstData['fcstDate']) & (df['Time'] == fcstData['fcstTime'])]
        if len(exitdb): # 있다면 Update
            self.updateQuery(fcstData, cur)
        else:   # 없다면 Insert
            self.insertQuery(fcstData, cur)
        conn.commit()
    conn.close()
    print(f'[{dt.now()}] [{baseDate} {baseTime}] 기상DB 업데이트') 


def updateQuery(self, fcstData, cur):
    if 'TMN' in fcstData.keys():
        query = '''UPDATE weather
                      SET TMP = %s, VEC = %s, WSD = %s, SKY = %s, PTY = %s, POP = %s, PCP = %s, REH = %s, SNO = %s, TMN = %s
                    WHERE fcstDate = %s + 00 And fcstTime = %s '''
        cur.execute(query, (fcstData['TMP'], fcstData['VEC'], fcstData['WSD']
                          , fcstData['SKY'], fcstData['PTY'], fcstData['POP']
                          , fcstData['PCP'], fcstData['REH'], fcstData['SNO']
                          , fcstData['TMN'], fcstData['fcstDate'], fcstData['fcstTime']))
        
    elif 'TMM' in fcstData.keys():
        query = '''UPDATE weather
                      SET TMP = %s, VEC = %s, WSD = %s, SKY = %s, PTY = %s, POP = %s, PCP = %s, REH = %s, SNO = %s, TMM = %s
                    WHERE fcstDate = %s + 00 And fcstTime = %s '''
        cur.execute(query, (fcstData['TMP'], fcstData['VEC'], fcstData['WSD']
                          , fcstData['SKY'], fcstData['PTY'], fcstData['POP']
                          , fcstData['PCP'], fcstData['REH'], fcstData['SNO']
                          , fcstData['TMM'], fcstData['fcstDate'], fcstData['fcstTime']))
    else:
        query = '''UPDATE weather
                      SET TMP = %s, VEC = %s, WSD = %s, SKY = %s, PTY = %s, POP = %s, PCP = %s, REH = %s, SNO = %s
                WHERE fcstDate = %s + 00 And fcstTime = %s '''
        cur.execute(query, (fcstData['TMP'], fcstData['VEC'], fcstData['WSD']
                          , fcstData['SKY'], fcstData['PTY'], fcstData['POP']
                          , fcstData['PCP'], fcstData['REH'], fcstData['SNO']
                          , fcstData['fcstDate'], fcstData['fcstTime']))

def baseDataeTime(self, mode, *pasttime):
    PARA_TIME = ['0200','0500', '0800', '1100', '1400', '1700', '2000', '2300']
    API_TIME = [datetime.time(2,10), datetime.time(5,10), datetime.time(8,10),
                datetime.time(11,10), datetime.time(14,10), datetime.time(17,10),
                datetime.time(20,10), datetime.time(23,10)]
    # 어제 오전 2시부터 오후 23시까지
    if mode == -1:           
        baseDate = f'{(dt.now().date() - td(days=1)).strftime("%Y%m%d")}'
        baseTime = PARA_TIME[pasttime[0]]      
        
        # 오늘중 현재시간까지의 API 기준시간 갯수
    elif mode == 0:   
        nowTime = dt.now().time()
        for i, time in enumerate(API_TIME[::-1]):
            if nowTime >= time:
                # baseTime = PARA_TIME[i]
                return (len(PARA_TIME)-i)
            elif nowTime < API_TIME[0]: # 현재 시각이 오전 2시이면 오늘 업데이트할 것 없음. 
                return -1
            
    elif mode == 1:
        baseDate = f'{dt.now().date().strftime("%Y%m%d")}'
        baseTime = PARA_TIME[pasttime[0]]
        
        # 자동 업데이트할 API 기준시간
    elif mode == 2:
        baseDate = f'{dt.now().date().strftime("%Y%m%d")}'
        baseTime = f'{dt.now().time().strftime("%H")}00'

    return (baseDate, baseTime)


def newData(self, baseDate, baseTime):
      # 초단기 예보 API 요청을 위한 쿼리 파라미터 설정
    queryParams_1 = '?' + urlencode({ quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('base_date') : base_date, quote_plus('pageNo') : 1,
                                      quote_plus('base_time') : base_time, quote_plus('nx') : 98, quote_plus('ny') : 76,
                                      quote_plus('dataType') : 'json', quote_plus('numOfRows') : '60'})
    
    # basetime, date 계산 

    

    response_predict = requests.get(url_predict + queryParams_1, verify=False)
    newFcstData  = response_predict.json().get('response').get('body').get('items').get('item')
    
    changeDate = newFcstData[0]['fcstDate']
    changeTime = newFcstData[0]['fcstTime']

    fcstData = dict()
    allFcstData = []
    CATEGORY = ['TMP', 'VEC', 'WSD', 'SKY', 'PTY', 'POP', 'PCP', 'REH', 'SNO', 'TMN', 'TMM']

    for item in newFcstData:
            # 필요없는 것 삭제
            del item['baseDate'], item['baseTime'], item['nx'], item['ny']

            for category in CATEGORY:
                # item 딕셔너리 키,값 정리           
                if item['category'] == category:
                    # (key:카테고리 명/ value: 수치) 추가
                    item[f'{category}'] = item['fcstValue']
                    break
            # (key:카테고리, key: 수치) 데이터 삭제    
            del item['category'], item['fcstValue'] 

            # 날짜, 시간 같으면 정리한 item을 fcstDate에 쌓기
            if changeDate == item['fcstDate'] and changeTime == item['fcstTime']:
                fcstData = fcstData|item
            # 날짜,시간 다르면 같은 시각의 데이터들을 모은
            else:   # 딕셔너리fcstDate를 allFcstData에 추가하고 초기화하고 item 담기
                fcstData['fcstTime'] = fcstData['fcstTime'] + '00'  # TIME : 000000
                allFcstData.append(fcstData)
                fcstData = dict()
                fcstData = item
                changeDate = item['fcstDate']   # 날짜 변경
                changeTime = item['fcstTime']   # 시간 변경

    fcstData['fcstTime'] = fcstData['fcstTime'] + '00'        
    allFcstData.append(fcstData)    # 마지막 날짜시각 데이터 allFcstDate에 추가
    return allFcstData

def autoUpdateDb(self):
    baseDate, baseTime = self.baseDateTime(2)
    self.updateDB(baseDate, baseTime)



# 예측 확률까지 더해야함 



# 초단기 실황 
while True:
  # 현재 날짜와 시간을 가져와서 원하는 형식으로 변환 
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

  # 초단기 실황 API 요청을 위한 쿼리 파라미터 설정 
  queryParams = '?' + urlencode({ quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('base_date') : base_date, quote_plus('pageNo') : 1,
                                      quote_plus('base_time') : base_time, quote_plus('nx') : 98, quote_plus('ny') : 76,
                                      quote_plus('dataType') : 'json', quote_plus('numOfRows') : '60'}) #페이지로 안나누고 한번에 받아오기 위해 numOfRows=60으로 설정해주었다
  

  # 실시간 부산광역시 하천 수위 API 요청을 위한 쿼리 파라미터 설정 
  queryParams_2 = '?' + urlencode(
                                    {
                                        'serviceKey' : serviceKey_water, 
                                        'pageNo' : '1',
                                        'numOfRow' : '100',
                                        'resultType' : 'json'
                                    })


  #  초단기 실황 값 요청 (웹 브라우저 서버에서 요청 - url주소와 파라미터) // 받아오는 거 확인 
  res = requests.get(url + queryParams, verify=False) # verify=False이거 안 넣으면 에러남ㅜㅜ
  items = res.json().get('response').get('body').get('items') #데이터들 아이템에 저장



  # 실시간 하천 수위 값 요청 (받아오는거 확인, 온천천 하류, 중앙여고, 온천장역 북측, 원동교(?))
  response_water = requests.get(url_water + queryParams_2, verify=False)
  items_water = response_water.json().get('getRvrwtLevelInfo').get('body').get('items').get('item')

#API 전체값 출력 코드(받을 수 있는지 확인)
#   if res.status_code == 200:
#       data = response_predict.json()
#       print(data)

#   else :
#       print("False Api load" , response_predict.status_code)


# ML 모델 돌리기 -> data 수정 

  # 저장된 모델 로드
#   stacking = load('Bigdata\model.joblib')

#   data = {
#       '기온(°C)': [float(items['item'][3]['obsrValue'])],
#       '풍향(deg)': [float(items['item'][5]['obsrValue'])],
#       '풍속(m/s)': [float(items['item'][7]['obsrValue'])],
#       '강수량(mm)': [float(items['item'][2]['obsrValue'])],
#   }

#   new_data = pd.DataFrame(data)

#   predictions = stacking.predict(new_data)

#   probabilities = stacking.predict_proba(new_data)

#   print(data)
#   print(f"침수확률 : {probabilities[0][1]:.2%}")

#   time.sleep(10)

