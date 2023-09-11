using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class SensorData
{
    #nullable enable
    // ���������1 ����
    public string? AD1_RCV_IR_Sensor;
    public string? AD1_RCV_Temperature;
    public string? AD1_RCV_Humidity;
    public string? AD1_RCV_Dust;
    public string? AD1_RCV_Parking_Status;
    #nullable disable
	public int AD2_RCV_CGuard = 0;  // ���ܺ� ����
    public float AD3_RCV_WGuard_Wave = 0; // ������ �Ÿ� cm
    public int AD4_RCV_WL_CNNT = 0;
    #nullable enable
    public string? AD4_RCV_NFC;

	// ���� ���� ����
    #nullable disable
	public string AD2_State_CGuard = "����";  // ���� ����.   // ���ܺ� ����,����.
    public string AD3_State_WPump = "OFF"; // ���� ����. // �����ߺ� ���� on/off
    public string AD3_State_WGuard = "�ϰ� ����";  // ���� ����. // ������ ���� ���� ��, ��� ����, �ϰ� ����
    #nullable enable

	// ���������2
	// ���������2���� ������ ������ ������ ����
	public string? IR_CAR_NUM;
	public string? CAR_NUMBER;

}