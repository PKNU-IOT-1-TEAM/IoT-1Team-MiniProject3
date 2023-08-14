# Library 선언
import pymysql 
import requests
import pandas as pd
import schedule
import time
import urllib3

from joblib import load
from urllib.parse import unquote, urlencode, quote_plus 
from datetime import datetime, timedelta

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# 현재 시간 함수
def base_Time():
    now = datetime.now()
    base_time = ""

    if  now.minute > 0 and now.minute < 30 :
        base_time = f"{now.hour-1:02d}30"
    
    elif now.minute > 30:
         base_time = f"{now.hour-1:02d}00"

    return base_time

# 현재 날짜 함수
def base_Date():
    today = datetime.today()
    y = today- timedelta(days=1)
    base_date = today.strftime("%Y%m%d")

    return base_date

# API LOAD 해서 저장하는 함수
def api_load(url, serviceKey):
    base_time = base_Time()
    base_date = base_Date()

    serviceKeyDecoded = unquote(serviceKey, 'UTF-8')

    queryParams = '?' + urlencode({quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('base_date') : base_date, quote_plus('pageNo') : 1,
                                        quote_plus('base_time') : base_time, quote_plus('nx') : 98, quote_plus('ny') : 76,
                                        quote_plus('dataType') : 'json', quote_plus('numOfRows') : '60'}) #페이지로 안나누고 한번에 받아오기 위해 numOfRows=60으로 설정해주었다

    res = requests.get(url + queryParams, verify=False)
    data = res.json().get('response').get('body').get('items')

    return data

# 강물 수위 API 가져와서 필요한 강만 추출하는 함수
def get_river(river_url, serviceKey):
    result = []

    serviceKeyDecoded = unquote(serviceKey, 'UTF-8') 

    queryParams = '?' + urlencode({quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('pageNo') : 1,
                                        quote_plus('resultType') : 'json', quote_plus('numOfRows') : '20'}) #페이지로 안나누고 한번에 받아오기 위해 numOfRows=60으로 설정해주었다

    res = requests.get(river_url + queryParams, verify=False)
    datas = res.json()['getRvrwtLevelInfo']['body']['items']['item']

    for data in datas:
        if (data['siteName'] == '연안교'):
            result.append(data)

    return result

# 실시간 날씨 API를 모델에 예측하는 함수
def get_predict():
  live_data = api_load(live_url, serviceKey)
  data = {
        'basedate' : [live_data['item'][0]['baseDate']],
        'basetime' : [live_data['item'][0]['baseTime']],
        '기온(°C)': [float(live_data['item'][3]['obsrValue'])],
        '풍향(deg)': [float(live_data['item'][5]['obsrValue'])],
        '풍속(m/s)': [float(live_data['item'][7]['obsrValue'])],
        '강수량(mm)': [float(live_data['item'][2]['obsrValue'])]
    }

  new_data = pd.DataFrame(data)
  probabilities = stacking.predict_proba(new_data.drop(columns = ['basedate', 'basetime']))

  return round(probabilities[0][1] * 100, 2), data

# 날씨 예보 API를 가져와서 dictionary로 변환하는 함수
def get_forecast():
    data = api_load(short_predict, serviceKey)
    weather_data = dict()

    for item in data['item']:
        Date = item['fcstDate']
        Time = item['fcstTime']
        if item['fcstDate'] not in weather_data:
            weather_data[Date] = dict()
        if item['fcstTime'] not in weather_data[Date]:
            weather_data[Date][Time] = dict()

        if item['category'] == "T1H":
            weather_data[Date][Time]['T1H'] = item['fcstValue']
        elif item['category'] == "RN1":
            weather_data[Date][Time]['RN1'] = item['fcstValue']
        elif item['category'] == "SKY":
            weather_data[Date][Time]["SKY"] = item['fcstValue']
        elif item['category'] == "UUU":
            weather_data[Date][Time]['UUU'] = item['fcstValue']
        elif item['category'] == "VVV":
            weather_data[Date][Time]['VVV'] = item['fcstValue']
        elif item['category'] == "REH":
            weather_data[Date][Time]['REH'] = item['fcstValue']
        elif item['category'] == "PTY":
            weather_data[Date][Time]['PTY'] = item['fcstValue']
        elif item['category'] == "LGT":
            weather_data[Date][Time]['LGT'] = item['fcstValue']
        elif item['category'] == "VEC":
            weather_data[Date][Time]['VEC'] = item['fcstValue']
        elif item['category'] == 'WSD':
            weather_data[Date][Time]['WSD'] = item['fcstValue']

    return weather_data

