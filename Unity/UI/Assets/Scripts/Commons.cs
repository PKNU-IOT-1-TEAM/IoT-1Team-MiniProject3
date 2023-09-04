using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수위 API 정보를 담는 클래스
public class RiverFlowData
{
#nullable enable
	public string[] siteName = new string[2];
	public string[] waterLevel = new string[2];
	public string[] obsrTime = new string[2];
	public string[] alertLevel1 = new string[2];
	public string[] alertLevel2 = new string[2];
	public string[] alertLevel3 = new string[2];
	public string[] alertLevel4 = new string[2];
	public string[] sttus = new string[2];
}

// 예측, 현재 날씨 데이터를 담는 클래스
public class PredictData
{
	public string? predict;
	public string? basedate;
	public string? basetime;
	public string? temp;
	public string? deg;
	public string? rain;
	public string? windspeed;
	//public string[] predict;
	//public string[] basedate;
	//public string[] basetime;
	//public string[] temp;
	//public string[] deg;
	//public string[] rain;
	//public string[] windspeed;
}
// 예보 데이터를 담는 클래스
public class UltrasrtfcstData
{
#nullable enable
	public string[] FcstDate = new string[6];
	public string[] FcstTime = new string[6];
	public string[] T1H = new string[6];
	public string[] RN1 = new string[6];
	public string[] SKY = new string[6];
	public string[] REH = new string[6];
	public string[] PTY = new string[6];
	public string[] VEC = new string[6];
	public string[] WSD = new string[6];
}
// 주차장 현황 데이터를 담는 클래스
public class ParkingStatusData
{
	public string[] id_x= new string[8];
	public string[] parking_IR = new string[8];
	public string[] NFC = new string[8];
	public string[] reservation_status = new string[8];
}
public class WeatherPridictUltraData
{
	public string[] SKY = new string[1];
	public string[] PTY = new string[1];
}

public class Commons
{
	// 데이터를 저장할 객체들
	public static RiverFlowData riverFlowData = new RiverFlowData();
	public static PredictData predictData = new PredictData();
	public static UltrasrtfcstData ultrasrtfcstData = new UltrasrtfcstData();
	public static ParkingStatusData parkingStatusData = new ParkingStatusData();
	public static WeatherPridictUltraData weatherPridictUltraData = new WeatherPridictUltraData();
}
