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
	// mqtt broker ����
	private MqttClient mqttClient;
	private string brokerAddress = "210.119.12.112"; // "192.168.0.25"; // MQTT BROKER IP
	private int brokerPort = 11000; // 1883;
	private string p_topic = "TEAM_ONE/parking/Control_data/"; // ������ ����
	//private string c_topic = "TEAM_ONE/parking/Car_Number_data/"; // ������ ����(���������2)

	private bool emergency = false;	// ���� 40�̻� ħ�� �߻� ��� ��Ȳ

	// �����ͺ��̽� ���ῡ �ʿ��� ������ �����մϴ�.
	private string db_Address = "210.119.12.112";  // �����ͺ��̽� ���� IP �ּ�
	private string db_Port = "10000";        // �����ͺ��̽� ��Ʈ ��ȣ
	private string db_Id = "pi";          // �����ͺ��̽� ���� ID
	private string db_Pw = "12345";         // �����ͺ��̽� ���� ��й�ȣ
	private string db_Name = "team1_iot";    // ����� �����ͺ��̽� �̸�
	// �����ͺ��̽� ���� ���ڿ��� ����
	private string conn_string;
	private MySqlConnection con = null;

	private GameObject cGuard;

	private float updateInterval = 1f;// ������Ʈ �ֱ� (1��)

	public InputField inputField;
	public Button Btn_Submit;
	public Button Btn_Cancel;
	private bool User_Make_Water_Level = false; // ������ ���� ��ư�� Ŭ���ؼ� ������ �����ߴ��� üũ�ϴ� ����
	private bool Check_One_Time = false;

	private string targetObjectName = "�����_������_0729_OPCTY"; // �����Ϸ��� ������Ʈ�� �̸�
	private string targetScriptName = "IS_WL_Controller"; // �����Ϸ��� ��ũ��Ʈ�� �̸�

	void Start()
	{
		// DB���� ���ڿ�
		conn_string = "Server=" + db_Address + ";Port=" + db_Port + ";Database=" + db_Name + ";User=" + db_Id + ";Password=" + db_Pw;
		// MQTT Client ����, ����
		mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
		mqttClient.Connect(Guid.NewGuid().ToString());
		//mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
		//mqttClient.Subscribe(new string[] { c_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

		cGuard = SceneManager.GetSceneByName("Inside_Scene").GetRootGameObjects()[0];
		cGuard.transform.rotation = Quaternion.Euler(0, 0, 90);

		Btn_Submit.onClick.AddListener(Submit_Water_Level);
		Btn_Cancel.onClick.AddListener(Cancel_Water_Level);

		InvokeRepeating("carNum", 0f, updateInterval); // ���� ���� �� 1�� ���� UpdateRiverFlow �Լ��� ȣ��
	}

	// ���ø����̼��� ����� �� DB, MQTT ������ �ݽ��ϴ�.
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
// MQTT ���� TEAM_ONE/parking/Control_data/
//	private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
//	{
//		// MQTT �޽��� ���� �̺�Ʈ ó��
//		string message = Encoding.UTF8.GetString(e.Message);
//		//JSON ������ �Ľ�
//		var data = JsonConvert.DeserializeObject<SensorData>(message);
//		// message �����ؼ� SensorData ������.
//		EventManager.Instance.SetSensorData(message);