# DB에 연결하는 함수
def connect_to_DB():
    conn = pymysql.connect(
        host = '210.119.12.112',
        port = 10000,
        user = 'pi',
        password = '12345',
        database = 'team1_iot'
    )
    return conn

# 날씨 예보 API INSERT
def InsertDB(input_data, tmp_day, tmp_time, bday, btime):

    try:
        conn = connect_to_DB()
        cur = conn.cursor()

        total_qry = f"""INSERT INTO ultrasrtfcst
                        (fcstDate, fcstTime, BaseDate, BaseTime, T1H, RN1, SKY, UUU, VVV, REH, PTY, LGT, VEC, WSD) VALUES
                            ('{tmp_day}', '{tmp_time}', '{bday}', '{btime}', {input_data['T1H']}, 
                            '{input_data['RN1']}', '{input_data['SKY']}', {input_data['UUU']}, 
                            {input_data['VVV']}, {input_data['REH']}, '{input_data['PTY']}',
                            {input_data['LGT']}, {input_data['VEC']}, {input_data['WSD']})"""
        
        cur.execute(total_qry)

        conn.commit()
        conn.close()
    except Exception as e:
        print(f'{e}')

# 날씨 예보 API UPDATE
def UpdateDB(input_data, tmp_day, tmp_time, bday, btime):
    try:
        conn = connect_to_DB()
        cur = conn.cursor()
        query = '''UPDATE ultrasrtfcst SET 
                        T1H = %s, RN1 = %s, SKY = %s,
                        UUU = %s, VVV = %s, REH = %s,
                        PTY = %s, LGT = %s, VEC = %s, 
                        WSD = %s, BaseDate = %s, BaseTime = %s  WHERE fcstDate = %s AND fcstTime = %s '''
        
        cur.execute(query, (input_data['T1H'],input_data['RN1'],input_data['SKY'],input_data['UUU'],
                            input_data['VVV'],input_data['REH'],input_data['PTY'],input_data['LGT'],
                            input_data['VEC'],input_data['WSD'], bday, btime, tmp_day, tmp_time))
        conn.commit()
        conn.close()
    except Exception as e:
        print(f'{e}')

# 예측한 값 DB INSERT or UPDATE
def InsertPredictDB():
    try:
        conn = connect_to_DB()
        cur = conn.cursor()

        predict, live_data = get_predict()

        bday = live_data['basedate'][0][0:4] + '-' + live_data['basedate'][0][4:6] + '-' + live_data['basedate'][0][6:]
        btime = live_data['basetime'][0][0:2] + ':' + live_data['basetime'][0][2:] + ":00"

        search_query = f'''
                        SELECT EXISTS (SELECT Idx FROM predict WHERE BaseDate = '{bday}' and BaseTime = '{btime}') AS SUCCESS;
                        '''
        cur.execute(search_query)        
        result = cur.fetchone()[0]

        if result == 0 :
            total_qry = f"""INSERT INTO predict
                            (predict, basedate, basetime, temp, deg, rain, windspeed) VALUES
                                ('{predict}', '{bday}', '{btime}', '{live_data['기온(°C)'][0]}', '{live_data['풍향(deg)'][0]}','{live_data['강수량(mm)'][0]}', '{live_data['풍속(m/s)'][0]}')"""
    
            cur.execute(total_qry)
        conn.commit()
        conn.close()
    except Exception as e:
        print(f'{e}')

