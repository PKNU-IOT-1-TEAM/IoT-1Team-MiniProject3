using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TxtSensor : MonoBehaviour
{
	public Text temperature;        // 온도
	public Text humidity;           // 습도
	public Text dust;               // 미세먼지 수치
	public Text WGuard;             // 리니어 모터 상태
	public Text WPump;              // 워터펌프 동작 상태
	public Text CNNT;               // 주차장 내부 수위
	public Text CGuard;             // 차단봉 상태

	public Image arrowIma_Temp;		// 온도계 
	public Image arrowIma_Humid;	// 습도계
	public Image arrowIma_Dust;		// 미세먼지 계기판
	public Image arrowIma_CNNT;     // 수위 계기판
	private void Start()
	{

	}
	// Update is called once per frame
	void Update()
    {
		temperature.text = EventManager.Instance.Sensor_Data.AD1_RCV_Temperature + "˚C";
		humidity.text = EventManager.Instance.Sensor_Data.AD1_RCV_Humidity + "%";
		dust.text = EventManager.Instance.Sensor_Data.AD1_RCV_Dust + "㎍/m³";
		CGuard.text = string.IsNullOrEmpty(EventManager.Instance.Sensor_Data.AD2_State_CGuard) ? "OFF" : EventManager.Instance.Sensor_Data.AD2_State_CGuard;    // 차단봉 열림/ 닫힘
		WGuard.text = EventManager.Instance.Sensor_Data.AD3_State_WGuard; // 차수막 동작중/상승완료/하강완료
		WPump.text = EventManager.Instance.Sensor_Data.AD3_State_WPump; // 워터펌프 on/off
		CNNT.text = EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT + "mm";

		// 화살표 이미지의 각도 설정
		arrowIma_Temp.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Temperature));
		arrowIma_Humid.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Humidity));
		arrowIma_Dust.rectTransform.localEulerAngles = new Vector3(0, 0, -float.Parse(EventManager.Instance.Sensor_Data.AD1_RCV_Dust));
		arrowIma_CNNT.rectTransform.localEulerAngles = new Vector3(0, 0, -EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT);

		// 텍스트 색상

		WGuard.color = EventManager.Instance.Sensor_Data.AD3_State_WGuard == "동작 중" ? Color.red : Color.blue;
	
    }
}
