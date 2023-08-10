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
    // ���û API ��û URL
    // �ܱ⿹�� (3�ð� ����) API URL
    // �ʴܱ��Ȳ (10�� ����) API URL

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

        // API URL�� �Ķ���Ϳ� API ���� Ű�� �߰��Ͽ� ��û URL�� �����մϴ�.
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