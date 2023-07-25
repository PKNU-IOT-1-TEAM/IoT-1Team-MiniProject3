using MahApps.Metro.Controls;
using System;
using System.IO.Packaging;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Markup;

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
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            //if (mqttClient == null)
            //{
            //mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
            //mqttClient.Connect(Guid.NewGuid().ToString());
            //}

            var data = new { AD1 = 0, AD2 = 0, AD3 = 1, AD4 = 0};  // 전송할 데이터 json형식으로 생성
            string json = JsonConvert.SerializeObject(data);
            
            mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {

            //if (mqttClient == null)
            //{
            //    mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
            //    mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            //    mqttClient.Connect(Guid.NewGuid().ToString());
            //    mqttClient.Subscribe(new string[] { s_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            //}
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
            int? AD3_RCV_WGuard_Wave = data.AD3_RCV_WGuard_Wave;
            string AD4_RCV_NFC = data.AD4_RCV_NFC;
            int? AD4_RCV_WL_CNNT = data.AD4_RCV_WL_CNNT;
            int? AD4_RCV_WL_NCNNT = data.AD4_RCV_WL_NCNNT;

            if (AD1_RCV_IR_Sensor != 1)
            {
				var Adata2 = new { AD2 = 1 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata2);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
            else
            {
				var Adata2 = new { AD2 = -1 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata2);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}



            if (AD4_RCV_WL_CNNT < 400)
            {
                var Adata3 = new { AD3_CNNT = 0 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata3);

                mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
            }
            else if (AD4_RCV_WL_CNNT >= 400)
            {
				var Adata3 = new { AD3_CNNT = 1 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata3);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
            
            if (AD4_RCV_WL_NCNNT == 1)
            {
				var Adata3 = new { AD3_NCNNT = 2 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata3);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}
			else if (AD4_RCV_WL_NCNNT == 0)
			{
				var Adata3 = new { AD3_NCNNT = 3 };  // 전송할 데이터 json형식으로 생성
				string json = JsonConvert.SerializeObject(Adata3);

				mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
			}

			// Update UI or perform other logic with the received data
			Dispatcher.Invoke(() =>
            {
                OutputText.Text = $"AD1_RCV_IR Sensor = {AD1_RCV_IR_Sensor}, AD1_RCV_Temperature = {AD1_RCV_Temperature}, AD1_RCV_Humidity = {AD1_RCV_Humidity}, AD1_RCV_Dust = {AD1_RCV_Dust}, AD2_RCV_CGuard = {AD2_RCV_CGuard}, AD3_RCV_WGuard_Wave = {AD3_RCV_WGuard_Wave}, AD4_RCV_NFC = {AD4_RCV_NFC}, AD4_RCV_WL_CNNT = {AD4_RCV_WL_CNNT}, AD4_RCV_WL_NCNNT = {AD4_RCV_WL_NCNNT}";
            });
        }

    }
}
