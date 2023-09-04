using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class WGuard_Controller : MonoBehaviour
//{
//    private string WGuard_Cube = "WGuard_Cube"; // 찾을 차수벽 오브젝트 이름

//    private GameObject WGuard; // 차수벽 오브젝트 참조 변수

//    private float updateInterval = 0.5f; // 업데이트 주기 (1초)

//    private float WGuard_SensorData_Level; // 초음파 센서로부터 읽은 차수벽 거리 데이터

//    // 프로그램이 시작 할 때랑 시작 했을 때 좌표 값 기준이 다르기 때문에 주의 해야된다.
//    // 차수벽 오브젝트 최초 위치 값 Y = 532652f : 물이 인식 되었을 때 오브젝트 시작 위치 = 1035739f
//    // 기본 위치
//    // WGuard_Cube : -410129 532650.9 -5011084 로컬 좌표계
//    // -410129 532650.9f -5011084
//    // -410129 1035739f -5011084


//    void Start()
//    {
//        // "WGuard_Cube" 이름의 차수벽 오브젝트를 찾아서 변수에 대입
//        WGuard = GameObject.Find(WGuard_Cube);
//        //Debug.Log(WGuard.transform.position);

//        // 모든 수위 오브젝트가 찾아졌는지 확인
//        if (WGuard != null)
//        {
//            Debug.Log("차수벽 오브젝트 찾기 완료");

//            // 주기적으로 센서 값을 읽어오는 함수를 호출
//            // 1초 단위 Read_IS_WL 함수 실행
//            InvokeRepeating("Read_WGuard", 0f, updateInterval);
//        }
//    }

//    private void Read_WGuard()
//    {
//        // MQTT 데이터에서 초음파 센서 값을 가져와서 파싱하여 저장
//        WGuard_SensorData_Level = EventManager.Instance.Sensor_Data.AD3_RCV_WGuard_Wave;
//        Debug.Log(WGuard_SensorData_Level);

//        if (WGuard_SensorData_Level == 0)
//        {
//            // 최초 오브젝트 위치 값 0일 때는 안보여야 됌
//            WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
//        }
//        // 차수벽 최소값 5 최대 값 8

//        //// 수위 40 이상이면 더이상 표현 할 필요가 없음 최대값인 532650.9f로 고정
//        else if (WGuard_SensorData_Level > 8.5)
//        {
//            // 새로운 위치 값을 적용하여 수위 오브젝트의 위치 조정
//            WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
//        }
//        else if (WGuard_SensorData_Level < 5.5)
//        {
//            // 새로운 위치 값을 적용하여 수위 오브젝트의 위치 조정
//            WGuard.transform.localPosition = new Vector3(-410129f, 1035739f, -5011084f);
//        }
//        // 0 ~ 40 사이의 수위 값만 보여주면 된다.
//        else
//        {
//			// 기본 위치
//			// WGuard_Cube : -410129 532650.9 -5011084 로컬 좌표계
//			// -410129f 532650.9f -5011084f
//			// -410129f 1035739f -5011084f
//			// 센서 값을 정규화하여 높이 변화에 사용
//			float normalizedSensorValue = (WGuard_SensorData_Level - 5.5f) / (8.5f - 5.5f);

//			// 수위 오브젝트의 Y 위치를 0에서 285.0f까지 변화시킴
//			float newYPosition_1 = Mathf.Lerp(532650.9f, 1035739f, normalizedSensorValue);

//            // 새로운 위치 값을 생성하여 수위 오브젝트의 Y 위치를 조정
//            Vector3 newPosition_1 = new Vector3(-410129f, newYPosition_1, -5011084f);

//            // 새로운 위치 값을 적용하여 수위 오브젝트의 위치 조정
//            WGuard.transform.localPosition = newPosition_1;
//		}
//    }
//}


public class WGuard_Controller : MonoBehaviour
{
	private string WGuard_Cube = "WGuard_Cube";
	private GameObject WGuard;
	private float updateInterval = 0.1f;
	private float WGuard_SensorData_Level;

	private float smoothedSensorValue = 0f;

	private List<float> sensorDataList = new List<float>(); // 리스트로 변경
	private int filterWindowSize = 5;

	private float MIN_VALUE = 8.5f;
	private float MAX_VALUE = 5.5f;

	void Start()
	{
		WGuard = GameObject.Find(WGuard_Cube);

		if (WGuard != null)
		{
			Debug.Log("차수벽 오브젝트 찾기 완료");
			InvokeRepeating("Read_WGuard", 0f, updateInterval);
		}
	}

	private void Read_WGuard()
	{
		WGuard_SensorData_Level = EventManager.Instance.Sensor_Data.AD3_RCV_WGuard_Wave;

		// 새로운 센서 데이터를 리스트에 추가
		sensorDataList.Add(WGuard_SensorData_Level);

		// 리스트의 크기가 윈도우 크기보다 크면 처음 데이터 제거
		if (sensorDataList.Count > filterWindowSize)
		{
			sensorDataList.RemoveAt(0);
		}

		// 리스트 내 데이터의 평균 계산
		smoothedSensorValue = CalculateMovingAverage();

		if (smoothedSensorValue == 0)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
		}
		else if (smoothedSensorValue > MIN_VALUE)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
		}
		else if (smoothedSensorValue < MAX_VALUE)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 1035739f, -5011084f);
		}
		else
		{
			float normalizedSensorValue = (smoothedSensorValue - MAX_VALUE) / (MIN_VALUE - MAX_VALUE);
			float newYPosition_1 = Mathf.Lerp(1035739f, 532650.9f, normalizedSensorValue);
			Vector3 newPosition_1 = new Vector3(-410129f, newYPosition_1, -5011084f);
			WGuard.transform.localPosition = newPosition_1;
		}
	}

	private float CalculateMovingAverage()
	{
		// 리스트 내 데이터의 평균 계산
		float sum = 0f;
		foreach (float value in sensorDataList)
		{
			sum += value;
		}
		return sum / sensorDataList.Count;
	}
}