# 강물 수위 데이터 DB INSERT (5m)
def InsertRiverDB():
    result = get_river(river_url, serviceKey)
    for data in result:
        try:
            conn = connect_to_DB()
            cur = conn.cursor()

            total_qry = f"""INSERT INTO riverflow
                            (siteName, waterLevel, dayLevelMax, obsrTime, alertLevel1, alertLevel1Nm, alertLevel2, alertLevel2Nm, 
                            alertLevel3, alertLevel3Nm, alertLevel4, alertLevel4Nm, sttus, sttusNm) VALUES
                                ('{data['siteName']}','{data['waterLevel']}', '{data['dayLevelMax']}', '{data['obsrTime']}','{data['alertLevel1']}', '{data['alertLevel1Nm']}',
                                '{data['alertLevel2']}', '{data['alertLevel2Nm']}','{data['alertLevel3']}', '{data['alertLevel3Nm']}','{data['alertLevel4']}', '{data['alertLevel4Nm']}',
                                '{data['sttus']}', '{data['sttusNm']}')"""
            
            cur.execute(total_qry)

            conn.commit()
            conn.close()
        except Exception as e:
            print(f'{e}')

# 날씨 예보 값이 DB에 있는지 없는지 점검 (있으면 UPDATE, 없으면 INSERT)
def weather_data_insert(weather_data):
    for day in weather_data:
        for time in weather_data[day]:
            conn = connect_to_DB()
            cur = conn.cursor()
            
            tmp_day = day[0:4] + '-' + day[4:6] + '-' + day[6:]
            tmp_time = time[0:2] + ':' + time[2:]
            
            bday = base_Date()[0:4] + '-' + base_Date()[4:6] + '-' + base_Date()[6:]
            btime = base_Time()[0:2] + ':' + base_Time()[2:] + ':00'
            
            search_query = f'''
                            SELECT EXISTS (SELECT Idx FROM ultrasrtfcst WHERE FcstDate = '{tmp_day}' and FcstTime = '{tmp_time}' and BaseDate = '{bday}' and BaseTime = '{btime}') AS SUCCESS;
                            '''
            cur.execute(search_query)        
            result = cur.fetchone()[0]
            conn.close()
            
            input_data = weather_data[day][time]
            if result == 1 :
                UpdateDB(input_data, tmp_day, tmp_time, bday, btime) 
            else :
                InsertDB(input_data, tmp_day, tmp_time, bday, btime)

def job():
    conn = connect_to_DB()
    cur = conn.cursor()

    weather_data = get_forecast()
    weather_data_insert(weather_data)
    InsertPredictDB()
    InsertRiverDB()
    
    conn.commit()
    conn.close()

    print(time.strftime('%Y.%m.%d - %H:%M:%S'), '실행 중입니다.')

# 사용하는 변수 선언 (__main__ 문 안에 넣어도 무방)
live_url = "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst"
short_predict= "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtFcst" # 초단기 예보 (저장용)
river_url = "https://apis.data.go.kr/6260000/BusanRvrwtLevelInfoService/getRvrwtLevelInfo"

serviceKey = "wGokgRxD1t3z5G4u7MsWumpoCeiWO8JM6yZ87rX1ELTO9nMSUuMOQjHj70rAzuopgyB1iLdKX0S9WK0RLs88bQ==" # 공공데이터 포털에서 생성된 본인의 서비스 키를 복사 / 붙여넣기
serviceKeyDecoded = unquote(serviceKey, 'UTF-8') # 공공데이터 포털에서 제공하는 서비스키는 이미 인코딩된 상태이므로, 디코딩하여 사용해야 함 -> 초단기 실황(예보도 동일)

stacking = load('model.joblib')

# 이외 해야 하는 것 
# (1) 강물 수위 API 삽입하는 코드 작성 - complete-
# (2) 각기 저장하고 싶다면 멀티 스레드 사용
#     그러고 싶지 않으면 업데이트 되는 30분 마다 실행되는 time.sleep() 사용하거나 schedule 사용

# 초단기실황 - 정시단위 (매시각 40분 이후 호출)
# 초단기예보 - 30분 단위 (매시각 45분 이후 호출)
# 강물 수위 - 5분 단위
if __name__ == "__main__":
    while True:
        job()
        time.sleep(1500)
