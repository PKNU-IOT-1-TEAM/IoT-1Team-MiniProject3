using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;
using System.IO;
using System.Text;

public class WeatherAPIExample : MonoBehaviour
{
    // 기상청 API 요청 URL
    // 단기예보 (3시간 단위) API URL
    // 초단기실황 (10분 단위) API URL

    // Start is called before the first frame update
    void Start()
    {
        string apiUrl = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst";
        string apiKey = "yR%2BIgD8xUGGLPLv5yhXB%2F1N8mrSZZShOsMcBdKjRIHdtT2R1%2FDttqVn95bmWMWkbyEbcz%2FJ8qqJaoq5sRLTbbw%3D%3D";
        string pageNo = "1";
        string numOfRows = "1000";
        string dataType = "JSON";
        string baseDate = DateTime.Now.ToString("yyyyMMdd");
        string baseTime = "0900";
        string nx = "98";
        string ny = "76";

        // API URL에 파라미터와 API 인증 키를 추가하여 요청 URL을 생성합니다.
        string requestUrl = $"{apiUrl}?serviceKey={apiKey}" +
            $"&pageNo={pageNo}&numOfRows={numOfRows}&dataType={dataType}" +
            $"&base_date={baseDate}&base_time={baseTime}" +
            $"&nx={nx}&ny={ny}";

        GET(requestUrl);
    }

    // GET JSON Response
    public void GET(string url)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        try
        {
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                string data = reader.ReadToEnd();
                Debug.Log(data);
            }
        }
        catch (WebException ex)
        {
            WebResponse errorResponse = ex.Response;
            using (Stream responseStream = errorResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                string errorText = reader.ReadToEnd();
                // log errorText
            }
            throw;
        }

    }

        // Update is called once per frame
        void Update()
    {

    }

}