//		//�����͸� EventManager�� ����
//		EventManager.Instance.Sensor_Data.IR_CAR_NUM = data.IR_CAR_NUM;
//		EventManager.Instance.Sensor_Data.CAR_NUMBER = data.CAR_NUMBER;
//}

	private void PublishDataToMQTT(string jsonData)
	{
		byte[] payload = Encoding.UTF8.GetBytes(jsonData);
		// Debug.Log($"Ȯ��: {jsonData}");
		mqttClient.Publish(p_topic, payload, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
	}

	// Start is called before the first frame update
	// Update is called once per frame

	private bool hasSentData = false; // ���� ���� �߰�
	void Update()
	{
		if (User_Make_Water_Level) // ����ڰ� ������ ���Ƿ� ���� �ߴٸ� ���� �ٲ���� �ݺ� ������ ����
		{
			EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = int.Parse(inputField.text);
		}

		//Debug.Log("������Ʈ ��");
		// ħ�� ��Ȳ [������ ������ġ �̻��̸� ������/��������/���ܺ� ��Ʈ��]
		// ħ�� �߻� O
		if (((EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT > 40) || (User_Make_Water_Level == true)) && emergency == false)
		{
			Overflow_Situation();
		}
		else if (Check_One_Time == true && EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT > 40)
		{

		}
		// ħ����Ȳ ����
		else if (((EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT < 10) || (User_Make_Water_Level == false)) && (emergency == true))
		{
			End_Overflow_Situation();
		}
	}
	// ħ�� �� ��Ȳ ����
	public void Overflow_Situation()
	{
		if (Check_One_Time == false) // ���� ���� �Ȱ� �ƴ϶��
		{
			// ħ����Ȳ true
			emergency = true;
			//Debug.Log("ħ�� �ƴ�.");
			//// �������� ON 2
			//if (EventManager.Instance.Sensor_Data.AD3_State_WPump != "ON")
			//{
			var data3 = new { AD3_WPump = 2 };
			string json3 = JsonConvert.SerializeObject(data3);
			PublishDataToMQTT(json3);
			Debug.Log(json3);
			EventManager.Instance.Sensor_Data.AD3_State_WPump = "ON";
			Thread.Sleep(2000);
			//}
			//// ������ ��� 3
			//if(EventManager.Instance.Sensor_Data.AD3_State_WGuard != "��� ����")
			//{
			var data2 = new { AD3_WGuard = 3 };
			string json2 = JsonConvert.SerializeObject(data2);
			PublishDataToMQTT(json2);
			Debug.Log(json2);
			//EventManager.Instance.Sensor_Data.AD3_State_WGuard = "Danger";
			//}
			// ���ܺ� ����
			//if(EventManager.Instance.Sensor_Data.AD2_State_CGuard != "����")
			//{
			var data1 = new { AD2_CGuard = 3 };      // close
			string json1 = JsonConvert.SerializeObject(data1);
			PublishDataToMQTT(json1);
			Debug.Log(json1);
			EventManager.Instance.Sensor_Data.AD2_State_CGuard = "����";
			//}
		}
		Check_One_Time = true;
	}
	// ħ�� ���� �� ��Ȳ ����
	public void End_Overflow_Situation()
	{
		// ������ �ϰ� 4
		string json = "{\"AD3_WGuard\":4}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		Thread.Sleep(2000);

		// �������� OFF 1
		json = "{\"AD3_WPump\":1}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD3_State_WPump = "OFF";

		// ħ�� ������ ����
		emergency = false;
		Check_One_Time = false;
	}

	// ���� �ν� ���� ��
	private void carNum()
	{
		#region <����ȣ�ν�>
		if (EventManager.Instance.Sensor_Data.IR_CAR_NUM == "0")        // IR���� �ν� �Ǹ�
		{
			string CARNUM = EventManager.Instance.Sensor_Data.CAR_NUMBER;
			try
			{
				// �����ͺ��̽� ������ �õ�
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
				// DB�� Car_NUM�� ��
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
							Debug.Log("���O");
							var data = new { AD2_CGuard = 2 };      // open
							string json = JsonConvert.SerializeObject(data);
							PublishDataToMQTT(json);
							Debug.Log(json);
							EventManager.Instance.Sensor_Data.AD2_State_CGuard = "ON";
							hasSentData = false;
						}
						else
						{
							Debug.Log("���X");
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

	// ��ư Ŭ�� �� ħ�� ���� ������ִ� �Լ�
	public void Submit_Water_Level()
	{
		GameObject targetObject = GameObject.Find(targetObjectName);
		MonoBehaviour targetScript = targetObject.GetComponent(targetScriptName) as MonoBehaviour;
		Destroy(targetScript);

		string Inside_Water_Level_1 = "Inside_Water_Level_1"; // ã�� ���� ������Ʈ �̸�
		string Inside_Water_Level_2 = "Inside_Water_Level_2"; // ã�� ���� ������Ʈ �̸�
		string Inside_Water_Level_3 = "Inside_Water_Level_3"; // ã�� ���� ������Ʈ �̸�

		GameObject Water_Level_1; // ���� ������Ʈ ���� ����
		GameObject Water_Level_2; // ���� ������Ʈ ���� ����
		GameObject Water_Level_3; // ���� ������Ʈ ���� ����

		// "Inside_Water_Level_1" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
		Water_Level_1 = GameObject.Find(Inside_Water_Level_1);
		// "Inside_Water_Level_2" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
		Water_Level_2 = GameObject.Find(Inside_Water_Level_2);
		// "Inside_Water_Level_3" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
		Water_Level_3 = GameObject.Find(Inside_Water_Level_3);

		EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = int.Parse(inputField.text);

		if (int.Parse(inputField.text) >= 40)
		{
			User_Make_Water_Level = true;
		}

		if (int.Parse(inputField.text) == 0)
		{
			// ���� ������Ʈ ��ġ �� 0�� ���� �Ⱥ����� ��
			Water_Level_1.transform.localPosition = new Vector3(-109.3485f, -42223f, -2494964f);
			Water_Level_2.transform.localPosition = new Vector3(-2417370f, -42223f, -3641050f);
			Water_Level_3.transform.localPosition = new Vector3(0.8125f, -42223f, -4972127f);
		}
		// ���� 40 �̻��̸� ���̻� ǥ�� �� �ʿ䰡 ���� �ִ밪�� 285.0f�� ����
		else if (int.Parse(inputField.text) > 40)
		{
			// ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
			Water_Level_1.transform.localPosition = new Vector3(-109.3485f, 70000, -2494964f);
			Water_Level_2.transform.localPosition = new Vector3(-2417370f, 70000, -3641050f);
			Water_Level_3.transform.localPosition = new Vector3(0.8125f, 70000, -4972127f);
		}
		// 0 ~ 40 ������ ���� ���� �����ָ� �ȴ�.
		else
		{
			// ���� ���� ����ȭ�Ͽ� ���� ��ȭ�� ���
			float normalizedSensorValue = int.Parse(inputField.text) / 40.0f;

			// ���� ������Ʈ�� Y ��ġ�� 0���� 285.0f���� ��ȭ��Ŵ
			float newYPosition_1 = Mathf.Lerp(0, 70000, normalizedSensorValue);
			float newYPosition_2 = Mathf.Lerp(0, 70000, normalizedSensorValue);
			float newYPosition_3 = Mathf.Lerp(0, 70000, normalizedSensorValue);

			// ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� Y ��ġ�� ����
			Vector3 newPosition_1 = new Vector3(-109.3485f, newYPosition_1, -2494964f);
			Vector3 newPosition_2 = new Vector3(-2417370f, newYPosition_2, -3641050f);
			Vector3 newPosition_3 = new Vector3(0.8125f, newYPosition_3, -4972127f);

			// ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
			Water_Level_1.transform.localPosition = newPosition_1;
			Water_Level_2.transform.localPosition = newPosition_2;
			Water_Level_3.transform.localPosition = newPosition_3;
		}
	}
	// ��ư Ŭ�� �� ħ�� ���� �������ִ� �Լ�
	public void Cancel_Water_Level()
	{
		GameObject targetObject = GameObject.Find(targetObjectName);
		MonoBehaviour targetScript = targetObject.GetComponent(targetScriptName) as MonoBehaviour;
		EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT = 0;
		targetObject.AddComponent(System.Type.GetType(targetScriptName));

		User_Make_Water_Level = false;
	}

	// mqtt�� �Ƶ��̳�1�� ���� ������ 
	// �� ��ȣ �ν��ؼ� ���ܺ�

	// ���� on��ư Ŭ����
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
	// ������ ��� 3
	public void On_AD3_WGuard()
	{
		// �̹� ��� �����̸� �н�
		if (EventManager.Instance.Sensor_Data.AD3_State_WGuard == "��� ����") return;
		// ��� ��� ����
		string json = "{\"AD3_WGuard\":3}";
		PublishDataToMQTT(json);
		Debug.Log(json);
	}
	// �������� ON 2
	public void On_AD3_WPump()
	{
		if (EventManager.Instance.Sensor_Data.AD3_State_WPump == "ON") return;
		string json = "{\"AD3_WPump\":2}";
		PublishDataToMQTT(json);
		Debug.Log(json);
		EventManager.Instance.Sensor_Data.AD3_State_WPump = "ON";
	}
	#endregion

	// ���� off��ư Ŭ����
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

	// ������ �ϰ� 4
	public void Off_AD3_WGaurd()
	{
		// �̹� �ϰ� �����̸� �н�
		if (EventManager.Instance.Sensor_Data.AD3_State_WGuard == "�ϰ� ����") return;
		// �ϰ� ��� ����
		string json = "{\"AD3_WGuard\":4}";
		PublishDataToMQTT(json);
		Debug.Log(json);
	}

	// �������� OFF 1
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
