using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TxtSensor : MonoBehaviour
{
	public Text temperature;        // �µ�
	public Text humidity;           // ����
	public Text dust;               // �̼����� ��ġ
	public Text WGuard;             // ���Ͼ� ���� ����
	public Text WPump;              // �������� ���� ����
	public Text CNNT;               // ������ ���� ����
	public Text CGuard;             // ���ܺ� ����

	public Image arrowIma_Temp;		// �µ��� 
	public Image arrowIma_Humid;	// ������
	public Image arrowIma_Dust;		// �̼����� �����
	public Image arrowIma_CNNT;     // ���� �����
	private void Start()
	{

	}
	// Update is called once per frame
	void Update()
    {
		temperature.text = EventManager.Instance.Sensor_Data.AD1_RCV_Temperature + "��C";
		humidity.text = EventManager.Instance.Sensor_Data.AD1_RCV_Humidity + "%";
		dust.text = EventManager.Instance.Sensor_Data.AD1_RCV_Dust + "��/m��";
		CGuard.text = string.IsNullOrEmpty(EventManager.Instance.Sensor_Data.AD2_State_CGuard) ? "OFF" : EventManager.Instance.Sensor_Data.AD2_State_CGuard;    // ���ܺ� ����/ ����
		WGuard.text = EventManager.Instance.Sensor_Data.AD3_State_WGuard; // ������ ������/��¿Ϸ�/�ϰ��Ϸ�
		WPump.text = EventManager.Instance.Sensor_Data.AD3_State_WPump; // �������� on/off
		CNNT.text = EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT + "mm";

		// ȭ��ǥ �̹����� ���� ����
		arrowIma_Temp.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Temperature));
		arrowIma_Humid.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Humidity));
		arrowIma_Dust.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Dust));
		arrowIma_CNNT.rectTransform.localEulerAngles = new Vector3(0, 0, -EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT);

		// �ؽ�Ʈ ����

		WGuard.color = EventManager.Instance.Sensor_Data.AD3_State_WGuard == "���� ��" ? Color.red : Color.blue;
	
    }
}
