using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� API ������ ��� Ŭ����
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

// ����, ���� ���� �����͸� ��� Ŭ����
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
// ���� �����͸� ��� Ŭ����
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
// ������ ��Ȳ �����͸� ��� Ŭ����
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
	// �����͸� ������ ��ü��
	public static RiverFlowData riverFlowData = new RiverFlowData();
	public static PredictData predictData = new PredictData();
	public static UltrasrtfcstData ultrasrtfcstData = new UltrasrtfcstData();
	public static ParkingStatusData parkingStatusData = new ParkingStatusData();
	public static WeatherPridictUltraData weatherPridictUltraData = new WeatherPridictUltraData();
}
