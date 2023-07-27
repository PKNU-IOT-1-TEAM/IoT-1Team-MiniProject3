using MahApps.Metro.Controls;
using System;
using System.IO.Packaging;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;

namespace WPF_APP_TEST
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MqttClient mqttClient;
        private string brokerAddress = "127.0.0.1";      // MQTT BROKER IP
        private int brokerPort = 1883;
        private string p_topic = "TEAM_ONE/parking/Control_data/";            // 발행할 토픽
        private string s_topic = "TEAM_ONE/parking/Sensor_data/";            // 구독할 토픽

        public MainWindow()
        {
            InitializeComponent();
			mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
			mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
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
			// 카메라로 차가 있는지 없는지 판단,등록된 차량이면(0아니면1로)


			// 아두이노 3
			// 3. 물 수위 일정 높이 이하면 워터펌프 멈춤(0)
            

			// Update UI or perform other logic with the received data
			Dispatcher.Invoke(() =>
            {
                OutputText.Text = $"AD1_RCV_IR Sensor = {AD1_RCV_IR_Sensor}, AD1_RCV_Temperature = {AD1_RCV_Temperature}, AD1_RCV_Humidity = {AD1_RCV_Humidity}, AD1_RCV_Dust = {AD1_RCV_Dust}, AD2_RCV_CGuard = {AD2_RCV_CGuard}, AD3_RCV_WGuard_Wave = {AD3_RCV_WGuard_Wave}, AD4_RCV_NFC = {AD4_RCV_NFC}, AD4_RCV_WL_CNNT = {AD4_RCV_WL_CNNT}";
            });
        }

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
