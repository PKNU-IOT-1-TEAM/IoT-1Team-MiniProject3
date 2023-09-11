using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class SensorData
{
    #nullable enable
    // 라즈베리파이1 센서
    public string? AD1_RCV_IR_Sensor;
    public string? AD1_RCV_Temperature;
    public string? AD1_RCV_Humidity;
    public string? AD1_RCV_Dust;
    public string? AD1_RCV_Parking_Status;
    #nullable disable
	public int AD2_RCV_CGuard = 0;  // 차단봉 각도
    public float AD3_RCV_WGuard_Wave = 0; // 차수막 거리 cm
    public int AD4_RCV_WL_CNNT = 0;
    #nullable enable
    public string? AD4_RCV_NFC;

	// 센서 상태 저장
    #nullable disable
	public string AD2_State_CGuard = "닫힘";  // 새로 만듦.   // 차단봉 열림,닫힘.
    public string AD3_State_WPump = "OFF"; // 새로 만듦. // 워터펌브 상태 on/off
    public string AD3_State_WGuard = "하강 상태";  // 새로 만듦. // 차수막 상태 동작 중, 상승 상태, 하강 상태
    #nullable enable

	// 라즈베리파이2
	// 라즈베리파이2에서 보내는 데이터 저장할 변수
	public string? IR_CAR_NUM;
	public string? CAR_NUMBER;

}