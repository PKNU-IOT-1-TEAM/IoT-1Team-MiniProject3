using System;
using System.Data;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;



public class API_Data : MonoBehaviour
{
	// 데이터를 저장할 객체들
	private float updateInterval = 10f;// 업데이트 주기 (10초)

	private void Start()
	{
		InvokeRepeating("UpdateRiverFlow", 0f, updateInterval); // 최초 실행 후 5분 마다 UpdateRiverFlow 함수를 호출
	}
// #pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.

	private void UpdateRiverFlow()
	{


		using (MySqlConnection conn = new MySqlConnection(NFC_IR_Sensor.conn_string)) // 기존에 NFC_IR_Sensor의 DB연결 스트링 재사용
		{
			conn.Open();
			try
			{
				// 강의 흐름 정보 가져오기
				string River_sql = "SELECT siteName, waterLevel, obsrTime, alertLevel1, alertLevel2, alertLevel3, alertLevel4, sttus FROM team1_iot.riverflow WHERE siteName ='연안교' OR siteName = '중앙여고' ORDER BY idx DESC LIMIT 2;";
				MySqlDataAdapter m_adapter = new MySqlDataAdapter(River_sql, conn);
				DataSet ds = new DataSet();
				m_adapter.Fill(ds, "riverflow");
				DataTable dt_riverflow = ds.Tables[0];

				// Debug.Log(dt_riverflow);

				var loop = 0;
				foreach (DataRow item in dt_riverflow.Rows)
				{
					Commons.riverFlowData.siteName[loop] = item[0].ToString();
					Commons.riverFlowData.waterLevel[loop] = (item[1].ToString());
					Commons.riverFlowData.obsrTime[loop] = (item[2].ToString());
					Commons.riverFlowData.alertLevel1[loop] = (item[3].ToString());
					Commons.riverFlowData.alertLevel2[loop] = (item[4].ToString());
					Commons.riverFlowData.alertLevel3[loop] = (item[5].ToString());
					Commons.riverFlowData.alertLevel4[loop] = (item[6].ToString());
					Commons.riverFlowData.sttus[loop] = (item[7].ToString());
					loop++;
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}

		using (MySqlConnection conn = new MySqlConnection(NFC_IR_Sensor.conn_string)) // 기존에 nfc_ir_sensor의 db연결 재사용
		{
			try
			{
				// 기상 데이터 가져오기
				string predict_sql = "select predict, basedate, basetime, temp, deg, rain, windspeed from team1_iot.predict order by idx desc limit 1";
				MySqlDataAdapter m_adapter = new MySqlDataAdapter(predict_sql, conn);
				DataSet ds = new DataSet();
				m_adapter.Fill(ds, "predict");
				DataTable dt_predict = ds.Tables[0];


				Commons.predictData.predict= dt_predict.Rows[0][0].ToString();
				Commons.predictData.basedate = dt_predict.Rows[0][1].ToString();
				Commons.predictData.basetime = dt_predict.Rows[0][2].ToString();
				Commons.predictData.temp = dt_predict.Rows[0][3].ToString();
				Commons.predictData.deg = dt_predict.Rows[0][4].ToString();
				Commons.predictData.rain = dt_predict.Rows[0][5].ToString();
				Commons.predictData.windspeed = dt_predict.Rows[0][6].ToString();
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		using (MySqlConnection conn = new MySqlConnection(NFC_IR_Sensor.conn_string)) // 기존에 nfc_ir_sensor의 db연결 재사용
		{
			try
			{
				// 기상 데이터 가져오기
				string ultrasrtfcst_sql = $"SELECT FcstDate, FcstTime, T1H, RN1, SKY, REH, PTY, VEC, WSD FROM team1_iot.ultrasrtfcst ORDER BY idx DESC LIMIT 6";
				MySqlDataAdapter m_adapter = new MySqlDataAdapter(ultrasrtfcst_sql, conn);
				DataSet ds = new DataSet();
				m_adapter.Fill(ds, "ultrasrtfcst");
				DataTable dt_ultrasrtfcst = ds.Tables[0];

				// Debug.Log(dt_ultrasrtfcst);

				var loop = 0;
				foreach (DataRow item in dt_ultrasrtfcst.Rows)
				{
					Commons.ultrasrtfcstData.FcstDate[loop] = (item[0].ToString());// 기준 날짜
					Commons.ultrasrtfcstData.FcstTime[loop] = (item[1].ToString()); // 기준 시간
					Commons.ultrasrtfcstData.T1H[loop] = (item[2].ToString()); // 기온
					if (item[3].ToString() == "강수없음")
					{
						Commons.ultrasrtfcstData.RN1[loop] = "0";
					}
                    else if (item[3].ToString().Contains(".0mm") == true)
					{
						Commons.ultrasrtfcstData.RN1[loop] = item[3].ToString().Replace(".0mm", "");
					}
					else
                    {
						Commons.ultrasrtfcstData.RN1[loop] = item[3].ToString();
                    }
					Commons.ultrasrtfcstData.SKY[loop] = (item[4].ToString());// 하늘상태
					Commons.ultrasrtfcstData.REH[loop] = (item[5].ToString()); // 습도
					Commons.ultrasrtfcstData.PTY[loop] = (item[6].ToString()); // 강수형태
					Commons.ultrasrtfcstData.VEC[loop] = (item[7].ToString()); // 풍향
					Commons.ultrasrtfcstData.WSD[loop] = (item[8].ToString()); // 풍속

					loop++;
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		// parkingStatusData
		using (MySqlConnection conn = new MySqlConnection(NFC_IR_Sensor.conn_string)) // 기존에 nfc_ir_sensor의 db연결 재사용
		{
			try
			{
				// 기상 데이터 가져오기
				string parkingStatus_sql = $"SELECT id_x, parking_IR, NFC, reservation_status FROM team1_iot.parking_status;";
				MySqlDataAdapter m_adapter = new MySqlDataAdapter(parkingStatus_sql, conn);
				DataSet ds = new DataSet();
				m_adapter.Fill(ds, "parking_status");
				DataTable dt_parkingStatus = ds.Tables[0];

				// Debug.Log(dt_parkingStatus);

				var loop = 0;
				foreach (DataRow item in dt_parkingStatus.Rows)
				{
					Commons.parkingStatusData.id_x[loop] = (item[0].ToString());// idx
					Commons.parkingStatusData.parking_IR[loop] = (item[1].ToString()); // IR 상태
					Commons.parkingStatusData.NFC[loop] = (item[2].ToString()); // NFC
					Commons.parkingStatusData.reservation_status[loop] = (item[3].ToString());// reservation_status

					loop ++;
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		// weatherPridictUltraData 날씨 표시
		using (MySqlConnection conn = new MySqlConnection(NFC_IR_Sensor.conn_string)) // 기존에 nfc_ir_sensor의 db연결 재사용
		{
			try
			{
				// 기상 데이터 가져오기
				string weatherPridictUltraData_sql = $"SELECT ul.SKY, ul.PTY FROM team1_iot.ultrasrtfcst AS ul INNER JOIN (SELECT basetime, basedate FROM team1_iot.predict ORDER BY idx DESC LIMIT 1 ) AS pc ON ul.FcstDate = pc.basedate AND ul.FcstTime >= pc.basetime LIMIT 1;";
				MySqlDataAdapter m_adapter = new MySqlDataAdapter(weatherPridictUltraData_sql, conn);
				DataSet ds = new DataSet();
				m_adapter.Fill(ds, "parking_status");
				DataTable dt_weatherPridictUltraData = ds.Tables[0];

				// Debug.Log(dt_weatherPridictUltraData);

				var loop = 0;
				foreach (DataRow item in dt_weatherPridictUltraData.Rows)
				{
					Commons.weatherPridictUltraData.SKY[loop] = (item[0].ToString()); // SKY
					Commons.weatherPridictUltraData.PTY[loop] = (item[1].ToString()); // PTY
					loop++;
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		Debug.Log("DB 연결 해제!");
	}
}