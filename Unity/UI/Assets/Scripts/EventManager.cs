using UnityEngine;
using System;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // 센서 데이터 클래스의 인스터스를 멤버 변수로 선언
    public SensorData Sensor_Data;
    private float pre_WGuard_Wave; // 초음파 센서 cm 이전값 저장
    private int Wave_Count = 0; // 값 중복 횟수 저장
                                // 패널 생성 요청 이벤트를 정의
    public event Action<MapPinInfo> OnPanelCreationRequested;
    public event Action<MapPinInfo> OnPanelDestructionResponsed;

	public InputField inputField;
	public Button Btn_Submit;
	public Button Btn_Cancel;

    private string nfc_temp = "";
	public void Start()
	{
		//// Submit 버튼에 함수 연결
		//Btn_Submit.onClick.AddListener(Click_Submit_Btn);
		//// Cancel 버튼에 함수 연결
		//Btn_Cancel.onClick.AddListener(Click_Cancel_Btn);
	}

	private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("EventManager.Instance 생성");
            Instance = this;
            Sensor_Data = new SensorData();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // 이 객체가 씬 전환 시 파괴되지 않도록 설정
    }

    // 패널 생성 요청 함수
    public void RequestPanelCreation(MapPinInfo mapPinInfo)
    {
        // 패널 생성 요청 이벤트를 발생시킴
        Debug.Log("패널 생성 요청 이벤트를 발생시킴");
        OnPanelCreationRequested?.Invoke(mapPinInfo);

    }
    // 패널 파괴 응답 함수
    public void ResponsePanelDestruction(MapPinInfo mapPinInfo)
    {
        // 패널 파괴 반응 이벤트를 발생시킴
        Debug.Log("패널 파괴 반응 이벤트를 발생시킴");
        OnPanelDestructionResponsed?.Invoke(mapPinInfo);
    }

    // 센서 데이터 초기화
    public void SetSensorData(string message)
    {
        // 기존 NFC값 임시 저장
        nfc_temp = Sensor_Data.AD4_RCV_NFC;

		// mqtt로 받은 message(json형식)을 Sensor_Data 객체로.
		JsonUtility.FromJsonOverwrite(message, Sensor_Data);

		// 소수점 자리 수정 (온도값, 습도값)
        float floatValue = float.Parse(Sensor_Data.AD1_RCV_Temperature);
		Sensor_Data.AD1_RCV_Temperature = floatValue.ToString("F1"); // "F1"은 소수점 한자리까지 출력하는 형식 지정자
        floatValue = float.Parse(Sensor_Data.AD1_RCV_Humidity);
		Sensor_Data.AD1_RCV_Humidity = floatValue.ToString("F1"); // "F1"은 소수점 한자리까지 출력하는 형식 지정자

        // 차단막 상태 변수 set
		if (Sensor_Data.AD3_RCV_WGuard_Wave == 0.0f)    // 초기 상태는 하강상태
        {
            Sensor_Data.AD3_State_WGuard = "하강 상태";
		}
        else if (Sensor_Data.AD3_RCV_WGuard_Wave != pre_WGuard_Wave)    // 현재값이랑 이전값이 다르면 동작중
        {
            Sensor_Data.AD3_State_WGuard = "동작 중";
            Wave_Count = 0;
        }
        else    // 현재값과 이전값이 같으면 동작 끝! //정지 상태
        {
            Wave_Count += 1;
            if (Sensor_Data.AD3_RCV_WGuard_Wave < 6.0f && Wave_Count >= 4)
            {
                Sensor_Data.AD3_State_WGuard = "상승 상태";
            }
            else if (Wave_Count >= 5)
            {
                Sensor_Data.AD3_State_WGuard = "하강 상태";
            }
            else
            {
                Sensor_Data.AD3_State_WGuard = "동작 중";
            }

        }
        //      else if(Sensor_Data.AD3_RCV_WGuard_Wave <= 5.5f )
        //      {
        //          Sensor_Data.AD3_State_WGuard = "상승 상태";
        //}
        //      else if(Sensor_Data.AD3_RCV_WGuard_Wave >= 8.5f && Sensor_Data.AD3_RCV_WGuard_Wave <= 13.0f)
        //      {
        //	Sensor_Data.AD3_State_WGuard = "하강 상태";
        //}
        //      else
        //      {
        //	Sensor_Data.AD3_State_WGuard = "동작 중";
        //}

        pre_WGuard_Wave = Sensor_Data.AD3_RCV_WGuard_Wave;  // 이전 값 저장



        // NFC 데이터 수정
        if (Sensor_Data.AD4_RCV_NFC == "None" || Sensor_Data.AD4_RCV_NFC == "")
		{
			Sensor_Data.AD4_RCV_NFC = nfc_temp;
		}

	}

	//void Click_Submit_Btn()
	//{
	//	Instance.Sensor_Data.AD4_RCV_WL_CNNT = inputField.text;
	//}
	//void Click_Cancel_Btn()
	//{
 //       // 원래 값으로 되돌리기
	//}
}
