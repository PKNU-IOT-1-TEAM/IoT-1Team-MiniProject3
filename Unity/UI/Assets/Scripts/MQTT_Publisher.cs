using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MQTT_Publisher : MonoBehaviour
{
	// mqtt broker 연결
	private MqttClient mqttClient;
	private string brokerAddress = "210.119.12.112"; // "192.168.0.25"; // MQTT BROKER IP
	private int brokerPort = 11000; // 1883;
	private string p_topic = "TEAM_ONE/parking/Control_data/"; // 발행할 토픽
	//private string c_topic = "TEAM_ONE/parking/Car_Number_data/"; // 구독할 토픽(라즈베리파이2)

	private bool emergency = false;	// 수위 40이상 침수 발생 비상 상황

	// 데이터베이스 연결에 필요한 정보를 설정합니다.
	private string db_Address = "210.119.12.112";  // 데이터베이스 서버 IP 주소
	private string db_Port = "10000";        // 데이터베이스 포트 번호
	private string db_Id = "pi";          // 데이터베이스 접속 ID
	private string db_Pw = "12345";         // 데이터베이스 접속 비밀번호
	private string db_Name = "team1_iot";    // 사용할 데이터베이스 이름
	// 데이터베이스 연결 문자열을 설정
	private string conn_string;
	private MySqlConnection con = null;

	private GameObject cGuard;

	private float updateInterval = 1f;// 업데이트 주기 (1초)

	public InputField inputField;
	public Button Btn_Submit;
	public Button Btn_Cancel;
	private bool User_Make_Water_Level = false; // 유저가 직접 버튼을 클릭해서 수위를 조절했는지 체크하는 변수
	private bool Check_One_Time = false;

	private string targetObjectName = "노블래스_주차장_0729_OPCTY"; // 제어하려는 오브젝트의 이름
	private string targetScriptName = "IS_WL_Controller"; // 제어하려는 스크립트의 이름

	void Start()
	{
		// DB연결 문자열
		conn_string = "Server=" + db_Address + ";Port=" + db_Port + ";Database=" + db_Name + ";User=" + db_Id + ";Password=" + db_Pw;
		// MQTT Client 생성, 연결
		mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
		mqttClient.Connect(Guid.NewGuid().ToString());
		//mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
		//mqttClient.Subscribe(new string[] { c_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

		cGuard = SceneManager.GetSceneByName("Inside_Scene").GetRootGameObjects()[0];
		cGuard.transform.rotation = Quaternion.Euler(0, 0, 90);

		Btn_Submit.onClick.AddListener(Submit_Water_Level);
		Btn_Cancel.onClick.AddListener(Cancel_Water_Level);

		InvokeRepeating("carNum", 0f, updateInterval); // 최초 실행 후 1초 마다 UpdateRiverFlow 함수를 호출
	}

	// 애플리케이션이 종료될 때 DB, MQTT 연결을 닫습니다.
	private void OnApplicationQuit()
	{
		if (con != null)
		{
			if (con.State != ConnectionState.Closed)
			{
				con.Close();
				Debug.Log("Mysql connection closed");
			}
			con.Dispose();
		}

		if (mqttClient != null && mqttClient.IsConnected)
		{
			mqttClient.Disconnect();
			Console.WriteLine("Disconnected from MQTT broker.");
		}
	}
// MQTT 발행 TEAM_ONE/parking/Control_data/
//	private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
//	{
//		// MQTT 메시지 수신 이벤트 처리
//		string message = Encoding.UTF8.GetString(e.Message);
//		//JSON 데이터 파싱
//		var data = JsonConvert.DeserializeObject<SensorData>(message);
//		// message 전달해서 SensorData 설정함.
//		EventManager.Instance.SetSensorData(message);

//		//데이터를 EventManager에 저장
//		EventManager.Instance.Sensor_Data.IR_CAR_NUM = data.IR_CAR_NUM;
//		EventManager.Instance.Sensor_Data.CAR_NUMBER = data.CAR_NUMBER;
//}

	private void PublishDataToMQTT(string jsonData)
	{
		byte[] payload = Encoding.UTF8.GetBytes(jsonData);
		// Debug.Log($"확인: {jsonData}");
		mqttClient.Publish(p_topic, payload, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
	}

	// Start is called before the first frame update
	// Update is called once per frame

	private bool hasSentData = false; // 상태 변수 추가
	void Update()
	{
		if (User_Make_Water_Level) // 사용자가 수위를 임의로 설정 했다면 값을 바꿔줘야 반복 동작을 안함
		{
			EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = int.Parse(inputField.text);
		}

		//Debug.Log("업데이트 중");
		// 침수 상황 [수위가 일정수치 이상이면 차수막/워터펌프/차단봉 컨트롤]
		// 침수 발생 O
		if (((EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT > 40) || (User_Make_Water_Level == true)) && emergency == false)
		{
			Overflow_Situation();
		}
		else if (Check_One_Time == true && EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT > 40)
		{

		}
		// 침수상황 해제
		else if (((EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT < 10) || (User_Make_Water_Level == false)) && (emergency == true))
		{
			End_Overflow_Situation();
		}
	}
	// 침수 시 상황 설정
	public void Overflow_Situation()
	{
		if (Check_One_Time == false) // 아직 실행 된게 아니라면
		{
			// 침수상황 true
			emergency = true;
			//Debug.Log("침수 됐다.");
			//// 워터펌프 ON 2
			//if (EventManager.Instance.Sensor_Data.AD3_State_WPump != "ON")
			//{
			var data3 = new { AD3_WPump = 2 };
			string json3 = JsonConvert.SerializeObject(data3);
			PublishDataToMQTT(json3);
			Debug.Log(json3);
			EventManager.Instance.Sensor_Data.AD3_State_WPump = "ON";
			Thread.Sleep(2000);
			//}
			//// 차수막 상승 3
			//if(EventManager.Instance.Sensor_Data.AD3_State_WGuard != "상승 상태")
			//{
			var data2 = new { AD3_WGuard = 3 };
			string json2 = JsonConvert.SerializeObject(data2);
			PublishDataToMQTT(json2);
			Debug.Log(json2);
			//EventManager.Instance.Sensor_Data.AD3_State_WGuard = "Danger";
			//}
			// 차단봉 닫힘
			//if(EventManager.Instance.Sensor_Data.AD2_State_CGuard != "닫힘")
			//{
			var data1 = new { AD2_CGuard = 3 };      // close
			string json1 = JsonConvert.SerializeObject(data1);
			PublishDataToMQTT(json1);
			Debug.Log(json1);
			EventManager.Instance.Sensor_Data.AD2_State_CGuard = "닫힘";
			//}
		}
		Check_One_Time = true;
	}
	// 침수 종료 시 상황 설정
	public void End_Overflow_Situation()
	{
		// 차수막 하강 4
		string json = "{\"AD3_WGuard\":4}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		Thread.Sleep(2000);

		// 워터펌프 OFF 1
		json = "{\"AD3_WPump\":1}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD3_State_WPump = "OFF";

		// 침수 비상상태 종료
		emergency = false;
		Check_One_Time = false;
	}

	// 차량 인식 됐을 시
	private void carNum()
	{
		#region <차번호인식>
		if (EventManager.Instance.Sensor_Data.IR_CAR_NUM == "0")        // IR센서 인식 되면
		{
			string CARNUM = EventManager.Instance.Sensor_Data.CAR_NUMBER;
			try
			{
				// 데이터베이스 연결을 시도
				con = new MySqlConnection(conn_string);
				con.Open();
				Debug.Log("Mysql state: " + con.State);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}

			try
			{
				// DB의 Car_NUM과 비교
				using (MySqlConnection connection = new MySqlConnection(conn_string))
				{
					connection.Open();

					string query = $"SELECT COUNT(*) FROM account_parking WHERE car_number = @{CARNUM}";
					using (MySqlCommand command = new MySqlCommand(query, connection))
					{
						command.Parameters.AddWithValue($"@{CARNUM}", CARNUM);
						long count = (long)command.ExecuteScalar();

						if (count > 0)
						{
							Debug.Log("등록O");
							var data = new { AD2_CGuard = 2 };      // open
							string json = JsonConvert.SerializeObject(data);
							PublishDataToMQTT(json);
							Debug.Log(json);
							EventManager.Instance.Sensor_Data.AD2_State_CGuard = "ON";
							hasSentData = false;
						}
						else
						{
							Debug.Log("등록X");
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		else if (EventManager.Instance.Sensor_Data.IR_CAR_NUM == "1" && !hasSentData)
		{
			var data = new { AD2_CGuard = 3 };
			string json = JsonConvert.SerializeObject(data);
			PublishDataToMQTT(json);
			Debug.Log(json);
			EventManager.Instance.Sensor_Data.AD2_State_CGuard = "OFF";
			hasSentData = true;
		}
		Thread.Sleep(1000);
		#endregion
	}

	// 버튼 클릭 시 침수 상태 만들어주는 함수
	public void Submit_Water_Level()
	{
		GameObject targetObject = GameObject.Find(targetObjectName);
		MonoBehaviour targetScript = targetObject.GetComponent(targetScriptName) as MonoBehaviour;
		Destroy(targetScript);

		string Inside_Water_Level_1 = "Inside_Water_Level_1"; // 찾을 수위 오브젝트 이름
		string Inside_Water_Level_2 = "Inside_Water_Level_2"; // 찾을 수위 오브젝트 이름
		string Inside_Water_Level_3 = "Inside_Water_Level_3"; // 찾을 수위 오브젝트 이름

		GameObject Water_Level_1; // 수위 오브젝트 참조 변수
		GameObject Water_Level_2; // 수위 오브젝트 참조 변수
		GameObject Water_Level_3; // 수위 오브젝트 참조 변수

		// "Inside_Water_Level_1" 이름의 오브젝트를 찾아서 변수에 대입
		Water_Level_1 = GameObject.Find(Inside_Water_Level_1);
		// "Inside_Water_Level_2" 이름의 오브젝트를 찾아서 변수에 대입
		Water_Level_2 = GameObject.Find(Inside_Water_Level_2);
		// "Inside_Water_Level_3" 이름의 오브젝트를 찾아서 변수에 대입
		Water_Level_3 = GameObject.Find(Inside_Water_Level_3);

		EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = int.Parse(inputField.text);

		if (int.Parse(inputField.text) >= 40)
		{
			User_Make_Water_Level = true;
		}

		if (int.Parse(inputField.text) == 0)
		{
			// 최초 오브젝트 위치 값 0일 때는 안보여야 됌
			Water_Level_1.transform.localPosition = new Vector3(-109.3485f, -42223f, -2494964f);
			Water_Level_2.transform.localPosition = new Vector3(-2417370f, -42223f, -3641050f);
			Water_Level_3.transform.localPosition = new Vector3(0.8125f, -42223f, -4972127f);
		}
		// 수위 40 이상이면 더이상 표현 할 필요가 없음 최대값인 285.0f로 고정
		else if (int.Parse(inputField.text) > 40)
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
			float normalizedSensorValue = int.Parse(inputField.text) / 40.0f;

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
	// 버튼 클릭 시 침수 상태 해제해주는 함수
	public void Cancel_Water_Level()
	{
		GameObject targetObject = GameObject.Find(targetObjectName);
		MonoBehaviour targetScript = targetObject.GetComponent(targetScriptName) as MonoBehaviour;
		EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = 0;
		targetObject.AddComponent(System.Type.GetType(targetScriptName));

		User_Make_Water_Level = false;
	}

	// mqtt로 아두이노1에 보낼 데이터 
	// 차 번호 인식해서 차단봉

	// 제어 on버튼 클릭시
	#region
	public void On_AD2_CGuard()
	{
		var data = new { AD2_CGuard = 2 };      // open
		string json = JsonConvert.SerializeObject(data);
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD2_State_CGuard = "ON";

		//cGuard.transform.rotation = Quaternion.Euler(0, 90, 180);
	}
	// 차수막 상승 3
	public void On_AD3_WGuard()
	{
		// 이미 상승 상패이면 패스
		if (EventManager.Instance.Sensor_Data.AD3_State_WGuard == "상승 상태") return;
		// 상승 명령 전달
		string json = "{\"AD3_WGuard\":3}";
		PublishDataToMQTT(json);
		Debug.Log(json);
	}
	// 워터펌프 ON 2
	public void On_AD3_WPump()
	{
		if (EventManager.Instance.Sensor_Data.AD3_State_WPump == "ON") return;
		string json = "{\"AD3_WPump\":2}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD3_State_WPump = "ON";
	}
	#endregion

	// 제어 off버튼 클릭시
	#region 
	public void Off_AD2_CGuard()
	{
		var data = new { AD2_CGuard = 3 };
		string json = JsonConvert.SerializeObject(data);
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD2_State_CGuard = "OFF";

		//cGuard.transform.rotation = Quaternion.Euler(-90, 90, 180);
	}

	// 차수막 하강 4
	public void Off_AD3_WGaurd()
	{
		// 이미 하강 상패이면 패스
		if (EventManager.Instance.Sensor_Data.AD3_State_WGuard == "하강 상태") return;
		// 하강 명령 전달
		string json = "{\"AD3_WGuard\":4}";
		PublishDataToMQTT(json);
		Debug.Log(json);
	}

	// 워터펌프 OFF 1
	public void Off_AD3_WPump()
	{
		//if (EventManager.Instance.Sensor_Data.AD3_State_WPump == "OFF") return;

		string json = "{\"AD3_WPump\":1}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD3_State_WPump = "OFF";
	}
	#endregion
}
