using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IS_WL_Controller : MonoBehaviour
{
    private string Inside_Water_Level_1 = "Inside_Water_Level_1"; // 찾을 수위 오브젝트 이름
    private string Inside_Water_Level_2 = "Inside_Water_Level_2"; // 찾을 수위 오브젝트 이름
    private string Inside_Water_Level_3 = "Inside_Water_Level_3"; // 찾을 수위 오브젝트 이름

    private GameObject Water_Level_1; // 수위 오브젝트 참조 변수
    private GameObject Water_Level_2; // 수위 오브젝트 참조 변수
    private GameObject Water_Level_3; // 수위 오브젝트 참조 변수

    private float updateInterval = 1.0f; // 업데이트 주기 (5분)

    private float SensorData_Water_Level; // 센서로부터 읽은 수위 데이터

	// 프로그램이 시작 할 때랑 시작 했을 때 좌표 값 기준이 다르기 때문에 주의 해야된다.
	// 수위 오브젝트 최초 위치 값 Y = 279.62f : 물이 인식 되었을 때 오브젝트 시작 위치 = 282.0f : 마지노선 Y = 285.0f
	// 기본 위치
	// Inside_Water_Level_1 : -109.3485 -42223 -2494964 로컬 좌표계
	// 2108.73, 278.18, 939.01
	// 2108.73, 278.18, 939.01
	// Inside_Water_Level_2 : -2417370 -42223 -3641050 로컬 좌표계
	// 1997.91, 278.18, 1172.76
	// Inside_Water_Level_3 : 0.8125 -42223 -4972127 로컬 좌표계
	// 1869.19, 278.18, 939.00

	void Start()
    {
        // "Inside_Water_Level_1" 이름의 오브젝트를 찾아서 변수에 대입
        Water_Level_1 = GameObject.Find(Inside_Water_Level_1);
        // "Inside_Water_Level_2" 이름의 오브젝트를 찾아서 변수에 대입
        Water_Level_2 = GameObject.Find(Inside_Water_Level_2);
        // "Inside_Water_Level_3" 이름의 오브젝트를 찾아서 변수에 대입
        Water_Level_3 = GameObject.Find(Inside_Water_Level_3);

        //Debug.Log(Water_Level_1.transform.position);
        //Debug.Log(Water_Level_2.transform.position);
        //Debug.Log(Water_Level_3.transform.position);

        // 모든 수위 오브젝트가 찾아졌는지 확인
        if (Water_Level_1 != null && Water_Level_2 != null && Water_Level_3 != null)
        {
            Debug.Log("오브젝트 찾기 완료");

            // 주기적으로 센서 값을 읽어오는 함수를 호출
            // 5분 Read_IS_WL 함수 실행
            InvokeRepeating("Read_IS_WL", 0f, updateInterval);
        }
    }

    private void Read_IS_WL()
    {

        // Debug.Log(Water_Level_1.transform.position);
        // Debug.Log(Water_Level_2.transform.position);
        // Debug.Log(Water_Level_3.transform.position);

        // API 데이터에서 아날로그 접촉식 수위 센서 값을 가져와서 파싱하여 저장
        SensorData_Water_Level = EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT;

        if (SensorData_Water_Level == 0)
        {
            // 최초 오브젝트 위치 값 0일 때는 안보여야 됌
            Water_Level_1.transform.localPosition = new Vector3(-109.3485f, - 42223f, - 2494964f);
            Water_Level_2.transform.localPosition = new Vector3(-2417370f, - 42223f, - 3641050f);
            Water_Level_3.transform.localPosition = new Vector3(0.8125f, - 42223f, - 4972127f);
        }
        // 수위 40 이상이면 더이상 표현 할 필요가 없음 최대값인 285.0f로 고정
        else if (SensorData_Water_Level > 40) 
        {
            // 새로운 위치 값을 적용하여 수위 오브젝트의 위치 조정
            Water_Level_1.transform.localPosition = new Vector3(-109.3485f, 70000, -2494964f);
            Water_Level_2.transform.localPosition = new Vector3(-2417370f, 70000, -3641050f);
            Water_Level_3.transform.localPosition = new Vector3(0.8125f, 70000, -4972127f);
        }
        // 0 ~ 40 사이의 수위 값만 보여주면 된다.
        else
        {
            // 센서 값을 정규화하여 높이 변화에 사용
            float normalizedSensorValue = SensorData_Water_Level / 40.0f;

            // 수위 오브젝트의 Y 위치를 0에서 285.0f까지 변화시킴
            float newYPosition_1 = Mathf.Lerp(0, 70000, normalizedSensorValue);
            float newYPosition_2 = Mathf.Lerp(0, 70000, normalizedSensorValue);
            float newYPosition_3 = Mathf.Lerp(0, 70000, normalizedSensorValue);

            // 새로운 위치 값을 생성하여 수위 오브젝트의 Y 위치를 조정
            Vector3 newPosition_1 = new Vector3(-109.3485f, newYPosition_1, -2494964f);
            Vector3 newPosition_2 = new Vector3(-2417370f, newYPosition_2, -3641050f);
            Vector3 newPosition_3 = new Vector3(0.8125f, newYPosition_3, -4972127f);

            // 새로운 위치 값을 적용하여 수위 오브젝트의 위치 조정
            Water_Level_1.transform.localPosition = newPosition_1;
            Water_Level_2.transform.localPosition = newPosition_2;
            Water_Level_3.transform.localPosition = newPosition_3;
        }
    }
}
