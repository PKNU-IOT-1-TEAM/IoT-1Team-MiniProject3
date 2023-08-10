import requests
import pandas as pd
import time
import datetime
from urllib.parse import unquote, urlencode, quote_plus
from datetime import datetime as dt, timedelta as td
import pymysql
import json
import schedule
from joblib import load

url_predict= "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtFcst" # 초단기 예보 (저장용)

# 초단기 예보 강물 데이터 API를 받아오는 소스 작성 json -> 원하는 값만 뽑아서 data랑  강수예측 % 랑 같이 DB에 업데이트 하는 코드 
 
serviceKey= "wGokgRxD1t3z5G4u7MsWumpoCeiWO8JM6yZ87rX1ELTO9nMSUuMOQjHj70rAzuopgyB1iLdKX0S9WK0RLs88bQ==" # 공공데이터 포털에서 생성된 본인의 서비스 키를 복사 / 붙여넣기

serviceKeyDecoded = unquote(serviceKey, 'UTF-8') # 공공데이터 포털에서 제공하는 서비스키는 이미 인코딩된 상태이므로, 디코딩하여 사용해야 함 -> 초단기 실황(예보도 동일)

# DB 예보 한줄 삽입

def base_Time():
    now = dt.now()
    base_time = ""

    if now.minute < 45:
        base_time = f"{now.hour: 02d}30"
    
    else:
        next_hour = now.hour +1
        base_time = f"{next_hour: 02d}00"

    return base_time

def base_Date():
    today = dt.today()
    y = today- td(days=1)
    yesterday = y.strftime("%Y%m%d")
    base_date = today.strftime("%Y%m%d")

    return yesterday, base_date

def predict_weather(db_config):
    url_predict= "https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtFcst" 
    serviceKey= "wGokgRxD1t3z5G4u7MsWumpoCeiWO8JM6yZ87rX1ELTO9nMSUuMOQjHj70rAzuopgyB1iLdKX0S9WK0RLs88bQ=="
    serviceKeyDecoded = unquote(serviceKey, 'UTF-8')

    base_time = base_Time()
    yesterday , base_date = base_Date()

    queryParams = '?' + urlencode({quote_plus('serviceKey') : serviceKeyDecoded, quote_plus('base_date') : base_date, quote_plus('pageNo') : 1,
                                      quote_plus('base_time') : base_time, quote_plus('nx') : 98, quote_plus('ny') : 76,
                                      quote_plus('dataType') : 'json', quote_plus('numOfRows') : '60'}) #페이지로 안나누고 한번에 받아오기 위해 numOfRows=60으로 설정해주었다
    
    res = requests.get(url_predict, params=queryParams,  verify=False)
    data = res.json().get('response').get('body').get('items').get('item')

    weather_data = dict()

    for item in data:
        if item['category'] == 'T1H':
            weather_data['T1H'] = item['fcstValue']
        if item['category'] == 'RN1':
            weather_data['RN1'] = item['fcstValue']
        if item['category'] == 'SKY':
            weather_data['SKY'] = item['fcstValue']
        if item['category'] == '':
            weather_data['UUU'] = item['fcstValue']
        if item['category'] == 'VVV':
            weather_data['VVV'] = item['fcstValue']
        if item['category'] == 'REH':
            weather_data['REH'] = item['fcstValue']
        if item['category'] == 'PTY':
            weather_data['PTY'] = item['fcstValue']  
        if item['category'] == 'LGT':
            weather_data['LGT'] = item['fcstValue']
        if item['category'] == 'VEC':
            weather_data['VEC'] = item['fcstValue']
        if item['category'] == 'WSD':
            weather_data['WSD'] = item['fcstValue']

    try:
        connection = pymysql.connect(**db_config)
        with connection.cursor() as cursor:
            query = '''INSERT INTO predict_w (fcstDate, fcstTime, T1H, RN1, SKY, UUU, VVV, REH, PTY, LGT, VEC, WSD )
                    VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'''
            values = (data[0]['fcstDate'], data[0]['fcstTime'], weather_data['T1H'], weather_data['RN1'], weather_data['SKY'], 
                      weather_data['UUU'], weather_data['VVV'], weather_data['REH'], weather_data['PTY'], weather_data['LGT'], 
                      weather_data['VEC'], weather_data['WSD'])
            cursor.execute(query, values)
            connection.commit()
    except Exception as e:
        print("Error : ", e)
    finally:
        connection.close()

    


# def InsertDB (data, weather_data ,db_config):
#     try:
#         connection = pymysql.connect(**db_config)
#         with connection.cursor() as cursor:
#             for item in data:
#                 query = '''INSERT INTO predict_w (fcstDate, fcstTime, T1H, RN1, SKY, UUU, VVV, REH, PTY, LGT, VEC, WSD )
#                         VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'''
#                 values = (item['fcstDate'], item['fcstTime'], data['T1H'], weather_data['RN1'], weather_data['SKY'], 
#                           weather_data['UUU'], weather_data['VVV'], weather_data['REH'], weather_data['PTY'], weather_data['LGT'], 
#                           weather_data['VEC'], weather_data['WSD'])
#                 cursor.execute(query, values)
#             connection.commit()
#     except Exception as e:
#         print("Error : ", e)
#     finally:
#         connection.close()
    
    
if __name__ == "__main__":

    db_config = {
        'host' : 'localhost',
        'user' : 'root',
        'password' : '12345',
        'db' : 'team1_iot',
        'charset' : 'utf8mb4',
        'cursorclass': pymysql.cursors.DictCursor
    }
    predict_weather(db_config)


