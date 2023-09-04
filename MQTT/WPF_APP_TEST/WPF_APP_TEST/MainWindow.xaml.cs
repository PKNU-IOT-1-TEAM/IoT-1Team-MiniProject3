using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WPF_APP_TEST
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : MetroWindow
    {
		private MySqlConnection connection; // DB 연결위해 필요함
        private MqttClient mqttClient;
        private string brokerAddress = "127.0.0.1";      // MQTT BROKER IP
        private int brokerPort = 1883;
        private string p_topic = "TEAM_ONE/parking/Control_data/";            // 발행할 토픽
        private string s_topic = "TEAM_ONE/parking/Sensor_data/";            // 구독할 토픽
		
        public MainWindow()
        {
            InitializeComponent();
			string connectionString = "Server=210.119.12.100;" +
									  "Port=10000;" +
									  "Database=team1_iot;" +
									  "Uid=pi;" +
									  "Pwd=12345;";
			connection = new MySqlConnection(connectionString);
			
			mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
			mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived; // 이벤트핸들러 등록
			mqttClient.Connect(Guid.NewGuid().ToString());
			mqttClient.Subscribe(new string[] { s_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		}

		private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // MQTT message received event handler
            string message = Encoding.UTF8.GetString(e.Message);
            // Parse received JSON data and perform desired processing
            var data = JsonConvert.DeserializeObject<dynamic>(message);

            // Extract individual values from JSON data
            int? AD1_RCV_IR_Sensor = data.AD1_RCV_IR_Sensor;
            int? AD1_RCV_Temperature = data.AD1_RCV_Temperature;
            int? AD1_RCV_Humidity = data.AD1_RCV_Humidity;
            int? AD1_RCV_Dust = data.AD1_RCV_Dust;
			int? AD1_RCV_Parking_Status = data.AD1_RCV_Parking_status;
			int? AD2_RCV_CGuard = data.AD2_RCV_CGuard;
            int? AD2_RCV_IR_Sensor = data.AD2_RCV_IR_Sensor;
            int? AD3_RCV_WGuard_Wave = data.AD3_RCV_WGuard_Wave;
            string AD4_RCV_NFC = data.AD4_RCV_NFC;
            int? AD4_RCV_WL_CNNT = data.AD4_RCV_WL_CNNT;

			// 아두이노 1
			// 1. 주차장 내부 LED
			// 주차장에 차 있고 예약된 자리가 아니면 => 흰색
			// 주차장에 차 없고 예약된 자리이면		 => 주황색
			// 주차장에 차 없고 예약된 자리가 아니면 => 녹색
			// 주차장에 차 있고 예약된 자리이면		 => 빨간색
			if (AD1_RCV_IR_Sensor == 0)
			{
				var Adata = new { AD1_LED_RED = 1 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else if (AD1_RCV_IR_Sensor == 1)
			{
				var Adata = new { AD1_LED_RED = 2 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			/*
			else if(AD1_RCV_IR_Sensor == 0)
			{
				var Adata = new { AD1_LED_RED = 3 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else if(AD1_RCV_IR_Sensor == 1)
			{
				var Adata = new { AD1_LED_RED = 4 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			*/


			// 아두이노 2
			// 2. 주차장 입구 차단봉
			// 카메라로 차가 있는지 없는지 판단,등록된 차량이면(-1아니면1로)


			// 아두이노 3
			// 3. 물 수위 일정 높이 이상이면 워터펌프 동작 / 리니어(차수막) 올라감
			if (AD4_RCV_WL_CNNT > 40)
			{	
				for (int i = 0; i < 2; i++)
				{
					if(i == 0)
					{
						var Adata = new { AD3_WPump = 1 };  // 전송할 데이터 json형식으로 생성
						string json = JsonConvert.SerializeObject(Adata);

						mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
					}
					else if(i == 1)
					{
						var Adata = new { AD3_Linear = 2 };  // 전송할 데이터 json형식으로 생성
						string json = JsonConvert.SerializeObject(Adata);

						mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
					}
					
				}
			}
			else if(AD4_RCV_WL_CNNT < 10)
			{
				for (int i = 0; i < 2; i++)
				{
					if (i == 0)
					{
						var Adata = new { AD3_WPump = 0 };  // 전송할 데이터 json형식으로 생성
						string json = JsonConvert.SerializeObject(Adata);

						mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
					}
					else if (i == 1)
					{
						var Adata = new { AD3_Linear = 3 };  // 전송할 데이터 json형식으로 생성
						string json = JsonConvert.SerializeObject(Adata);

						mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
					}

				}
			}

			// Update UI or perform other logic with the received data
			Dispatcher.Invoke(() =>
            {
                OutputText.Text = $"AD1_RCV_IR Sensor = {AD1_RCV_IR_Sensor}, AD1_RCV_Temperature = {AD1_RCV_Temperature}, AD1_RCV_Humidity = {AD1_RCV_Humidity}, AD1_RCV_Dust = {AD1_RCV_Dust}, AD1_RCV_Parking_Status = {AD1_RCV_Parking_Status}, AD2_RCV_CGuard = {AD2_RCV_CGuard}, AD3_RCV_WGuard_Wave = {AD3_RCV_WGuard_Wave}, AD4_RCV_NFC = {AD4_RCV_NFC}, AD4_RCV_WL_CNNT = {AD4_RCV_WL_CNNT}";
            });
			
			// 데이터베이스 업데이트
			//UpdateDataIntoDatabase(AD1_RCV_IR_Sensor, AD1_RCV_Temperature, AD1_RCV_Humidity, AD1_RCV_Dust, AD1_RCV_Parking_Status, AD2_RCV_CGuard, AD3_RCV_WGuard_Wave, AD4_RCV_NFC, AD4_RCV_WL_CNNT);

		}

		//private void UpdateDataIntoDatabase(int? AD1_RCV_IR_Sensor, int? AD1_RCV_Temperature, int? AD1_RCV_Humidity, int? AD1_RCV_Dust, int? AD1_RCV_Parking_Status, int? AD2_RCV_CGuard, int? AD3_RCV_WGuard_Wave, string AD4_RCV_NFC, int? AD4_RCV_WL_CNNT)
		//{
		//	try
		//	{
		//		// mqtt에서 받은 데이터 MySQL DB에 업데이트 하기
		//		connection.Open();

		//		// 'sensor_db' 테이블에 UPDATE
		//		string query = @"UPDATE sensor_db
  //              SET
  //              AD1_RCV_IR_Sensor = @AD1_RCV_IR_Sensor,
  //              AD1_RCV_Temperature = @AD1_RCV_Temperature,
  //              AD1_RCV_Humidity = @AD1_RCV_Humidity,
  //              AD1_RCV_Dust = @AD1_RCV_Dust,
		//		AD1_RCV_Parking_Status = @AD1_RCV_Parking_Status,
  //              AD2_RCV_CGuard = @AD2_RCV_CGuard,
  //              AD3_RCV_WGuard_WAVE = @AD3_RCV_WGuard_WAVE,
  //              AD4_RCV_NFC = @AD4_RCV_NFC,
  //              AD4_RCV_WL_CNNT = @AD4_RCV_WL_CNNT
  //              WHERE id_x = 0";

		//		MySqlCommand cmd = new MySqlCommand(query, connection);

		//		// 파라미터에 값 할당
		//		cmd.Parameters.AddWithValue("@AD1_RCV_IR_Sensor", AD1_RCV_IR_Sensor);
		//		cmd.Parameters.AddWithValue("@AD1_RCV_Temperature", AD1_RCV_Temperature);
		//		cmd.Parameters.AddWithValue("@AD1_RCV_Humidity", AD1_RCV_Humidity);
		//		cmd.Parameters.AddWithValue("@AD1_RCV_Dust", AD1_RCV_Dust);
		//		cmd.Parameters.AddWithValue("@AD1_RCV_Parking_Status", AD1_RCV_Parking_Status);
		//		cmd.Parameters.AddWithValue("@AD2_RCV_CGuard", AD2_RCV_CGuard);
		//		cmd.Parameters.AddWithValue("@AD3_RCV_WGuard_Wave", AD3_RCV_WGuard_Wave);
		//		cmd.Parameters.AddWithValue("@AD4_RCV_NFC", AD4_RCV_NFC);
		//		cmd.Parameters.AddWithValue("@AD4_RCV_WL_CNNT", AD4_RCV_WL_CNNT);

		//		// 쿼리 실행
		//		int rowsAffected = cmd.ExecuteNonQuery();

		//		// 업데이트 성공 여부 확인
		//		if (rowsAffected > 0)
		//		{
		//			//MessageBox.Show("데이터 업데이트 successfully.");
		//		}
		//		else
		//		{
		//			MessageBox.Show("데이터 업데이트 failed or no matching record found.");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show($"에러 !! {ex.Message}");
		//	}
		//	finally
		//	{
		//		connection.Close();
		//	}
  //      }
		

		private void BtnCGuard_Click(object sender, RoutedEventArgs e)
		{
			if (BtnCGuard.IsChecked == true)
			{
				var data = new { AD2 = 2 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else
			{
				var data = new { AD2 = 3 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
		}

		private void BtnLinear_Click(object sender, RoutedEventArgs e)
		{
			if (BtnLinear.IsChecked == true)
			{
				var data = new { AD3_Linear = 2 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else
			{
				var data = new { AD3_Linear = 3 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
		}

		private void BtnWPump_Click(object sender, RoutedEventArgs e)
		{
			if (BtnWPump.IsChecked == true)
			{
				var data = new { AD3_WPump = 0 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);
				
				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else
			{
				var data = new { AD3_WPump = 1 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(data);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
		}
	}
}
