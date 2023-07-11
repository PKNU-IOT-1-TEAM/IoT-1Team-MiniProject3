using MahApps.Metro.Controls;
using System;
using System.IO.Packaging;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using System.Text;

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
        private string p_topic = "TEAM_ONE/parking/s_data/";            // 발행할 토픽
        private string s_topic = "TEAM_ONE/parking/data/";            // 구독할 토픽

        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
                mqttClient.Connect(Guid.NewGuid().ToString());
            }

            var data = new { AD2_CGuard = -1, AD3_WGuard_Wave = 1};  // 전송할 데이터 json형식으로 생성
            string json = JsonConvert.SerializeObject(data);
            
            mqttClient.Publish(p_topic, Encoding.UTF8.GetBytes(json));
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {

            if (mqttClient == null)
            {
                mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                mqttClient.Connect(Guid.NewGuid().ToString());
                mqttClient.Subscribe(new string[] { s_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }


            
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // MQTT message received event handler
            string message = Encoding.UTF8.GetString(e.Message);
            // Parse received JSON data and perform desired processing
            var data = JsonConvert.DeserializeObject<dynamic>(message);

            // Extract individual values from JSON data
            int IR_Sensor = data.IR_Sensor;
            int Temperature = data.Temperature;
            int Humidity = data.Humidity;
            int? AD2_RCV_CGuard = data.AD2_RCV_CGuard;
            int? AD3_RCV_WGuard_Wave = data.AD3_RCV_WGuard_Wave;
            string NFC = data.NFC;
            int WL_CNNT = data.WL_CNNT;
            int WL_NCNNT = data.WL_NCNNT;

            // Update UI or perform other logic with the received data
            Dispatcher.Invoke(() =>
            {
                OutputText.Text = $"Received data: IR Sensor = {IR_Sensor}, Temperature = {Temperature}, Humidity = {Humidity}, AD2_RCV_CGuard = {AD2_RCV_CGuard}, AD3_RCV_WGuard_Wave = {AD3_RCV_WGuard_Wave}, NFC = {NFC}, WL_CNNT = {WL_CNNT}, WL_NCNNT = {WL_NCNNT}";
            });
        }

    }
}
